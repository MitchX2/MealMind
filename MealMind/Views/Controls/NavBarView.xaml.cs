using MealMind.Views.Pages;

namespace MealMind.Views.Controls;

public partial class NavBarView : ContentView
{
	public NavBarView()
	{
		InitializeComponent();
	
		
	
	}

    private async void Settings_Clicked(object sender, EventArgs e)
    => await Shell.Current.GoToAsync(nameof(SettingsPage));

    private async void Shopping_Clicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync(nameof(ShoppingListPage));

    private async void Favourites_Clicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync(nameof(FavoritesPage));

    private async void MealPlan_Clicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync(nameof(MealPlanPage));

    private async void Search_Clicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync(nameof(SearchPage));

}