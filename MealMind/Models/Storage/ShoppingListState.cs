using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealMind.Models.Storage
{

    // ShoppingListState
    // Represents the persisted shopping list stored in shoppinglist.json.
    // Managed by ShoppingListRepository.
    public sealed class ShoppingListState
    {
        // List of shopping list items in display order.
        public List<ShoppingItem> Items { get; set; } = new();
    }

    // Represents a single shopping list entry.
    public sealed class ShoppingItem
    {
        public string Name { get; set; } = "";
        // kept as text for simplicity ("2", "500g", "1 pack")
        public string? QuantityText { get; set; }
        // Tracks whether the item has been marked as bought.
        public bool IsChecked { get; set; }
    }
}
