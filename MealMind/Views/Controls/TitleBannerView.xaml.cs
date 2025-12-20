namespace MealMind.Views.Controls;
// Renamed the name space to reflect the file structure

public partial class TitleBannerView : ContentView
{
	public TitleBannerView()
	{
		InitializeComponent();
	}

	private async void TitleBannerClicked(object sender, EventArgs e)
	{
		// 
		var state = Shell.Current.CurrentState;

		// Check if were on the MainPage Currently
		if (state.Location.OriginalString == "//home")
			// If we are do nothing
			return;
		else {
			// Otherwise go to home / MAinPage

			await Shell.Current.GoToAsync("//MainPage");
		}

		//return for now until the rest is made up

			return;
	}
}