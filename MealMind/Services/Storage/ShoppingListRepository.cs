using MealMind.Models;
using MealMind.Models.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealMind.Services.Storage
{

    // ShoppingListRepository
    // Stores the user's shopping list as app state in state/shoppinglist.json.
    // ViewModels use this repository instead of dealing with JSON/files directly.
    // Includes merging duplicate ingredients and sorting items.
    public sealed class ShoppingListRepository
    {
        // Stored as a single JSON file in the state folder.
        private const string FileName = "shoppinglist.json";
        private readonly LocalStoreService _store;


        public ShoppingListRepository(LocalStoreService store) => _store = store;

        private string Path => _store.StatePath(FileName);


        // Loads the shopping list from disk.
        // If no file exists yet, return a new empty list.
        public async Task<ShoppingListState> GetAsync()
            => await _store.ReadJsonAsync<ShoppingListState>(Path) ?? new ShoppingListState();


        // Persists the entire shopping list using an atomic write (prevents corrupted files).
        public async Task SaveAsync(ShoppingListState state)
            => await _store.WriteJsonAtomicAsync(Path, state);


        // Adds ingredients from a recipe into the shopping list.
        // Merges items by name (case-insensitive) so we don't get duplicates like "Onion" vs "onion".
        // Quantity is kept as text because ingredient measures are not always parseable.
        public async Task AddFromRecipeAsync(Recipe recipe)
        {
            var state = await GetAsync();

            foreach (var ing in recipe.Ingredients)
            {


                var name = (ing.Name ?? "").Trim();

                // Continue to ignore blank ingredient lines from the API.
                if (string.IsNullOrWhiteSpace(name)) continue;

                var qty = (ing.MeasureRaw ?? "").Trim();

                // Merge by case-insensitive name
                var existing = state.Items.FirstOrDefault(i =>
                    string.Equals(i.Name, name, StringComparison.OrdinalIgnoreCase));

                if (existing == null)
                {
                    state.Items.Add(new ShoppingItem
                    {
                        Name = name,
                        QuantityText = qty,
                        IsChecked = false
                    });
                }
                else
                {
                    // Simple merge: keep as text append quantity text to avoid clashing units (cups, tsp, grams, etc.).
                    // Should be upgraded in the future to parse and sum quantities properly.
                    if (string.IsNullOrWhiteSpace(existing.QuantityText))
                    {
                        existing.QuantityText = qty;
                    }
                    else if (!string.IsNullOrWhiteSpace(qty) &&
                             existing.QuantityText.IndexOf(qty, StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        existing.QuantityText = $"{existing.QuantityText} + {qty}";
                    }
                }
            }

            // Keep UI tidy: unchecked items first, then alphabetical.
            state.Items = state.Items
                .OrderBy(i => i.IsChecked)
                .ThenBy(i => i.Name)
                .ToList();

            await SaveAsync(state);
        }


        // Updates the "bought" checkbox for an item.
        // Uses name matching (case-insensitive) as the simple key for list items.
        public async Task SetCheckedAsync(string name, bool isChecked)
        {
            var state = await GetAsync();

            var item = state.Items.FirstOrDefault(i =>
                string.Equals(i.Name, name, StringComparison.OrdinalIgnoreCase));

            if (item == null) return;

            item.IsChecked = isChecked;
            await SaveAsync(state);
        }


        // Removes an item by name (case-insensitive match).
        public async Task RemoveAsync(string name)
        {
            var state = await GetAsync();
            state.Items.RemoveAll(i => string.Equals(i.Name, name, StringComparison.OrdinalIgnoreCase));
            await SaveAsync(state);
        }


        // Removes items the user has already checked off.
        public async Task ClearCheckedAsync()
        {
            var state = await GetAsync();
            state.Items.RemoveAll(i => i.IsChecked);
            await SaveAsync(state);
        }


        // Clears the entire shopping list.
        public async Task ClearAllAsync()
        {
            await SaveAsync(new ShoppingListState());
        }
    }
}

