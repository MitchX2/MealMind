using MealMind.Models;


namespace MealMind.Services.Storage
{

    // RecipeRepository
    // Provides a clean API for storing and retrieving Recipe objects locally.
    //
    // Implements the Repository pattern to separate data access from ViewModels.
    // Each recipe is stored individually by mealId to support offline access and fast lookup.
    public sealed class RecipeRepository
    {
        // File access handled by LocalStoreService.
        private readonly LocalStoreService _store;
        public RecipeRepository(LocalStoreService store) => _store = store;


        // Loads a saved recipe by mealId.
        // Returns null if the recipe has not been saved locally yet.
        public async Task<Recipe?> GetAsync(string mealId)
            => await _store.ReadJsonAsync<Recipe>(_store.RecipePath(mealId));


        // Saves or updates a recipe locally using its IdMeal as the storage key.
        public async Task SaveAsync(Recipe recipe)
        {   // Validation protects against corrupt or incomplete data being written.
            // It never be blank but prevented incase we add creating new recipes in future.
            if (string.IsNullOrWhiteSpace(recipe.IdMeal))
                throw new ArgumentException("Recipe.IdMeal is required to save.", nameof(recipe));

            await _store.WriteJsonAtomicAsync(_store.RecipePath(recipe.IdMeal), recipe);
        }


        // Removes a locally saved recipe.
        // File deletion Handled by LocalStoreService.
        public Task DeleteAsync(string mealId)
            => _store.DeleteAsync(_store.RecipePath(mealId));
    }
}
