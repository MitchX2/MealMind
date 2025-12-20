using MealMind.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using static MealMind.Models.DTO.TheMealDbSearchDtos;


// TheMealDbClient
// Thin HTTP wrapper around TheMealDB API.
// Returns DTOs (API-shaped objects) so the rest of the app stays clean and testable.
//
// Note : DTO's are stored in MealMind/Models/DTO/

namespace MealMind.Utilities;
public sealed class TheMealDbClient
{
    // Normally I wouldnt include api's directly in code like this,
    // I'd use a gitignored config file but this is for you to be able to grade it easily.
    private const string BaseUrl = "https://www.themealdb.com/api/json/v2/65232507/";


    // Single HttpClient instance for the lifetime of this client.
    // Reusing HttpClient avoids socket exhaustion and is the recommended pattern.
    private readonly HttpClient _http = new()
    {
        BaseAddress = new Uri(BaseUrl)
    };

    // Search meals by name (search.php?s=...)
    public async Task<List<MealSummaryDto>> SearchByNameAsync(string name)
    {
        var url = $"search.php?s={Uri.EscapeDataString(name)}";
        var resp = await _http.GetFromJsonAsync<SearchByNameResponseDto>(url);
        return resp?.meals ?? new List<MealSummaryDto>();
    }

    // Filter meals by ingredient (filter.php?i=...)
    public async Task<List<MealFilterDto>> FilterByIngredientAsync(string ingredient)
    {
        var url = $"filter.php?i={Uri.EscapeDataString(ingredient)}";
        var resp = await _http.GetFromJsonAsync<FilterResponseDto>(url);
        return resp?.meals ?? new List<MealFilterDto>();
    }


    // Lookup full meal details by ID (lookup.php?i=...)
    public async Task<MealLookupDto?> LookupByIdAsync(string mealId)
    {
        var url = $"lookup.php?i={Uri.EscapeDataString(mealId)}";
        var resp = await _http.GetFromJsonAsync<MealLookupResponse>(url);
        return resp?.Meals?.FirstOrDefault();
    }

    // Filter meals by area (filter.php?a=...)
    public async Task<List<MealFilterDto>> FilterByAreaAsync(string area)
    {
        var url = $"filter.php?a={Uri.EscapeDataString(area)}";
        var resp = await _http.GetFromJsonAsync<FilterResponseDto>(url);
        return resp?.meals ?? new List<MealFilterDto>();
    }


    // Filter meals by category (filter.php?c=...)
    public async Task<List<MealFilterDto>> FilterByCategoryAsync(string category)
    {
        var url = $"filter.php?c={Uri.EscapeDataString(category)}";
        var resp = await _http.GetFromJsonAsync<FilterResponseDto>(url);
        return resp?.meals ?? new List<MealFilterDto>();
    }


    // Get a selection of random meals (randomselection.php)
    public async Task<List<MealLookupDto>> GetRandomSelectionAsync()
    {
        // randomselection.php is a Premium endpoint and documented under v1
        // tested here but it seems to only work with v2 (didnt get it working with the example form the site)
        var url = $"https://www.themealdb.com/api/json/v2/65232507/randomselection.php";

        var resp = await _http.GetFromJsonAsync<RandomSelectionResponseDto>(url);
        return resp?.Meals ?? new List<MealLookupDto>();
    }


    // Get a single random meal (random.php)
    // Used for MealOfTheDay currently as its not a managed application with user accounts.
    public async Task<MealLookupDto?> GetRandomMealAsync()
    {
        var url = "random.php";
        var resp = await _http.GetFromJsonAsync<MealLookupResponse>(url);
        return resp?.Meals?.FirstOrDefault();
    }

}
