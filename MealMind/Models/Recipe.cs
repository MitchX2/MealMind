using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MealMind.Models.DTO;
namespace MealMind.Models
{
    public class Recipe
    {
        // From API
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

        public ObservableCollection<IngredientLine> Ingredients { get; init; } = new();
        public ObservableCollection<InstructionStep> Steps { get; init; } = new();



        // App-specific
        public bool IsFavourite { get; set; }
        public int TimesCooked { get; set; }
        public DateTime DateSaved { get; set; } = DateTime.UtcNow;
        public bool OnMealCalendar { get; set; }


       

        
    }
}
