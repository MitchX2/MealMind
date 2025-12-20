using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MealMind.Models;

namespace MealMind.Models.DTO
{
    // MealDTOExtensions
    // Maps API DTOs (MealDTO) into app models (Recipe).
    // Keeps API parsing/mapping out of ViewModels so the UI layer stays clean.
    public static class MealDTOExtensions
    {

        // Allows us to fill a recipe class from a MealDTO
        //  Only to be used for new Recipe creation
        public static Recipe ToRecipe(this MealDTO dto)
        {
            // API fields can be null, so  normalised to a safe strings
            var id = dto.IdMeal?.Trim() ?? "";
            var name = dto.StrMeal?.Trim() ?? "";


            // Create the recipe object
            var recipe = new Recipe
            {
                // Core fields
                IdMeal = id,
                Name = name,
                AlternateName = dto.StrMealAlternate?.Trim(),
                Category = dto.StrCategory?.Trim(),
                Area = dto.StrArea?.Trim(),
                TagsRaw = dto.StrTags?.Trim(),
                YoutubeUrl = dto.StrYoutube?.Trim(),
                SourceUrl = dto.StrSource?.Trim(),
                ImageUrl = dto.StrMealThumb?.Trim(),
                ImageSource = dto.StrImageSource?.Trim(),
                CreativeCommonsConfirmed = ParseCreativeCommons(dto.StrCreativeCommonsConfirmed),
                DateModified = ParseDateModified(dto.DateModified),

                // App-specific fields default on creation
                // That are tracked locally for favourites, usage counters, and offline features.
                IsFavourite = false,
                TimesCooked = 0,
                DateSaved = DateTime.UtcNow,
                OnMealCalendar = false
            };


            // Convert DTO ingredient/measure pairs into IngredientLine objects used by the UI.
            foreach (var (ingredient, measure) in dto.EnumerateIngredients())
            {
                recipe.Ingredients.Add(new IngredientLine
                {
                    Name = ingredient,
                    MeasureRaw = measure
                });
            }

            // Instructions -> steps
            // Split the raw instruction text into numbered steps for display and optional checkbox ticking.
            foreach (var step in SplitInstructions(dto.StrInstructions))
            {
                recipe.Steps.Add(step);
            }

            recipe.Tags.AddRange(ParseTags(dto.StrTags));


            return recipe;
        }


        // Converts the API's "Yes/No/1/0" style value into a nullable bool.

        private static bool? ParseCreativeCommons(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;

            var v = value.Trim();

            if (v.Equals("Yes", StringComparison.OrdinalIgnoreCase)) return true;
            if (v.Equals("No", StringComparison.OrdinalIgnoreCase)) return false;

            // Some APIs may return "1"/"0"
            if (v == "1") return true;
            if (v == "0") return false;

            return null;
        }


        // Parses TheMealDB "dateModified" field into a nullable DateTime (UTC).

        private static DateTime? ParseDateModified(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;

            // TheMealDB typically uses "yyyy-MM-dd HH:mm:ss" when present
            if (DateTime.TryParse(value.Trim(), CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                    out var dt))
            {
                return dt;
            }

            return null;
        }



        // Converts the instruction paragraph into individual numbered steps.
        private static IEnumerable<InstructionStep> SplitInstructions(string? instructions)
        {
            if (string.IsNullOrWhiteSpace(instructions))
                yield break;

            // Normalize line endings and split
            var lines = instructions
                // When we come across the \r\n sequence, replace it with \n
                .Replace("\r\n", "\n")
                // Now split to isolate this string
                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Where(s => s.Length > 0);


            // Number steps starting from 1 for UI display.
            int stepNo = 1;

            foreach (var line in lines)
            {

                yield return new InstructionStep
                {
                    // itterate step
                    Number = stepNo++,
                    Text = line,
                    IsDone = false
                };
            }
        }

        // Splits comma-separated tag text into a clean list.
        private static IEnumerable<string> ParseTags(string? tagsRaw)
        {
            if (string.IsNullOrWhiteSpace(tagsRaw))
                yield break;

            foreach (var t in tagsRaw.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                var tag = t.Trim();
                if (tag.Length > 0)
                    yield return tag;
            }
        }


        
    }
}
