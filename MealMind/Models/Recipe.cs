using MealMind.Models.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace MealMind.Models
{

    // Recipe
    // Representing a full recipe in the app.
    // Combines data loaded from TheMealDB with app-specific state
    // such as favourites, saved date, and calendar usage.
    public class Recipe
    {
        // From API (mapped from MealDTO / MealLookupDto)
        public string IdMeal { get; init; } = "";
        public string Name { get; init; } = "";
        public string? AlternateName { get; init; }
        public string? Category { get; init; }
        public string? Area { get; init; }
        public string? TagsRaw { get; init; }           // e.g. "Meat,Casserole"
        public List<string> Tags { get; init; } = new();
        public string? YoutubeUrl { get; init; }
        public string? SourceUrl { get; init; }
        public string? ImageUrl { get; init; }          // strMealThumb
        public string? ImageSource { get; init; }
        public bool? CreativeCommonsConfirmed { get; init; }
        public DateTime? DateModified { get; init; }


        // ObservableCollection supports UI updates when ingredients are added/changed.
        public ObservableCollection<IngredientLine> Ingredients { get; init; } = new();
        
        
        // Steps are split and numbered for display and optional checkbox ticking.
        public ObservableCollection<InstructionStep> Steps { get; init; } = new();



        // App-specific fields for tracking key stats.
        public bool IsFavourite { get; set; }
        public int TimesCooked { get; set; }
        public DateTime DateSaved { get; set; } = DateTime.UtcNow;
        public bool OnMealCalendar { get; set; }


       

        
    }
}
