using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealMind.Models.Settings
{
    // Enum for users theme preference
    public enum AppThemePreference
    {
        System = 0,
        Light = 1,
        Dark = 2
    }

    // AppSettings
    // Simple data model that represents all user-configurable settings in the app.
    // Loaded and saved by SettingsService using MAUI Preferences.
    public sealed class AppSettings
    {
        public AppThemePreference Theme { get; set; } = AppThemePreference.System;

        // Accessibility: makes nav swipe overlay buttons visible (uses your accElementBackground key)
        public bool HighContrastTouchZones { get; set; } = false;

        // Default for recipes (user can still tick per recipe; a just default show/hide)
        public bool ShowInstructionCheckboxes { get; set; } = true;
    }
}
