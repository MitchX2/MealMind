
using MealMind.Models;
using System.Diagnostics;
using MealMind.Models.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MealMind.Services.Settings
{

    // SettingsService
    // Loads/saves user settings using MAUI Preferences and applies them to the running app.
    // Handles theme selection and simple accessibility options via ResourceDictionary updates.
    public sealed class SettingsService
    {
        // Preference keys stored in MAUI Preferences (persistent key/value storage).
        // Constants ot prevent typos and keep settings names consistent across the app.
        private const string ThemeKey = "settings_theme";
        private const string HighContrastKey = "settings_high_contrast_touch_zones";
        private const string ShowInstructionChecksKey = "settings_show_instruction_checks";


        // Reads saved settings from Preferences.
        // Defaults are used on first launch if no value has been saved yet.
        public AppSettings Load()
        {
            return new AppSettings
            {
                Theme = (AppThemePreference)Preferences.Get(ThemeKey, (int)AppThemePreference.System),
                HighContrastTouchZones = Preferences.Get(HighContrastKey, false),
                ShowInstructionCheckboxes = Preferences.Get(ShowInstructionChecksKey, true)
            };
        }


        // Persists the current settings to Preferences.
        public void Save(AppSettings s)
        {
            Preferences.Set(ThemeKey, (int)s.Theme);
            Preferences.Set(HighContrastKey, s.HighContrastTouchZones);
            Preferences.Set(ShowInstructionChecksKey, s.ShowInstructionCheckboxes);
        }


        // Applies settings immediately while the app is running.
        // Updates the active theme ResourceDictionary and any accessibility resources.
        public void Apply(AppSettings s)
        {
            // Theme
            if (Application.Current != null)
            {
                // Apply our chosen Theme
                ApplyThemeResources(s.Theme);

                // Accessibility overlay background used by MealFeatureView 
                // larger/high-contrast touch zones when enabled;
                Application.Current.Resources["accElementBackground"] =
                    s.HighContrastTouchZones
                        ? Color.FromArgb("#33FFFFFF")
                        : Colors.Transparent;
            }
        }


        // Save to disk and update the current UI in one call.
        public void SaveAndApply(AppSettings s)
        {
            Save(s);
            Apply(s);
        }


        // Applies theme resources by swapping the active ColorsLight/ColorsDark ResourceDictionary.
        // We remove any previous theme dictionary first to avoid duplicate keys and resource conflicts.
        private static void ApplyThemeResources(AppThemePreference pref)
        {
            if (Application.Current == null)
                return;

            // 1) Tell MAUI which app theme mode we're in
            Application.Current.UserAppTheme = pref switch
            {
                AppThemePreference.Light => AppTheme.Light,
                AppThemePreference.Dark => AppTheme.Dark,
                _ => AppTheme.Unspecified // System
            };

            // 2) Select our app colour dictionary
            ResourceDictionary dict = pref switch
            {
                AppThemePreference.Dark => new MealMind.Resources.Styles.ColorsDark(),
                AppThemePreference.Light => new MealMind.Resources.Styles.ColorsLight(),
                _ => new MealMind.Resources.Styles.ColorsDefault() // System → Default
            };

            // 3) Remove any previously applied theme dictionaries
            var merged = Application.Current.Resources.MergedDictionaries;

            var toRemove = merged
                .Where(md => md is MealMind.Resources.Styles.ColorsLight
                          || md is MealMind.Resources.Styles.ColorsDark
                          || md is MealMind.Resources.Styles.ColorsDefault)
                .ToList();

            foreach (var md in toRemove)
                merged.Remove(md);

            // 4) Apply the selected theme
            merged.Add(dict);
        }
    }
}
