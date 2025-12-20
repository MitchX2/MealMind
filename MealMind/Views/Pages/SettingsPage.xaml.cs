using MealMind.ViewModels;

namespace MealMind.Views.Pages;


public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();
        BindingContext = new SettingsPageViewModel();
    }
}