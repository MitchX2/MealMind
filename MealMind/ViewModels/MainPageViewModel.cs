using MealMind.Models;
using MealMind.Services.Storage;
using MealMind.Utilities;
using MealMind.Views.Pages;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MealMind.ViewModels;


// MainPageViewModel
// Drives the MainPage UI (feature carousel + search accordion + navigation).
// Holds bindable state and ICommand actions for XAML.
// Loads dynamic feature cards (meal of the day + random favourite) on page appear.
// BindableObject to support OnPropertyChanged for XAML bindings.
public class MainPageViewModel : BindableObject
{
    
    // Feature carousel cards
    // are lightweight view models used by the UI (title, image, preview text, tap command).

    private double _carouselItemWidth;

    // Updated by the page when the screen size changes so carousel cards scale correctly.
    public double CarouselItemWidth
    {
        get => _carouselItemWidth;
        set { _carouselItemWidth = value; OnPropertyChanged(); }
    }


    // Data sources:
    //  TheMealDbClient: remote API (random meal)
    //  Repositories: local persistence (cached recipes + favourites state)
    private readonly TheMealDbClient _mealDb = new();

    private readonly LocalStoreService _store;
    private readonly RecipeRepository _recipes;
    private readonly FavoritesRepository _favorites;


    // Stores the selected meal IDs so the feature cards can navigate to RecipePage when tapped.
    private string? _mealOfDayId;
    private string? _randomFavouriteId;


    // Cards displayed in the feature carousel (bound to a CollectionView/CarouselView in XAML).
    public ObservableCollection<FeatureCardVm> FeatureCards { get; } = new();

    // Index of the currently visible feature card.
    //  Used to enable/disable left/right navigation buttons.
    private int _currentIndex;
    public int CurrentIndex
    {
        get => _currentIndex;
        set
        {
            if (_currentIndex == value) return;
            _currentIndex = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanGoLeft));
            OnPropertyChanged(nameof(CanGoRight));
        }
    }

    public bool CanGoLeft => CurrentIndex > 0;
    public bool CanGoRight => FeatureCards.Count > 0 && CurrentIndex < FeatureCards.Count - 1;


    // Set by MainPage.xaml.cs so the ViewModel can scroll the carousel via buttons.
    public Action<int>? RequestScrollToIndex { get; set; }


    // Commands bound from XAML (buttons/tap gestures).
    // Commands call private methods that perform navigation or async loading.
    public ICommand PrevFeatureCommand { get; }
    public ICommand NextFeatureCommand { get; }
    public ICommand OpenFeatureCommand { get; }

    // Existing page navigation 
    public ICommand GoToFavoritesCommand { get; }
    public ICommand GoToMealPlanCommand { get; }
    //public ICommand GoToPantryCommand { get; }
    public ICommand GoToShoppingListCommand { get; }

    // Search accordion state
    // Stores search inputs so they can be passed to SearchPage via Shell query parameters.

    private bool _isSearchExpanded;
    public bool IsSearchExpanded
    {
        get => _isSearchExpanded;
        set { _isSearchExpanded = value; OnPropertyChanged(); }
    }

    private string _searchText = "";
    public string SearchText
    {
        get => _searchText;
        set { _searchText = value; OnPropertyChanged(); }
    }

    private string _ingredientsText = "";
    public string IngredientsText
    {
        get => _ingredientsText;
        set { _ingredientsText = value; OnPropertyChanged(); }
    }

    public ICommand ToggleSearchExpandedCommand { get; }
    public ICommand SearchCommand { get; }
    public ICommand AdvancedSearchCommand { get; }



    // Sets up repositories and initializes commands.
    // Also creates placeholder feature cards so the UI has something to show immediately.
    public MainPageViewModel()
    {
        // Initialize storage and repos (NO DI)
        _store = new LocalStoreService();
        _recipes = new RecipeRepository(_store);
        _favorites = new FavoritesRepository(_store, _recipes);

        // Commands 
        ToggleSearchExpandedCommand = new Command(() => IsSearchExpanded = !IsSearchExpanded);

        SearchCommand = new Command(async () => await GoToSearchAsync(openAdvanced: false));
        AdvancedSearchCommand = new Command(async () => await GoToSearchAsync(openAdvanced: true));

        GoToFavoritesCommand = new Command(async () => await Shell.Current.GoToAsync(nameof(FavoritesPage)));
        GoToMealPlanCommand = new Command(async () => await Shell.Current.GoToAsync(nameof(MealPlanPage)));
        //GoToPantryCommand = new Command(async () => await Shell.Current.GoToAsync(nameof(PantryPage)));
        GoToShoppingListCommand = new Command(async () => await Shell.Current.GoToAsync(nameof(ShoppingListPage)));

        PrevFeatureCommand = new Command(() => GoToIndex(CurrentIndex - 1));
        NextFeatureCommand = new Command(() => GoToIndex(CurrentIndex + 1));
        OpenFeatureCommand = new Command(async (p) => await OpenFeatureAsync(p));

        // Placeholder cards
        FeatureCards.Clear();

        FeatureCards.Add(new FeatureCardVm
        {
            // Placeholder text avoids blank UI while async data loads in InitializeAsync().
            Key = "motd",
            BadgeText = "Meal of the Day",
            Title = "Meal of the Day",
            IngredientsPreview = "Finding something tasty…",
            ImageUrl = "",
            Image = null,
            TapCommand = OpenFeatureCommand,
            TapCommandParameter = "motd"
        });

        FeatureCards.Add(new FeatureCardVm
        {
            Key = "plan",
            BadgeText = "My Meal Plan",
            Title = "This Week",
            IngredientsPreview = "Tap to view and plan your meals.",
            ImageUrl = "",
            Image = null,
            TapCommand = OpenFeatureCommand,
            TapCommandParameter = "plan"
        });

        FeatureCards.Add(new FeatureCardVm
        {
            Key = "fav",
            BadgeText = "Random Favourite",
            Title = "Random Favourite",
            IngredientsPreview = "Favourite a recipe to see it here.",
            ImageUrl = "",
            Image = null,
            TapCommand = OpenFeatureCommand,
            TapCommandParameter = "fav"
        });

        CurrentIndex = 0;
    }

    // Called from MainPage.xaml.cs OnAppearing
    // To ensure navigation + UI are ready and async work doesn't block creation.
    public async Task InitializeAsync()
    {
        if (FeatureCards.Count == 0) return;

        CurrentIndex = Math.Clamp(CurrentIndex, 0, FeatureCards.Count - 1);
        RequestScrollToIndex?.Invoke(CurrentIndex);

        await LoadMealOfDayAsync();
        await LoadRandomFavouriteAsync();
    }


    // Moves the carousel selection safely (clamped so it never goes out of range).
    private void GoToIndex(int index)
    {
        if (FeatureCards.Count == 0) return;

        index = Math.Clamp(index, 0, FeatureCards.Count - 1);
        CurrentIndex = index;
        RequestScrollToIndex?.Invoke(CurrentIndex);
    }

    // Handles taps on feature cards and routes to the correct page.
    // Uses saved meal IDs when available, otherwise falls back to Search/Favourites pages.
    private async Task OpenFeatureAsync(object? param)
    {
        var key = param as string;
        if (string.IsNullOrWhiteSpace(key)) return;

        switch (key)
        {
            case "motd":
                if (!string.IsNullOrWhiteSpace(_mealOfDayId))
                {
                    await Shell.Current.GoToAsync(nameof(RecipePage),
                        new Dictionary<string, object> { ["mealId"] = _mealOfDayId });
                }
                else
                {
                    await Shell.Current.GoToAsync(nameof(SearchPage));
                }
                break;

            case "plan":
                await Shell.Current.GoToAsync(nameof(MealPlanPage));
                break;

            case "fav":
                if (!string.IsNullOrWhiteSpace(_randomFavouriteId))
                {
                    await Shell.Current.GoToAsync(nameof(RecipePage),
                        new Dictionary<string, object> { ["mealId"] = _randomFavouriteId });
                }
                else
                {
                    await Shell.Current.GoToAsync(nameof(FavoritesPage));
                }
                break;
        }
    }


    // Navigates to SearchPage and passes search text/ingredients as query parameters.

    private async Task GoToSearchAsync(bool openAdvanced)
    {
        // Build query parameters
        var qp = new Dictionary<string, object>();

        // Only include non-empty parameters
        if (!string.IsNullOrWhiteSpace(SearchText))
            qp["q"] = SearchText.Trim();

        if (!string.IsNullOrWhiteSpace(IngredientsText))
            qp["ing"] = IngredientsText.Trim();

        if (openAdvanced)
            qp["adv"] = "true";

        // Perform shell navigate + prams
        await Shell.Current.GoToAsync(nameof(SearchPage), qp);
    }


    // Loads the "Meal of the Day" card using a random API meal.
    // The recipe is cached locally so RecipePage can load it quickly/offline.

    private async Task LoadMealOfDayAsync()
    {
        var card = FeatureCards.First(c => c.Key == "motd");

        try
        {
            // Initial placeholder state
            card.Title = "Meal of the Day";
            card.IngredientsPreview = "Finding something tasty…";
            card.ImageUrl = "";
            card.Image = null;

            // Fetch random meal from API
            var dto = await _mealDb.GetRandomMealAsync();
            if (dto == null)
            {
                // API failed fallback
                card.IngredientsPreview = "Couldn’t load a meal right now. Tap to Search.";
                _mealOfDayId = null;
                return;
            }

            // Convert DTO to Recipe model and save locally
            Recipe recipe = dto.ToRecipe();

            // Save to local repo for offline access
            await _recipes.SaveAsync(recipe);

            // Update card with real data
            _mealOfDayId = recipe.IdMeal;
            card.Title = string.IsNullOrWhiteSpace(recipe.Name) ? "Meal of the Day" : recipe.Name;
            card.ImageUrl = recipe.ImageUrl ?? "";
            card.Image = CreateImageSource(recipe.ImageUrl);
            card.IngredientsPreview = BuildPreview(recipe);
        }
        catch
        {
            // Fallback to placeholder on error
            card.Title = "Meal of the Day";
            card.IngredientsPreview = "Couldn’t load a meal right now. Tap to Search.";
            card.ImageUrl = "";
            card.Image = null;
            _mealOfDayId = null;
        }
    }


    // Loads the "Meal of the Day" card using a random API meal.
    // The recipe is cached locally so RecipePage can load it quickly/offline.
    private async Task LoadRandomFavouriteAsync()
    {
        var card = FeatureCards.First(c => c.Key == "fav");

        try
        {
            // Initial placeholder state
            card.Title = "Random Favourite";
            card.IngredientsPreview = "Checking your favourites…";
            card.ImageUrl = "";
            card.Image = null;

            // Get list of favourite meal IDs
            var ids = await _favorites.GetMealIdsAsync();
            if (ids.Count == 0)
            {
                // If the user doesnt have any favourites yet - placeholder
                card.IngredientsPreview = "Favourite a recipe to see one here.";
                _randomFavouriteId = null;
                return;
            }

            // Pick a random favourite ID
            var id = ids[Random.Shared.Next(ids.Count)];
            _randomFavouriteId = id;
            // Load the full recipe from local repo
            var recipe = await _recipes.GetAsync(id);
            if (recipe == null)
            {
                // Recipe not found (shouldn't happen anymore) - show placeholder
                card.IngredientsPreview = "That favourite isn’t saved locally yet.";
                return;
            }

            // Update card with real data
            card.Title = string.IsNullOrWhiteSpace(recipe.Name) ? "Favourite Pick" : recipe.Name;
            card.ImageUrl = recipe.ImageUrl ?? "";
            card.Image = CreateImageSource(recipe.ImageUrl);
            card.IngredientsPreview = BuildPreview(recipe);
        }
        catch
        {
            // Fallback to placeholder on error
            card.Title = "Random Favourite";
            card.IngredientsPreview = "Couldn’t load favourites right now.";
            card.ImageUrl = "";
            card.Image = null;
            _randomFavouriteId = null;
        }
    }


    // Creates a short ingredient preview string for the carousel card (first few ingredients).
    private static string BuildPreview(Recipe recipe)
    {
        // No ingredients fallback
        if (recipe.Ingredients == null || recipe.Ingredients.Count == 0)
            return "Tap to view recipe.";

        // Build preview from first few ingredient names
        var names = recipe.Ingredients
            .Where(x => !string.IsNullOrWhiteSpace(x.Name))
            .Select(x => x.Name.Trim())
            .Take(4);

        // Join names with commas and return string
        var s = string.Join(", ", names);
        return string.IsNullOrWhiteSpace(s) ? "Tap to view recipe." : s;
    }


    // Converts an image URL into an ImageSource for XAML binding (with caching enabled).
    private static ImageSource? CreateImageSource(string? url)
    {
        // Null/empty URL check
        if (string.IsNullOrWhiteSpace(url)) return null;

        // Create UriImageSource with caching
        if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
            return new UriImageSource { Uri = uri, CachingEnabled = true };

        return null;
    }
}
