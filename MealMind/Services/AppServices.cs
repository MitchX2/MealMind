using global::MealMind.Services.Settings;
using global::MealMind.Services.Storage;
   
namespace MealMind.Services
{
    // AppServices
    // Central container for shared application services.
    // Creates and holds single instances of repositories, storage, and settings.
    // This avoids passing multiple services into every ViewModel and keeps dependencies consistent.
    public sealed class AppServices
    {

        // Shared service instances used throughout the app.
        // All repositories share the same LocalStoreService instance.
        public LocalStoreService Store { get; }

        public RecipeRepository Recipes { get; }
        public FavoritesRepository Favorites { get; }
        public MealPlanRepository MealPlan { get; }
        public ShoppingListRepository ShoppingList { get; }
        public SettingsService Settings { get; }


        
        public AppServices()
        {
            // single storage instance for the app
            Store = new LocalStoreService();

            Recipes = new RecipeRepository(Store);
            Favorites = new FavoritesRepository(Store, Recipes);
            MealPlan = new MealPlanRepository(Store, Recipes);
            ShoppingList = new ShoppingListRepository(Store);
            Settings = new SettingsService();
        }
    }
}
    
    
