using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using MealMind.Models;
using MealMind.Models.DTO;

namespace MealMind.Utilities
{

    // MealLookupDtoExtensions
    // Maps TheMealDB lookup/random DTO (MealLookupDto) into the app's Recipe model.
    // Keeps API-specific parsing out of ViewModels and UI code.
    public static class MealLookupDtoExtensions
    {

        // Converts a MealLookupDto (API-shaped object) into a Recipe (app model).
        // Used when loading full meal details from lookup.php/random.php before displaying or caching locally.
        public static Recipe ToRecipe(this MealLookupDto dto)
        {
            // Null protection: mapping requires a valid DTO and an ID to use as the storage key.

            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var id = dto.idMeal?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("MealLookupDto.idMeal is required to map to Recipe.");



            var tags = (dto.strTags ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();


            // Builds a clean ingredient list.
            var ingredients = BuildIngredients(dto);
            var steps = BuildSteps(dto.strInstructions);

            // Construct the new Recipe and save the current UTC time as DateSaved.
            return new Recipe
            {
                IdMeal = id,
                Name = dto.strMeal?.Trim() ?? "",
                Category = dto.strCategory?.Trim(),
                Area = dto.strArea?.Trim(),
                TagsRaw = dto.strTags,
                Tags = tags,
                YoutubeUrl = dto.strYoutube,
                SourceUrl = dto.strSource,
                ImageUrl = dto.strMealThumb,
                Ingredients = new ObservableCollection<IngredientLine>(ingredients),
                Steps = new ObservableCollection<InstructionStep>(steps),
                DateSaved = DateTime.UtcNow
            };
        }



        // Converts raw instruction text into numbered steps for UI display.
        // Uses newline splitting first, with a sentence-split fallback for "one giant paragraph" recipes.
        private static List<IngredientLine> BuildIngredients(MealLookupDto dto)
        {
            // Pair up ingredient + measure 1..20
            var list = new List<IngredientLine>();

            string?[] ings =
            {
            dto.strIngredient1, dto.strIngredient2, dto.strIngredient3, dto.strIngredient4, dto.strIngredient5,
            dto.strIngredient6, dto.strIngredient7, dto.strIngredient8, dto.strIngredient9, dto.strIngredient10,
            dto.strIngredient11, dto.strIngredient12, dto.strIngredient13, dto.strIngredient14, dto.strIngredient15,
            dto.strIngredient16, dto.strIngredient17, dto.strIngredient18, dto.strIngredient19, dto.strIngredient20
        };

            string?[] meas =
            {
            dto.strMeasure1, dto.strMeasure2, dto.strMeasure3, dto.strMeasure4, dto.strMeasure5,
            dto.strMeasure6, dto.strMeasure7, dto.strMeasure8, dto.strMeasure9, dto.strMeasure10,
            dto.strMeasure11, dto.strMeasure12, dto.strMeasure13, dto.strMeasure14, dto.strMeasure15,
            dto.strMeasure16, dto.strMeasure17, dto.strMeasure18, dto.strMeasure19, dto.strMeasure20
        };

            for (int i = 0; i < ings.Length; i++)
            {
                var ing = ings[i]?.Trim();
                if (string.IsNullOrWhiteSpace(ing)) continue;

                var m = meas[i]?.Trim() ?? "";

                list.Add(new IngredientLine
                {
                    Name = ing,
                    MeasureRaw = m
                });
            }

            return list;
        }

        private static List<InstructionStep> BuildSteps(string? instructions)
        {
            if (string.IsNullOrWhiteSpace(instructions))
                return new List<InstructionStep>();

            // Simple split: lines + sentences fallback
            var lines = instructions
                .Replace("\r\n", "\n")
                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Where(s => s.Length > 0)
                .ToList();

            // If everything is one giant line, split on ". "
            if (lines.Count == 1 && lines[0].Length > 200)
            {
                lines = lines[0]
                    .Split(". ", StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim().TrimEnd('.'))
                    .Where(s => s.Length > 0)
                    .ToList();
            }

            var steps = new List<InstructionStep>();
            for (int i = 0; i < lines.Count; i++)
            {
                steps.Add(new InstructionStep
                {
                    Number = i + 1,
                    Text = lines[i],
                    IsDone = false
                });
            }

            return steps;
        }
    }
}
