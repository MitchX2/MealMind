using MealMind.Utilities;

namespace MealMind
{
    public partial class App : Application
    {
        public App()
        {
            //// Added for debugging purposes
            //AppDomain.CurrentDomain.FirstChanceException += (s, e) =>
            //System.Diagnostics.Debug.WriteLine("FIRST CHANCE: " + e.Exception);
            
            
            InitializeComponent();
            // Apply the saved theme on startup
            MealMind.Utilities.ThemeManager.ApplySavedTheme();
            
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }


    }
}