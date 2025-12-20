using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealMind.Models.Storage
{

    // FavoritesState
    // Represents the persisted favourites state stored in favorites.json.
    // Stores only meal IDs to keep the state file small and efficient.
    public sealed class FavoritesState
    {
        // Use a HashSet for fast lookups and to prevent duplicate favourites.
        public HashSet<string> MealIds { get; set; } = new();
    }
}
