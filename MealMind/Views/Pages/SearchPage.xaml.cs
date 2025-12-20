
using MealMind.ViewModels;

namespace MealMind.Views.Pages
{

    // Query params: for loading search from MainPage
    [QueryProperty(nameof(Q), "q")]
    [QueryProperty(nameof(Ing), "ing")]
    [QueryProperty(nameof(Adv), "adv")]

    // Query param: for adding meal to meal plan on a specific date
    [QueryProperty(nameof(TargetDate), "targetDate")]


    // Shell query params:
    //  q / ing / adv are used when navigating from MainPage to pre-fill and optionally open advanced filters.
    //  targetDate is used when navigating from MealPlan so selected recipes can be assigned to a day.
    public partial class SearchPage : ContentPage
    {
        // Store query values until the ViewModel is available (applied in OnAppearing).
        private string _q = "";
        private string _ing = "";
        private string _adv = "";
        // _autoSearchDone prevents repeated auto-search when returning to the page.
        private bool _autoSearchDone;


        // Taking in parameters from query string
        public string? Q { set => _q = value ?? ""; }
        public string? Ing { set => _ing = value ?? ""; }
        public string? Adv { set => _adv = value ?? ""; }


        // MealPlanner integration: target date for adding meal
        private string? _targetDate;
        public string? TargetDate
        {
            get => _targetDate;
            set
            {
                _targetDate = value;

                if (BindingContext is SearchPageViewModel vm)
                    vm.TargetDate = _targetDate;
            }
        }

        // Initializes page and assigns SearchPageViewModel as BindingContext.
        public SearchPage()
        {
            InitializeComponent();
            BindingContext = new SearchPageViewModel();
        }


        // Apply query parameters to the ViewModel when the page becomes visible.
        // This updates bound UI controls (Entry fields) via MVVM.
        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is not SearchPageViewModel vm)
                return;

            // Apply query params to VM (this updates the Entry via binding)
            if (!string.IsNullOrWhiteSpace(_q))
                vm.SearchText = _q;

            if (!string.IsNullOrWhiteSpace(_ing))
                vm.IngredientsText = _ing;

            if (!string.IsNullOrWhiteSpace(_adv))
            {
                var open = string.Equals(_adv, "true", StringComparison.OrdinalIgnoreCase);
                vm.AreFiltersOpen = open;
                vm.IsAdvancedOpen = open; 
            }

            // Auto-run search once per navigation to refresh the page
            if (!_autoSearchDone && vm.Results.Count == 0)
            {
                _autoSearchDone = true;
                vm.SearchCommand.Execute(null);
            }
        }
    }
}