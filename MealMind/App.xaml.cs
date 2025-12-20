using MealMind.Services;
using MealMind.Utilities;


namespace MealMind
{
    public partial class App : Application
    {
        // Setting up for Storage and Indexing services
        // All using one service instance for app lifetime
        public static AppServices Services { get; } = new();
        
        
        public App()
        {
            //// Added for debugging purposes
            //AppDomain.CurrentDomain.FirstChanceException += (s, e) =>
            //System.Diagnostics.Debug.WriteLine("FIRST CHANCE: " + e.Exception);
            
            
            InitializeComponent();



            // Debugging ResourceDictionary loading issues
            var merged = Current!.Resources.MergedDictionaries;

            System.Diagnostics.Debug.WriteLine($"MergedDictionaries count: {merged.Count}");

            int i = 0;
            foreach (var d in merged)
            {
                System.Diagnostics.Debug.WriteLine($"  [{i}] type={d.GetType().FullName}");

                if (d.TryGetValue("__ColorsDefaultLoaded", out var v))
                    System.Diagnostics.Debug.WriteLine($"    -> FOUND __ColorsDefaultLoaded in [{i}] value={v}");

                i++;
            }

            var foundApp = Current!.Resources.TryGetValue("__ColorsDefaultLoaded", out var appVal);
            System.Diagnostics.Debug.WriteLine($"App resources has key? {foundApp} val={appVal}");

            var hasBg = Current!.Resources.TryGetValue("AppBackground", out var bg);
            System.Diagnostics.Debug.WriteLine($"Has AppBackground? {hasBg} type={bg?.GetType().FullName} value={bg}");


            static void DumpHasKey(ResourceDictionary d, string key, int i)
            {
                if (d.TryGetValue(key, out var v))
                    System.Diagnostics.Debug.WriteLine($"  [{i}] HAS '{key}' type={v?.GetType().FullName} value={v}");
            }

            i = 0;
            foreach (var d in Current!.Resources.MergedDictionaries)
            {
                DumpHasKey(d, "Primary", i);          // likely in Colors.xaml (template)
                DumpHasKey(d, "Gray100", i);          // template colors often have this
                DumpHasKey(d, "AppBackground", i);    // your ColorsDefault
                DumpHasKey(d, "PrimaryText", i);      // your ColorsDefault
                DumpHasKey(d, "BodyStyle", i);        // likely in Styles.xaml (template)
                DumpHasKey(d, "__ColorsDefaultLoaded", i); // your test flag
                i++;
            }

            static void DumpFirstKeys(ResourceDictionary d, int i, int max = 25)
            {
                System.Diagnostics.Debug.WriteLine($"--- Keys in [{i}] ---");

                int c = 0;
                foreach (var key in d.Keys)
                {
                    System.Diagnostics.Debug.WriteLine($"  {key}");
                    if (++c >= max) break;
                }

                if (c == 0)
                    System.Diagnostics.Debug.WriteLine("  (no keys)");
            }

            i = 0;
            foreach (var d in Current!.Resources.MergedDictionaries)
            {
                DumpFirstKeys(d, i);
                i++;
            }


            ApplyStartupSettings();

            //var found = Current!.Resources.TryGetValue("__ColorsDefaultLoaded", out var v);
            //System.Diagnostics.Debug.WriteLine($"ColorsDefaultLoaded found? {found} value={v}");

            // Apply theme on startup
            var settings = Services.Settings.Load();
            Services.Settings.Apply(settings);

            // Re-apply theme if system Light/Dark changes while app is running
            Application.Current!.RequestedThemeChanged += (_, __) =>
            {
                var s = Services.Settings.Load();
                Services.Settings.Apply(s);
            };



        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }

        
        private static void ApplyStartupSettings()
        {
            // Load + Apply settings
            var s = Services.Settings.Load();
            Services.Settings.Apply(s);

        }


    }


}


// Apply the saved theme on startup
//MealMind.Utilities.ThemeManager.ApplySavedTheme();


/* Kept for reference/rollback
         * 
        private static void ApplyAccessibilitySettings()
        {
            var s = Services.Settings.Load();

            Application.Current!.UserAppTheme = s.Theme switch
            {
                Models.Settings.AppThemePreference.Light => AppTheme.Light,
                Models.Settings.AppThemePreference.Dark => AppTheme.Dark,
                _ => AppTheme.Unspecified
            };

            // Accessibility overlay background
            Application.Current.Resources["accElementBackground"] =
                s.HighContrastTouchZones
                    ? Color.FromArgb("#33FFFFFF")
                    : Colors.Transparent;
        }
        */

// Kept for reference/rollback
//ThemeManager.ApplySavedTheme();
//ApplyAccessibilitySettings();