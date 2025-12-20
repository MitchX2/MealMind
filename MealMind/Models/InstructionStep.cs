using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MealMind.Models
{

    // InstructionStep
    // Represents a single instruction step in a recipe.
    // Used for displaying numbered steps and optional checkbox progress while cooking.
    public sealed class InstructionStep
    {
        // Step number for displaying  step order
        public int Number { get; init; }
        public string Text { get; init; } = "";

        // Bound directly to the checkbox (page-session only)
        public bool IsDone { get; set; }
    }
}
