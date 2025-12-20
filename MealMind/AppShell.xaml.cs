using MealMind.Views.Pages;

namespace MealMind;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register routes for navigation
        Routing.RegisterRoute(nameof(SearchPage), typeof(SearchPage));
        Routing.RegisterRoute(nameof(RecipePage), typeof(RecipePage));
        Routing.RegisterRoute(nameof(FavoritesPage), typeof(FavoritesPage));
        Routing.RegisterRoute(nameof(MealPlanPage), typeof(MealPlanPage));
        Routing.RegisterRoute(nameof(ShoppingListPage), typeof(ShoppingListPage));
        Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));

        //Routing.RegisterRoute(nameof(PantryPage), typeof(PantryPage));


    }
}


