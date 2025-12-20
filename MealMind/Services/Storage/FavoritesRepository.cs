using MealMind.Models;
using MealMind.Models.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealMind.Services.Storage
{
    // FavoritesRepository
    // Stores the user's favorite meal IDs in a small state file (state/favorites.json).
    // Uses RecipeRepository to ensure the full Recipe is cached locally when a meal is favorited
    // (Enables fast loading + offline access).
    public sealed class FavoritesRepository
    {
        // Favorites are app state (small data), so we store them in the "state" folder as one JSON file
        private const string FileName = "favorites.json";

        // LocalStoreService handles JSON + file IO.
        // RecipeRepository stores full recipes separately (recipes/{mealId}.json).
        private readonly LocalStoreService _store;
        private readonly RecipeRepository _recipes;

        // Constructor setting of dependencies.
        public FavoritesRepository(LocalStoreService store, RecipeRepository recipes)
        {
            _store = store;
            _recipes = recipes;
        }

        // Path to the favorites state file.
        private string Path => _store.StatePath(FileName);


        // Loads favorites state from disk.
        // If the file doesn't exist yet, return an empty state.
        public async Task<FavoritesState> GetAsync()
            => await _store.ReadJsonAsync<FavoritesState>(Path) ?? new FavoritesState();

        // Checks if a meal ID is favorited.
        public async Task<bool> IsFavoriteAsync(string mealId)
        {
            var state = await GetAsync();
            return state.MealIds.Contains(mealId);
        }



        // Adds/removes a favourite.
        // When favouriting we also save the full recipe locally, so the favourite can be opened offline.
        // Business rule: we do not persist "step tick" progress between sessions.
        public async Task SetFavoriteAsync(Recipe recipe, bool isFavorite)
        {
            if (string.IsNullOrWhiteSpace(recipe.IdMeal))
                throw new ArgumentException("Recipe.IdMeal is required.", nameof(recipe));

            // If favoriting, ensure recipe is stored locally
            if (isFavorite)
            {
                // Mever persist checkbox ticks (progress is UI-only)
                // Avoids confusion when reopening the app later.
                if (recipe.Steps != null)
                    foreach (var s in recipe.Steps) s.IsDone = false;

                await _recipes.SaveAsync(recipe);
            }

            var state = await GetAsync();

            if (isFavorite) state.MealIds.Add(recipe.IdMeal);
            else state.MealIds.Remove(recipe.IdMeal);

            await _store.WriteJsonAtomicAsync(Path, state);
        }


        // Returns the list of favourite meal IDs.
        // The Favorites page can then load full Recipe objects from RecipeRepository as needed.
        public async Task<List<string>> GetMealIdsAsync()
        {
            var state = await GetAsync();
            return state.MealIds.ToList();
        }
}
}
