using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

using MealMind.Resources.Styles;

namespace MealMind.Utilities
{
    public static class ThemeManager
    {
        private const string AppPrefKey = "AppTheme";
        // App.xaml: 0 = Styles.xaml, 1 = active theme dictionary

        public static string GetSavedTheme() =>
            // Get the saved theme if theres no saved theme pick Default
            Preferences.Default.Get(AppPrefKey, "Default");

        public static void SaveTheme(string theme) =>
            Preferences.Default.Set(AppPrefKey, theme);

        public static void ApplySavedTheme() =>
            ApplyTheme(GetSavedTheme());

        
        public static void ApplyTheme(string theme)
        {
            var merged = Application.Current!.Resources.MergedDictionaries;

            RemoveThemes(merged);

            merged.Add(theme switch
            {
                "Dark" => new ColorsDark(),
                "Light" => new ColorsLight(),
                "Default" => new ColorsDefault(),
                _ => new ColorsDefault()
            });
        }


        private static void RemoveThemes(ICollection<ResourceDictionary> merged)
        {
            // snapshot to avoid modifying while iterating
            var toRemove = merged
                .Where(d => d is ColorsDark || d is ColorsLight || d is ColorsDefault)
                .ToList();

            foreach (var d in toRemove)
                merged.Remove(d);
        }
    }
}
