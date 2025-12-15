using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealMind.Models
{
    public class InstructionStep
    {
        public int Number { get; init; }            
        public string Text { get; init; } = "";

        // IsDone added to track progress through recipe steps
        public bool IsDone { get; set; }            
    }
}
