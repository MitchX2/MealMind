using MealMind.Services.Storage;
using MealMind.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealMind.ViewModels
{

    // MealPlanPageViewModel
    // ViewModel for the Meal Plan page.
    // Loads the saved plan state for a given week and builds a bindable list of MealPlanDayRow items.
    // Uses repositories for persistence and keeps UI-friendly formatting here (week labels, busy state).
    public class MealPlanPageViewModel : BindableObject
    {
        // Repositories come from the central AppServices container.
        // MealPlanRepository stores the plan state (day -> mealId + completed days).
        // RecipeRepository loads cached recipes for display (name + thumbnail).
        private readonly MealPlanRepository _mealPlan = App.Services.MealPlan;
        private readonly RecipeRepository _recipes = App.Services.Recipes;

        // Bound to the UI list. Each row represents one day in the selected week.
        public ObservableCollection<MealPlanDayRow> Days { get; } = new();


        // used to prevent multiple loadWeek() calls triggering non stop refreshing
        private bool _isLoadingWeek;


        // Start date (Monday) of the currently displayed week.
        private DateTime _weekStart;
        public DateTime WeekStart
        {
            get => _weekStart;
            private set { _weekStart = value; OnPropertyChanged(); OnPropertyChanged(nameof(WeekLabel)); }
        }
        // Pre-formatted label for the header (e.g., "02 Dec - 08 Dec").
        public string WeekLabel => $"{WeekStart:dd MMM} - {WeekStart.AddDays(6):dd MMM}";


        // Used by the UI to show a loading indicator and disable actions during refresh.
        private bool _isBusy;
        public bool IsBusy { get => _isBusy; private set { _isBusy = value; OnPropertyChanged(); } }


        // Week navigation helpers used by the page buttons.
        public async Task LoadThisWeekAsync()
            => await LoadWeekAsync(DateHelpers.StartOfWeekMonday(DateTime.Today));

        public async Task LoadPrevWeekAsync()
            => await LoadWeekAsync(WeekStart.AddDays(-7));

        public async Task LoadNextWeekAsync()
            => await LoadWeekAsync(WeekStart.AddDays(7));



        // Called by the page when returning to this tab/page to refresh the currently selected week.
        public async Task RefreshAsync()
        {
            if (WeekStart == default)
                await LoadThisWeekAsync();
            else
                await LoadWeekAsync(WeekStart);
        }



        // Loads plan state and rebuilds the Days collection for the given week.
        // 1) Read MealPlanState (meal IDs + completed days)
        // 2) Create one MealPlanDayRow per day
        // 3) Resolve meal IDs into cached Recipe details for display
        public async Task LoadWeekAsync(DateTime weekStart)
        {
            // Repeated call delay
            if (_isLoadingWeek) return;
            _isLoadingWeek = true;

            // UI delay setting
            IsBusy = true;
            try
            {
                // Normalize week start to date only
                WeekStart = weekStart.Date;

                Days.Clear();

                // Load saved plan state
                var state = await _mealPlan.GetAsync();
                var weekDays = DateHelpers.DaysInWeek(WeekStart);

                // Build one row per day
                foreach (var d in weekDays)
                {
                    // Get meal ID for this day (if any)
                    var key = MealPlanRepository.DateKey(d);
                    //  Try get meal ID
                    state.DayToMealId.TryGetValue(key, out var mealId);
                    
                    // Create row and populate
                    var row = new MealPlanDayRow
                    {
                        Date = d,
                        MealId = mealId,
                        IsCompleted = state.CompletedDays.Contains(key)
                    };

                    // If we have a meal ID
                    if (!string.IsNullOrWhiteSpace(mealId))
                    {
                        // Load stored recipe (it should exist because we save when planning)
                        var recipe = await _recipes.GetAsync(mealId);
                        if (recipe != null)
                        {
                            row.MealName = recipe.Name;
                            row.Thumb = recipe.ImageUrl;
                        }
                        else
                        {
                            // Should not happen, but just in case
                            row.MealName = "Saved recipe missing";
                        }
                    }

                    Days.Add(row);
                }
            }
            finally
            {
                
                IsBusy = false;
                _isLoadingWeek = false;
            }
        }

        // Clears the planned meal for the given day and refreshes the week.
        public async Task ClearDayAsync(MealPlanDayRow row)
        {
            await _mealPlan.ClearMealAsync(row.Date);
            await LoadWeekAsync(WeekStart);
        }

        // Toggles the completed state for the given day and refreshes the week.
        public async Task ToggleCompletedAsync(MealPlanDayRow row)
        {
            await _mealPlan.ToggleCompletedAsync(row.Date);
            await LoadWeekAsync(WeekStart);
        }
    }
}
