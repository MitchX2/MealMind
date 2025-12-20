using MealMind.Models;
using MealMind.Models.DTO;
using MealMind.Services.Storage;
using MealMind.Utilities;
using System.Collections.ObjectModel;



namespace MealMind.ViewModels;


// RecipePageViewModel
// ViewModel for the Recipe detail page.
// Loads a recipe from TheMealDB by mealId, maps it into the Recipe model,
// and exposes UI state (busy, image visibility, favourite icon/toggle).
// Favourite persistence is handled via FavoritesRepository.
public class RecipePageViewModel : BindableObject
{
    // Data sources:
    //  TheMealDbClient loads full recipe details from the API.
    //  FavoritesRepository persists the user's favourites and caches recipes when favourited.
    private readonly TheMealDbClient _client = new();
    private readonly FavoritesRepository _favorites = App.Services.Favorites;


    // Icon path for the favourite button (updates when IsFavorite changes).
    public string FavoriteIcon => IsFavorite ? "heart_filled.png" : "heart_outline.png";


    // Image presence helpers
    public bool HasImage => !string.IsNullOrWhiteSpace(Recipe?.ImageUrl);
    public bool NoImage => !HasImage;



    // Current recipe displayed by the page.
    // When Recipe changes, we also notify HasImage/NoImage for placeholder image logic.
    private Recipe? _recipe;
    public Recipe? Recipe
    {
        get => _recipe;
        private set
        {
            _recipe = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasImage));
            OnPropertyChanged(nameof(NoImage));
        }
    }



    // Used by the UI to show a loading indicator and prevent actions while loading.
    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            _isBusy = value;
            OnPropertyChanged();
        }
    }

    // Favourite state for the current recipe, triggering icon updates when changed.
    private bool _isFavorite;
    public bool IsFavorite
    {
        get => _isFavorite;
        set
        {
            if (_isFavorite == value) return;
            _isFavorite = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(FavoriteIcon));
        }
    }

    // Whether to show checkboxes next to instruction steps.
    public bool ShowInstructionCheckboxes { get; set; } = true;


    // Loads a recipe by mealId.
    // Called when navigating to RecipePage (mealId passed via Shell query parameters).
    // Updates Recipe, then refreshes favourite status for the icon.
    public async Task LoadRecipeAsync(string mealId)
    {
        if (string.IsNullOrWhiteSpace(mealId))
            return;

        // Set busy state
        IsBusy = true;

        try
        {
            // Load from TheMealDB API
            MealLookupDto? dto = await _client.LookupByIdAsync(mealId);

            // If not found, create a placeholder Recipe
            if (dto == null)
            {
                Recipe = new Recipe
                {
                    // Fallback recipe object avoids null bindings in the UI.
                    IdMeal = mealId,
                    Name = "Recipe not found",
                    Ingredients = new ObservableCollection<IngredientLine>(),
                    Steps = new ObservableCollection<InstructionStep>()
                };

                IsFavorite = false;
                return;
            }

            // Recipe found - map DTO to Recipe model
            Recipe = MapToRecipe(dto);

            // Update favourite state for the icon
            await RefreshFavoriteFlagAsync();
        }
        finally
        {
            // Clear busy state
            IsBusy = false;
        }
    }


    // Maps MealLookupDto (API DTO) into the Recipe model (ingredients, tags, steps).
    private static Recipe MapToRecipe(MealLookupDto dto)
    {
        // Basic properties
        var recipe = new Recipe
        {
            IdMeal = dto.idMeal ?? "",
            Name = dto.strMeal ?? "",
            Category = dto.strCategory,
            Area = dto.strArea,
            TagsRaw = dto.strTags,
            YoutubeUrl = dto.strYoutube,
            SourceUrl = dto.strSource,
            ImageUrl = dto.strMealThumb,
            Ingredients = new ObservableCollection<IngredientLine>(),
            Steps = new ObservableCollection<InstructionStep>()
        };

        // Tags: "Meat,Casserole" -> ["Meat","Casserole"]
        if (!string.IsNullOrWhiteSpace(dto.strTags))
        {
            foreach (var t in dto.strTags.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                recipe.Tags.Add(t);
        }

        // Ingredients/measures (1..20)
        var pairs = new (string? ing, string? meas)[]
        {
            (dto.strIngredient1, dto.strMeasure1),
            (dto.strIngredient2, dto.strMeasure2),
            (dto.strIngredient3, dto.strMeasure3),
            (dto.strIngredient4, dto.strMeasure4),
            (dto.strIngredient5, dto.strMeasure5),
            (dto.strIngredient6, dto.strMeasure6),
            (dto.strIngredient7, dto.strMeasure7),
            (dto.strIngredient8, dto.strMeasure8),
            (dto.strIngredient9, dto.strMeasure9),
            (dto.strIngredient10, dto.strMeasure10),
            (dto.strIngredient11, dto.strMeasure11),
            (dto.strIngredient12, dto.strMeasure12),
            (dto.strIngredient13, dto.strMeasure13),
            (dto.strIngredient14, dto.strMeasure14),
            (dto.strIngredient15, dto.strMeasure15),
            (dto.strIngredient16, dto.strMeasure16),
            (dto.strIngredient17, dto.strMeasure17),
            (dto.strIngredient18, dto.strMeasure18),
            (dto.strIngredient19, dto.strMeasure19),
            (dto.strIngredient20, dto.strMeasure20),
        };

        // Add non-empty ingredients
        foreach (var (ing, meas) in pairs)
        {
            var name = (ing ?? "").Trim();
            if (string.IsNullOrWhiteSpace(name))
                continue;
            // Add ingredient line, Name and Measure
            recipe.Ingredients.Add(new IngredientLine
            {
                Name = name,
                MeasureRaw = (meas ?? "").Trim()
            });
        }

        // Instructions -> steps
        var raw = (dto.strInstructions ?? "").Trim();

        if (string.IsNullOrWhiteSpace(raw))
        {
            // Fallback step if no instructions provided
            recipe.Steps.Add(new InstructionStep { Number = 1, Text = "No instructions provided.", IsDone = false });
            return recipe;
        }

        // Split instructions into lines, trimming and ignoring empty lines
        var lines = raw.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                       .Select(l => l.Trim())
                       .Where(l => !string.IsNullOrWhiteSpace(l))
                       .ToList();

        if (lines.Count == 0)
            lines = new List<string> { raw };

        // Add each line as an instruction step
        for (int i = 0; i < lines.Count; i++)
        {
            recipe.Steps.Add(new InstructionStep
            {
                Number = i + 1,
                Text = lines[i],
                IsDone = false
            });
        }

        return recipe;
    }


    // Reads persisted favourites state for the current recipe and updates IsFavorite.
    public async Task RefreshFavoriteFlagAsync()
    {
        if (Recipe?.IdMeal is null) return;
        IsFavorite = await _favorites.IsFavoriteAsync(Recipe.IdMeal);
    }


    // Toggles favourite state and persists it.
    // FavoritesRepository also caches the recipe locally when favourited.
    public async Task ToggleFavoriteAsync()
    {
        if (Recipe == null) return;

        var newValue = !IsFavorite;
        await _favorites.SetFavoriteAsync(Recipe, newValue);
        IsFavorite = newValue;
    }
}



