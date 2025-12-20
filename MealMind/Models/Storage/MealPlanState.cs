using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealMind.Models.Storage
{
    // MealPlanState
    // Represents the persisted meal plan stored in mealplan.json.
    // Stores meal assignments and completion status by date.
    public sealed class MealPlanState
    {
        // Key: yyyy-MM-dd -> MealId
        public Dictionary<string, string> DayToMealId { get; set; } = new();

        // Tracks which days the user has marked as completed.
        public HashSet<string> CompletedDays { get; set; } = new();
    }
}
