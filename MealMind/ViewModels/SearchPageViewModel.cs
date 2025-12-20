using MealMind.Models;
using MealMind.Models.DTO;
using MealMind.Utilities;
using MealMind.Views.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MealMind.ViewModels;


// SearchPageViewModel
// ViewModel for SearchPage.
// Holds search/filter inputs, runs API queries via TheMealDbClient,
// and exposes a bindable Results list of SearchResultItem objects.
// Supports basic search, advanced filter search, and a random fallback when no input is provided.
public class SearchPageViewModel : BindableObject
{
    // Limits results to keep the UI fast and avoid large API lists.
    // Matches TheMealDB's own search result limits.
    private const int MaxResults = 10;

    // TheMealDB API client for searches.
    private readonly TheMealDbClient _client = new();

    // Bound to the results list UI (CollectionView). Items are lightweight summaries, not full Recipe objects.
    public ObservableCollection<SearchResultItem> Results { get; } = new();



    // Bindable search/filter inputs (entered by the user in the UI).
    private string _searchText = "";
    public string SearchText { get => _searchText; set { _searchText = value; OnPropertyChanged(); } }


    private string _ingredientsText = "";
    public string IngredientsText { get => _ingredientsText; set { _ingredientsText = value; OnPropertyChanged(); } }

    
    private string _areaText = "";
    public string AreaText { get => _areaText; set { _areaText = value; OnPropertyChanged(); } }

    
    private string _categoryText = "";
    public string CategoryText { get => _categoryText; set { _categoryText = value; OnPropertyChanged(); } }

    
    private bool _areFiltersOpen;
    public bool AreFiltersOpen { get => _areFiltersOpen; set { _areFiltersOpen = value; OnPropertyChanged(); } }



    // MealPlanner integration: target date for adding meal
    // set when coming from MealPlan, so selecting a result can assign it to a specific day. 
    private string? _targetDate;
    public string? TargetDate
    {
        get => _targetDate;
        set { _targetDate = value; OnPropertyChanged(); }
    }


    // Controls visibility of the filter/advanced search UI panels.
    private bool _isAdvancedOpen;
    public bool IsAdvancedOpen
    {
        get => _isAdvancedOpen;
        set { _isAdvancedOpen = value; OnPropertyChanged(); }
    }

    // Label showing the number of results found.
    public string ResultsCountLabel => $"Number of Results: {Results.Count}";


    // Commands bound from XAML (buttons/taps).
    public ICommand ToggleFiltersCommand { get; }
    public ICommand AdvancedSearchCommand { get; }
    public ICommand SearchCommand { get; }
    public ICommand OpenRecipeCommand { get; }


    // Wires up commands used by the SearchPage UI.
    public SearchPageViewModel()
    {
        ToggleFiltersCommand = new Command(() => AreFiltersOpen = !AreFiltersOpen);
        AdvancedSearchCommand = new Command(async () => await RunAdvancedSearchAsync());

        SearchCommand = new Command(async () => await RunSearchAsync());
        OpenRecipeCommand = new Command<string>(async (mealId) => await OpenRecipeAsync(mealId));
    }





    // Runs the main search.
    // Behaviour:
    // - If no inputs are provided, loads a random selection of 10 (default browse experience).
    // - Otherwise, runs name search and/or ingredient filter search and merges results.
    // TheMealDB API does not support combined queries
    private async Task RunSearchAsync()
    {
        // Clear previous results
        Results.Clear();

        // Trim inputs
        var name = SearchText?.Trim() ?? "";
        var ingredient = IngredientsText?.Trim() ?? "";

        var area = AreaText?.Trim() ?? "";
        var category = CategoryText?.Trim() ?? "";

        // No inputs -> load random selection
        if (string.IsNullOrWhiteSpace(name)
            && string.IsNullOrWhiteSpace(ingredient)
            && string.IsNullOrWhiteSpace(area)
            && string.IsNullOrWhiteSpace(category))
        {
            // No criteria -> show random recipes so the page isn't empty.
            await LoadRandomSelectionAsync();
            OnPropertyChanged(nameof(ResultsCountLabel));
            return;
        }

        try
        {
            // if name or ingredient provided, run those searches
            if (!string.IsNullOrWhiteSpace(name))
                await NameSearchAsync(name);

            else if (!string.IsNullOrWhiteSpace(ingredient))
                await IngredientSearchAsync(ingredient);
        }
        catch
        {
            // on error, clear results
            Results.Clear();
        }

        // Update results count label
        OnPropertyChanged(nameof(ResultsCountLabel));
    }


    // Search endpoint: returns meal summaries by name (search.php?s=...).
    private async Task NameSearchAsync(string name)
    {
        // TheMealDB API returns up to 10 results for name search,
        //  So we can just take them all in.
        var meals = await _client.SearchByNameAsync(name);

        // Go through results and add to Results collection.
        foreach (var m in meals.Take(MaxResults))
        {
            // Take in the meal ID
            var id = m.idMeal ?? "";
            // Skip if missing
            if (string.IsNullOrWhiteSpace(id)) continue;

            // Add to results
            Results.Add(new SearchResultItem
            {
                MealId = id,
                Title = m.strMeal ?? "",
                IngredientsPreview = "",
                MealImage = string.IsNullOrWhiteSpace(m.strMealThumb)
                    ? null
                    : new UriImageSource { Uri = new Uri(m.strMealThumb) }
            });
        }
    }

    // Filter endpoint: returns meals that include a given ingredient (filter.php?i=...).
    // Merges into Results, avoiding duplicates.
    private async Task IngredientSearchAsync(string ingredient)
    {
        var meals = await _client.FilterByIngredientAsync(ingredient);

        
        foreach (var m in meals)
        {
            if (Results.Count >= MaxResults) break;

            var id = m.idMeal ?? "";
            if (string.IsNullOrWhiteSpace(id)) continue;

            // Avoid duplicates if name and ingredient are both used
            if (Results.Any(r => r.MealId == id)) continue;

            Results.Add(new SearchResultItem
            {
                MealId = id,
                Title = m.strMeal ?? "",
                IngredientsPreview = $"Includes: {ingredient}",
                MealImage = string.IsNullOrWhiteSpace(m.strMealThumb)
                    ? null
                    : new UriImageSource { Uri = new Uri(m.strMealThumb) }
            });
        }
    }


    // Advanced search: runs one filter based on priority (Ingredient -> Area -> Category).
    // This keeps logic simple because TheMealDB doesn't support multi-filter intersections directly.
    private async Task RunAdvancedSearchAsync()
    {
        AreFiltersOpen = true; // keep panel open so user sees what's being used
        Results.Clear();

        // Priority: Ingredient -> Area -> Category
        var ing = IngredientsText?.Trim() ?? "";
        var area = AreaText?.Trim() ?? "";
        var cat = CategoryText?.Trim() ?? "";

        try
        {
            if (!string.IsNullOrWhiteSpace(ing))
            {
                await IngredientSearchAsync(ing);
            }
            else if (!string.IsNullOrWhiteSpace(area))
            {
                await AreaSearchAsync(area);
            }
            else if (!string.IsNullOrWhiteSpace(cat))
            {
                await CategorySearchAsync(cat);
            }
            // else: nothing filled -> do nothing
        }
        catch
        {
            Results.Clear();
        }

        OnPropertyChanged(nameof(ResultsCountLabel));
    }


    // Filter endpoint: returns meals by area (filter.php?a=...).
    private async Task AreaSearchAsync(string area)
    {
        var meals = await _client.FilterByAreaAsync(area);

        foreach (var m in meals.Take(MaxResults))
        {
            var id = m.idMeal ?? "";
            if (string.IsNullOrWhiteSpace(id)) continue;

            Results.Add(new SearchResultItem
            {
                MealId = id,
                Title = m.strMeal ?? "",
                IngredientsPreview = $"Area: {area}",
                MealImage = string.IsNullOrWhiteSpace(m.strMealThumb)
                    ? null
                    : new UriImageSource { Uri = new Uri(m.strMealThumb) }
            });
        }
    }

    // Filter endpoint: returns meals by category (filter.php?c=...).
    private async Task CategorySearchAsync(string category)
    {
        var meals = await _client.FilterByCategoryAsync(category);

        foreach (var m in meals.Take(MaxResults))
        {
            var id = m.idMeal ?? "";
            if (string.IsNullOrWhiteSpace(id)) continue;

            Results.Add(new SearchResultItem
            {
                MealId = id,
                Title = m.strMeal ?? "",
                IngredientsPreview = $"Category: {category}",
                MealImage = string.IsNullOrWhiteSpace(m.strMealThumb)
                    ? null
                    : new UriImageSource { Uri = new Uri(m.strMealThumb) }
            });
        }
    }


    // Default blank search results (10 random meals).
    // Used when the user searches with no criteria.
    private async Task LoadRandomSelectionAsync()
    {
        Results.Clear();

        var meals = await _client.GetRandomSelectionAsync();

        foreach (var m in meals.Take(MaxResults)) // MaxResults = 10
        {
            var id = m.idMeal ?? "";
            if (string.IsNullOrWhiteSpace(id)) continue;

            Results.Add(new SearchResultItem
            {
                MealId = id,
                Title = m.strMeal ?? "",
                IngredientsPreview = m.strCategory ?? "", // optional: show category or area here
                MealImage = string.IsNullOrWhiteSpace(m.strMealThumb)
                    ? null
                    : new UriImageSource { Uri = new Uri(m.strMealThumb) }
            });
        }
    }



    // Opens RecipePage for the selected meal.
    // If TargetDate is set, we pass it along so RecipePage can support "add to plan" behaviour.
    private async Task OpenRecipeAsync(string? mealId)
    {
        if (string.IsNullOrWhiteSpace(mealId))
            return;

        if (!string.IsNullOrWhiteSpace(TargetDate))
        {
            await Shell.Current.GoToAsync(
                $"{nameof(RecipePage)}?mealId={mealId}&targetDate={TargetDate}");
        }
        else
        {
            await Shell.Current.GoToAsync(
                $"{nameof(RecipePage)}?mealId={mealId}");
        }
    }
}


