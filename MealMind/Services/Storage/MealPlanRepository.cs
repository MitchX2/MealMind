using MealMind.Models;
using MealMind.Models.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealMind.Services.Storage
{

    // MealPlanRepository
    // Stores the user's meal plan in a small state file (state/mealplan.json).
    // The plan only stores meal IDs per day; full Recipe objects are cached separately via RecipeRepository.
    // This keeps the plan file small and supports offline access.
    public sealed class MealPlanRepository
    {

        // Meal plan is "app state", so it stays in the state folder as JSON.
        private const string FileName = "mealplan.json";
        private readonly LocalStoreService _store;
        private readonly RecipeRepository _recipes;

        public MealPlanRepository(LocalStoreService store, RecipeRepository recipes)
        {
            _store = store;
            _recipes = recipes;
        }

        private string Path => _store.StatePath(FileName);


        // Loads the meal plan from disk.
        // If there is no saved plan yet, return empty.
        public async Task<MealPlanState> GetAsync()
            => await _store.ReadJsonAsync<MealPlanState>(Path) ?? new MealPlanState();


        // Assigns a recipe to a specific day.
        // Also caches the full recipe locally so the plan works offline.
        // step tick progress is UI-only, so IsDone is reset before saving.
        public async Task SetMealAsync(DateTime day, Recipe recipe)
        {
            if (string.IsNullOrWhiteSpace(recipe.IdMeal))
                throw new ArgumentException("Recipe.IdMeal is required.", nameof(recipe));

            
            if (recipe.Steps != null)
                foreach (var s in recipe.Steps) s.IsDone = false;
            // Ensure recipe exists on disk when it’s in the plan
            await _recipes.SaveAsync(recipe);

            var state = await GetAsync();
            var key = DateKey(day);

            // reset completion if replacing
            state.DayToMealId[key] = recipe.IdMeal;
            state.CompletedDays.Remove(key); 

            await _store.WriteJsonAtomicAsync(Path, state);
        }

        // Removes the assigned meal and completion status for a given day.
        public async Task ClearMealAsync(DateTime day)
        {
            var state = await GetAsync();
            var key = DateKey(day);

            state.DayToMealId.Remove(key);
            state.CompletedDays.Remove(key);

            await _store.WriteJsonAtomicAsync(Path, state);
        }


        // Marks a day as completed/uncompleted.
        // Only allowing if a meal is assigned prevents "completing" an empty day
        public async Task ToggleCompletedAsync(DateTime day)
        {
            var state = await GetAsync();
            var key = DateKey(day);

            // Only allow complete if a meal is assigned
            if (!state.DayToMealId.ContainsKey(key))
                return;

            if (state.CompletedDays.Contains(key)) state.CompletedDays.Remove(key);
            else state.CompletedDays.Add(key);

            await _store.WriteJsonAtomicAsync(Path, state);
        }

        // Converts a DateTime into a key for dictionaries and JSON storage.
        // yyyy-MM-dd avoids timezone/time-of-day issues and is easier to read in the saved file
        public static string DateKey(DateTime day) => day.ToString("yyyy-MM-dd");
    }
}
