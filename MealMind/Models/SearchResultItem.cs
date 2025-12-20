using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealMind.Models
{

    // SearchResultItem
    // Lightweight UI model used for displaying recipes in search/browse result lists.
    // Contains only the fields needed for list presentation, not full recipe details.
    public class SearchResultItem
    {
        public string MealId { get; set; } = "";
        public string Title { get; set; } = "";
        public string IngredientsPreview { get; set; } = "";
        public ImageSource? MealImage { get; set; }
    }
}
