using MealMind.Models;
using MealMind.Services.Storage;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MealMind.ViewModels;

public sealed class FavoritesPageViewModel : BindableObject
{
    private readonly FavoritesRepository _favorites = App.Services.Favorites;
    private readonly RecipeRepository _recipes = App.Services.Recipes;

    public ObservableCollection<SearchResultItem> Items { get; } = new();

    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        private set { _isBusy = value; OnPropertyChanged(); }
    }

    public ICommand OpenRecipeCommand { get; }

    // Test


    public FavoritesPageViewModel()
    {
        OpenRecipeCommand = new Command<string>(async (mealId) => await OpenRecipeAsync(mealId));
    }

    public async Task RefreshAsync()
    {
        IsBusy = true;
        try
        {
            Items.Clear();

            var state = await _favorites.GetAsync();

            foreach (var mealId in state.MealIds.Reverse())
            {
                if (string.IsNullOrWhiteSpace(mealId))
                    continue;

                var recipe = await _recipes.GetAsync(mealId);

                if (recipe == null)
                {
                    Items.Add(new SearchResultItem
                    {
                        MealId = mealId,
                        Title = "Saved recipe missing",
                        IngredientsPreview = "Could not load local file.",
                        MealImage = null
                    });
                    continue;
                }

                Items.Add(new SearchResultItem
                {
                    MealId = recipe.IdMeal,
                    Title = recipe.Name,
                    IngredientsPreview =
                        !string.IsNullOrWhiteSpace(recipe.Category) && !string.IsNullOrWhiteSpace(recipe.Area)
                            ? $"{recipe.Category} • {recipe.Area}"
                            : recipe.Category ?? recipe.Area ?? "",
                    MealImage = string.IsNullOrWhiteSpace(recipe.ImageUrl)
                        ? null
                        : new UriImageSource { Uri = new Uri(recipe.ImageUrl) }
                });
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    private static async Task OpenRecipeAsync(string? mealId)
    {
        if (string.IsNullOrWhiteSpace(mealId))
            return;

        await Shell.Current.GoToAsync($"{nameof(Views.Pages.RecipePage)}?mealId={mealId}");
    }
}