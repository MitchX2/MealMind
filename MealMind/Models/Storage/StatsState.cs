using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealMind.Models.Storage
{
    // if we want to add filters for mostCooked or recomendations based on cooking stats
    // we can expand this class for other stats later
    public sealed class StatsState
    {
        // MealId -> times cooked
        public Dictionary<string, int> TimesCooked { get; set; } = new();
    }
}
