using MealMind.Services.Storage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealMind.ViewModels
{

    // ShoppingListPageViewModel
    // ViewModel for the Shopping List page.
    // Loads shopping list state from ShoppingListRepository and exposes a bindable Items collection.
    // Handles user actions like checking items, deleting items, and clearing the list.
    public sealed class ShoppingListPageViewModel : BindableObject
    {
        // ShoppingListRepository handles persistence of the shopping list state.
        private readonly ShoppingListRepository _repo = App.Services.ShoppingList;

        // Bindable collection of shopping list items for the UI.
        public ObservableCollection<ShoppingListItemRow> Items { get; } = new();


        // Used by the UI to show a loading indicator and prevent actions while loading.
        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            private set { _isBusy = value; OnPropertyChanged(); }
        }


        // Reloads saved shopping list state from disk and rebuilds the UI collection.
        public async Task RefreshAsync()
        {
            IsBusy = true;
            try
            {
                // Clear existing items.
                Items.Clear();

                // Load saved state from repository.
                var state = await _repo.GetAsync();

                foreach (var i in state.Items)
                {
                    // Add each item to the bindable collection.
                    Items.Add(new ShoppingListItemRow
                    {
                        Name = i.Name,
                        QuantityText = i.QuantityText,
                        IsChecked = i.IsChecked
                    });
                }
            }
            finally
            {
                // Always clear the busy flag.
                IsBusy = false;
            }
        }


        // Updates the checked state in the UI immediately and persists the change to storage.
        public async Task SetCheckedAsync(ShoppingListItemRow row, bool isChecked)
        {
            row.IsChecked = isChecked;
            await _repo.SetCheckedAsync(row.Name, isChecked);
        }

        // Deletes an item from storage and removes it from the UI list.
        public async Task DeleteAsync(ShoppingListItemRow row)
        {
            await _repo.RemoveAsync(row.Name);
            Items.Remove(row);
        }

        // Clears all checked items from storage and refreshes the UI list.
        public async Task ClearCheckedAsync()
        {
            await _repo.ClearCheckedAsync();
            await RefreshAsync();
        }

        // Clears all items from storage and refreshes the UI list.
        public async Task ClearAllAsync()
        {
            await _repo.ClearAllAsync();
            await RefreshAsync();
        }
    }
}
