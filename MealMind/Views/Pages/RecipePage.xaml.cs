using MealMind.ViewModels;

namespace MealMind.Views.Pages
{
    // Shell query properties for mealId and targetDate
    [QueryProperty(nameof(MealId), "mealId")]
    [QueryProperty(nameof(TargetDate), "targetDate")]



    // RecipePage
    // Code-behind for the Recipe detail page.
    // Receives Shell query parameters (mealId + optional targetDate),
    // triggers ViewModel loading, and handles page-level actions (meal plan, favourites, shopping list).
    public partial class RecipePage : ContentPage
    {
        // ViewModel instance for this page.
        private readonly RecipePageViewModel _vm;

        // Preset mealId from query parameter incase null.
        private string _mealId = "";
        public string MealId
        {
            get => _mealId;
            set
            {
                _mealId = value ?? "";

                // Trigger loading the recipe when mealId is set.
                if (!string.IsNullOrWhiteSpace(_mealId))
                    _ = LoadAsync(_mealId);
            }
        }

        // TargetDate from query parameter (for meal planning).
        private string? _targetDate;
        public string? TargetDate
        {   
            get => _targetDate;
            set
            {
                _targetDate = value;
                ApplyTargetDateToPicker();
            }
        }


        // Initializes page + binds ViewModel.
        // Sets a sensible DatePicker default in case no targetDate is provided.
        public RecipePage()
        {
            InitializeComponent();

            _vm = new RecipePageViewModel();
            BindingContext = _vm;

            // sensible default in case no query param is provided
            if (MealPlanDatePicker != null)
                MealPlanDatePicker.Date = DateTime.Today;
        }


        // QueryProperty values may be applied after construction, so re-apply the picker value on appearing.
        protected override void OnAppearing()
        {
            base.OnAppearing();
            // In case query props arrive after constructor
            ApplyTargetDateToPicker();
        }


        // Loads recipe data via the ViewModel and refreshes favourite state for the icon.
        private async Task LoadAsync(string mealId)
        {
            if (string.IsNullOrWhiteSpace(mealId))
                return;

            await _vm.LoadRecipeAsync(mealId);

            // once loaded, make sure favourites icon is correct
            await _vm.RefreshFavoriteFlagAsync();
        }


        // Applies the optional targetDate query parameter to the DatePicker.
        // Falls back to today if targetDate is missing/invalid.
        private void ApplyTargetDateToPicker()
        {
            if (MealPlanDatePicker is null)
                return;

            if (!string.IsNullOrWhiteSpace(_targetDate) &&
                DateTime.TryParse(_targetDate, out var dt))
            {
                MealPlanDatePicker.Date = dt.Date;
            }
            else
            {
                // keep default (today) if not set
                if (MealPlanDatePicker.Date == default)
                    MealPlanDatePicker.Date = DateTime.Today;
            }
        }


        // Adds the current recipe to the meal plan for the selected date.
        private async void AddToMealPlan_Clicked(object sender, EventArgs e)
        {
            if (_vm.Recipe == null)
                return;

            // Save to meal plan repository
            await App.Services.MealPlan.SetMealAsync(MealPlanDatePicker.Date, _vm.Recipe);

            // If we came from MealPlan flow, go back to it
            if (!string.IsNullOrWhiteSpace(TargetDate))
            {
                // returns us to mealplan
                await Shell.Current.GoToAsync("../..");
                return;
            }

            await DisplayAlert("Meal Plan", "Added to meal plan.", "OK");
        }

        // Toggles the favourite status of the current recipe.
        private async void OnFavoritesClicked(object sender, EventArgs e)
        {
            await _vm.ToggleFavoriteAsync();
        }

        // Adds the ingredients of the current recipe to the shopping list.
        private async void AddIngredientsToShopping_Clicked(object sender, EventArgs e)
        {
            if (_vm.Recipe == null) return;

            await App.Services.ShoppingList.AddFromRecipeAsync(_vm.Recipe);
            await DisplayAlert("Shopping List", "Ingredients added.", "OK");
        }

    }
}