using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealMind.Models
{
    // IngredientLine
    // Represents a single ingredient and its measurement in a recipe.
    // Used by Recipe and ShoppingList features.
    public class IngredientLine
    {
        public string Name { get; init; } = "";     // "Soy Sauce"
        public string MeasureRaw { get; init; } = ""; // "3/4 cup" (keep original text)

        // Parsed values for conversions
        public decimal? Quantity { get; init; }     // e.g. 0.75
        public string? Unit { get; init; }          // "cup", "g", "ml"
    }

    
}
