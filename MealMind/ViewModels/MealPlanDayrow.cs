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

    // MealPlanDayRow
    // Lightweight bindable row model for the Meal Plan list UI.
    // Represents a single day and the meal assigned to that day (plus completion state).
    // Inherits BindableObject so individual fields can update the UI without rebuilding the whole list.
    public sealed class MealPlanDayRow : BindableObject
    {
        // Stable string key used by MealPlanState (yyyy-MM-dd).
        public DateTime Date { get; init; }
        public string DateKey => MealPlanRepository.DateKey(Date);
        public string DayName => Date.ToString("dddd"); // Monday, Tuesday...
        public string DateShort => Date.ToString("dd MMM");


        // Meal ID assigned for this day (null/empty means no meal planned).
        // HasMeal is used by the UI to show/hide meal content.
        private string? _mealId;
        // Also notify HasMeal when MealId changes so visibility bindings update.
        public string? MealId { get => _mealId; set { _mealId = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasMeal)); } }
        public bool HasMeal => !string.IsNullOrWhiteSpace(MealId);


        // Meal name (or "No meal planned" if none).
        private string _mealName = "No meal planned";
        public string MealName { get => _mealName; set { _mealName = value; OnPropertyChanged(); } }

        // Thumbnail URL for displaying the meal image in the plan list.
        private string? _thumb;
        public string? Thumb { get => _thumb; set { _thumb = value; OnPropertyChanged(); } }

        // Whether the meal for this day is marked as completed.
        private bool _isCompleted;
        public bool IsCompleted { get => _isCompleted; set { _isCompleted = value; OnPropertyChanged(); } }
    }

   
}
