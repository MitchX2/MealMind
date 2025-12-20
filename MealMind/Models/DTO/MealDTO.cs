using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

using MealMind.Models;

namespace MealMind.Models.DTO
{

    // MealDTO
    // Data Transfer Object (DTO) that matches TheMealDB JSON structure exactly.
    // Used only for deserialization (API-shaped data), then mapped into the internal Recipe model.
    // TheMealDB uses numbered fields for ingredients/measures, so helper methods below make that easier to work with.
    public class MealDTO
    {
        // Taking in every field as the TheMealDB API provides it
        [JsonPropertyName("idMeal")]
        public string? IdMeal { get; set; }

        [JsonPropertyName("strMeal")]
        public string? StrMeal { get; set; }

        [JsonPropertyName("strMealAlternate")]
        public string? StrMealAlternate { get; set; }

        [JsonPropertyName("strCategory")]
        public string? StrCategory { get; set; }

        [JsonPropertyName("strArea")]
        public string? StrArea { get; set; }

        [JsonPropertyName("strInstructions")]
        public string? StrInstructions { get; set; }

        [JsonPropertyName("strMealThumb")]
        public string? StrMealThumb { get; set; }

        [JsonPropertyName("strTags")]
        public string? StrTags { get; set; }

        [JsonPropertyName("strYoutube")]
        public string? StrYoutube { get; set; }

        [JsonPropertyName("strSource")]
        public string? StrSource { get; set; }

        [JsonPropertyName("strImageSource")]
        public string? StrImageSource { get; set; }

        [JsonPropertyName("strCreativeCommonsConfirmed")]
        public string? StrCreativeCommonsConfirmed { get; set; }

        [JsonPropertyName("dateModified")]
        public string? DateModified { get; set; }


        // TheMealDB API has up to 20 ingredients and measures per meal
        // Ingredients 1->20
        [JsonPropertyName("strIngredient1")] 
        public string? StrIngredient1 { get; set; }

        [JsonPropertyName("strIngredient2")] 
        public string? StrIngredient2 { get; set; }

        [JsonPropertyName("strIngredient3")] 
        public string? StrIngredient3 { get; set; }

        [JsonPropertyName("strIngredient4")] 
        public string? StrIngredient4 { get; set; }

        [JsonPropertyName("strIngredient5")] 
        public string? StrIngredient5 { get; set; }
        
        [JsonPropertyName("strIngredient6")] 
        public string? StrIngredient6 { get; set; }
        
        [JsonPropertyName("strIngredient7")] 
        public string? StrIngredient7 { get; set; }
        [JsonPropertyName("strIngredient8")] 
        public string? StrIngredient8 { get; set; }

        [JsonPropertyName("strIngredient9")] 
        public string? StrIngredient9 { get; set; }

        [JsonPropertyName("strIngredient10")] 
        public string? StrIngredient10 { get; set; }

        [JsonPropertyName("strIngredient11")] 
        public string? StrIngredient11 { get; set; }

        [JsonPropertyName("strIngredient12")] 
        public string? StrIngredient12 { get; set; }

        [JsonPropertyName("strIngredient13")] 
        public string? StrIngredient13 { get; set; }

        [JsonPropertyName("strIngredient14")] 
        public string? StrIngredient14 { get; set; }

        [JsonPropertyName("strIngredient15")] 
        public string? StrIngredient15 { get; set; }

        [JsonPropertyName("strIngredient16")] 
        public string? StrIngredient16 { get; set; }

        [JsonPropertyName("strIngredient17")] 
        public string? StrIngredient17 { get; set; }

        [JsonPropertyName("strIngredient18")] 
        public string? StrIngredient18 { get; set; }

        [JsonPropertyName("strIngredient19")] 
        public string? StrIngredient19 { get; set; }

        [JsonPropertyName("strIngredient20")] 
        public string? StrIngredient20 { get; set; }


        // Measures 1->20
        [JsonPropertyName("strMeasure1")] 
        public string? StrMeasure1 { get; set; }

        [JsonPropertyName("strMeasure2")] 
        public string? StrMeasure2 { get; set; }

        [JsonPropertyName("strMeasure3")] 
        public string? StrMeasure3 { get; set; }

        [JsonPropertyName("strMeasure4")] 
        public string? StrMeasure4 { get; set; }

        [JsonPropertyName("strMeasure5")] 
        public string? StrMeasure5 { get; set; }

        [JsonPropertyName("strMeasure6")] 
        public string? StrMeasure6 { get; set; }

        [JsonPropertyName("strMeasure7")] 
        public string? StrMeasure7 { get; set; }

        [JsonPropertyName("strMeasure8")] 
        public string? StrMeasure8 { get; set; }

        [JsonPropertyName("strMeasure9")] 
        public string? StrMeasure9 { get; set; }

        [JsonPropertyName("strMeasure10")] 
        public string? StrMeasure10 { get; set; }

        [JsonPropertyName("strMeasure11")] 
        public string? StrMeasure11 { get; set; }

        [JsonPropertyName("strMeasure12")] 
        public string? StrMeasure12 { get; set; }

        [JsonPropertyName("strMeasure13")] 
        public string? StrMeasure13 { get; set; }

        [JsonPropertyName("strMeasure14")] 
        public string? StrMeasure14 { get; set; }

        [JsonPropertyName("strMeasure15")] 
        public string? StrMeasure15 { get; set; }

        [JsonPropertyName("strMeasure16")] 
        public string? StrMeasure16 { get; set; }

        [JsonPropertyName("strMeasure17")] 
        public string? StrMeasure17 { get; set; }

        [JsonPropertyName("strMeasure18")] 
        public string? StrMeasure18 { get; set; }

        [JsonPropertyName("strMeasure19")] 
        public string? StrMeasure19 { get; set; }

        [JsonPropertyName("strMeasure20")] 
        public string? StrMeasure20 { get; set; }





        // Transfer Methods lets us access ingredients and measures by index
        public string? GetIngredient(int index) => index switch
        {
            1 => StrIngredient1,
            2 => StrIngredient2,
            3 => StrIngredient3,
            4 => StrIngredient4,
            5 => StrIngredient5,
            6 => StrIngredient6,
            7 => StrIngredient7,
            8 => StrIngredient8,
            9 => StrIngredient9,
            10 => StrIngredient10,
            11 => StrIngredient11,
            12 => StrIngredient12,
            13 => StrIngredient13,
            14 => StrIngredient14,
            15 => StrIngredient15,
            16 => StrIngredient16,
            17 => StrIngredient17,
            18 => StrIngredient18,
            19 => StrIngredient19,
            20 => StrIngredient20,
            _ => null
            
        };

        public string? GetMeasure(int index) => index switch
        {
            1 => StrMeasure1,
            2 => StrMeasure2,
            3 => StrMeasure3,
            4 => StrMeasure4,
            5 => StrMeasure5,
            6 => StrMeasure6,
            7 => StrMeasure7,
            8 => StrMeasure8,
            9 => StrMeasure9,
            10 => StrMeasure10,
            11 => StrMeasure11,
            12 => StrMeasure12,
            13 => StrMeasure13,
            14 => StrMeasure14,
            15 => StrMeasure15,
            16 => StrMeasure16,
            17 => StrMeasure17,
            18 => StrMeasure18,
            19 => StrMeasure19,
            20 => StrMeasure20,
            _ => null
        };


        // Enumerates all non-empty ingredients with their corresponding measures
        //  Allows us to travel the 20 possible ingredient/measure pairs easily
        public IEnumerable<(string Ingredient, string Measure)> EnumerateIngredients()
        {
            // TheMealDB API supports up to 20 ingredients per meal
            for (int i = 1; i <= 20; i++)
            {
                var ing = GetIngredient(i)?.Trim();
                // If blank skip (some meals are missing ingredients in slots)
                if (string.IsNullOrWhiteSpace(ing)) continue;

                var meas = GetMeasure(i)?.Trim() ?? "";

                // Hand back the ingredient/measure pair
                yield return (ing, meas);
            }
        }

    }
}
