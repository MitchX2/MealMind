using MealMind.Utilities;
using MealMind.ViewModels;

namespace MealMind.Views.Pages
{


    // MealPlanPage
    // Code-behind for the Meal Plan page.
    // Responsible for wiring the ViewModel, calling refresh on appearing,
    // and handling UI events that trigger navigation or ViewModel actions.
    public partial class MealPlanPage : ContentPage
    {
        // Initialize the page and assigns its ViewModel as the BindingContext.
        public MealPlanPage()
        {
            InitializeComponent();
            BindingContext = new MealPlanPageViewModel();
        }

        // On page appearing, triggers a refresh of the ViewModel data.
        protected override async void OnAppearing()
        {
            base.OnAppearing();


            if (BindingContext is MealPlanPageViewModel vm)
                await vm.RefreshAsync();
        }

        // Event handler for navigating to the previous week's meal plan.
        private async void PrevWeek_Clicked(object sender, EventArgs e)
        {
            if (BindingContext is MealPlanPageViewModel vm)
                await vm.LoadPrevWeekAsync();
        }

        // Event handler for navigating to the next week's meal plan.
        private async void NextWeek_Clicked(object sender, EventArgs e)
        {
            if (BindingContext is MealPlanPageViewModel vm)
                await vm.LoadNextWeekAsync();
        }

        // "Add meal" for a specific day: navigate to SearchPage and pass the target date as a query parameter.
        private async void AddForDay_Clicked(object sender, EventArgs e)
        {
            if ((sender as Button)?.BindingContext is not MealPlanDayRow row)
                return;

            // Navigate to search with target date
            await Shell.Current.GoToAsync(
                $"{nameof(SearchPage)}?targetDate={row.Date:yyyy-MM-dd}");
        }


        // "View meal" for a specific day: navigate to RecipePage and pass the meal ID as a query parameter.
        private async void ViewMeal_Clicked(object sender, EventArgs e)
        {
            if ((sender as Button)?.BindingContext is not MealPlanDayRow row)
                return;

            if (string.IsNullOrWhiteSpace(row.MealId))
                return;

            await Shell.Current.GoToAsync(
                $"{nameof(RecipePage)}?mealId={row.MealId}");
        }


        // "Clear day" for a specific day: calls the ViewModel to clear the meal plan for that day.
        private async void ClearDay_Clicked(object sender, EventArgs e)
        {
            if (BindingContext is MealPlanPageViewModel vm &&
                (sender as Button)?.BindingContext is MealPlanDayRow row)
            {
                await vm.ClearDayAsync(row);
            }
        }

        // Checkbox changed for marking a day's meal as completed/not completed.
        private async void Completed_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (BindingContext is MealPlanPageViewModel vm &&
                (sender as CheckBox)?.BindingContext is MealPlanDayRow row)
            {
                await vm.ToggleCompletedAsync(row);
            }
        }

        
    }
}