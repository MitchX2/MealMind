using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace MealMind.ViewModels
{

    // ShoppingListItemRow
    // Lightweight bindable row model for the Shopping List UI.
    // Represents a single item with quantity text and checked state.
    public sealed class ShoppingListItemRow : BindableObject
    {
        // Item name (init used as its the stable identifier for persistence).
        public string Name { get; init; } = "";

        // Quantity text displayed in the UI (kept flexible, e.g. "2", "500g", "1 pack").
        private string? _quantityText;
        public string? QuantityText
        {
            get => _quantityText;
            set { _quantityText = value; OnPropertyChanged(); }
        }


        // Bound to the checkbox in the UI to mark an item as bought.
        private bool _isChecked;
        public bool IsChecked
        {
            get => _isChecked;
            set { _isChecked = value; OnPropertyChanged(); }
        }
    }
}
