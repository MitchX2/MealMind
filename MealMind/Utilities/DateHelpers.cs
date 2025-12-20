using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealMind.Utilities
{
    public static class DateHelpers
    {


        // Date has a DayOfWeek enum where Sunday = 0, Monday = 1, ..., Saturday = 6
        // Were using this to ensure our calendar weeks start on Monday
        public static DateTime StartOfWeekMonday(DateTime date)
        {
            // get only the date part
            date = date.Date;
            // Calculate difference to Monday
            int diff = (7 + (int)date.DayOfWeek - (int)DayOfWeek.Monday) % 7;
            // Subtract diff days to get back to Monday
            return date.AddDays(-diff);
        }

        public static List<DateTime> DaysInWeek(DateTime anyDayInWeek)
        {
            // Get the Monday of that week
            var start = StartOfWeekMonday(anyDayInWeek);
            // Generate the 7 days
            return Enumerable.Range(0, 7).Select(i => start.AddDays(i)).ToList();
        }
    }
}
