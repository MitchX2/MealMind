using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealMind.Models.DTO
{
    // DTOs for TheMealDB search and filter endpoints.
    // These endpoints return lightweight meal summaries rather than full recipe details.
    public class TheMealDbSearchDtos
    {
        // Response wrapper for search.php?s=...
        public class SearchByNameResponseDto
        {
            public List<MealSummaryDto>? meals { get; set; }
        }

        // Lightweight meal summary returned by search endpoints.
        public class MealSummaryDto
        {
            public string? idMeal { get; set; }
            public string? strMeal { get; set; }
            public string? strMealThumb { get; set; }
        }

        // Response wrapper for filter endpoints (ingredient, category, area).
        public class FilterResponseDto
        {
            public List<MealFilterDto>? meals { get; set; }
        }

        // Lightweight meal summary returned by filter endpoints.
        // Similar to MealSummaryDto but kept separate when trying to make multiple filters
        public class MealFilterDto
        {
            public string? idMeal { get; set; }
            public string? strMeal { get; set; }
            public string? strMealThumb { get; set; }
        }
    }
}
