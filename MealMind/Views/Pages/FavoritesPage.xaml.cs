using MealMind.ViewModels;

namespace MealMind.Views.Pages;

// Code-behind for the Favorites page.
// Responsible only for wiring the ViewModel and triggering refresh on page appearance.
public partial class FavoritesPage : ContentPage
{
    // Initialize the page and assigns its ViewModel as the BindingContext.
    public FavoritesPage()
    {
        InitializeComponent();
        BindingContext = new FavoritesPageViewModel();
    }

    // On page appearing, triggers a refresh of the ViewModel data.
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is FavoritesPageViewModel vm)
            await vm.RefreshAsync();
    }
}