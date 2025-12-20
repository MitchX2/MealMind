using MealMind.ViewModels;

namespace MealMind;

public partial class MainPage : ContentPage
{
    private bool _sizeHooked;

    public MainPage()
    {
        InitializeComponent();

        // REQUIRED: x:DataType does NOT create the ViewModel
        BindingContext = new MainPageViewModel();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is MainPageViewModel vm)
        {
            // Allow VM to request scrolling
            vm.RequestScrollToIndex = i =>
                FeatureCarousel.ScrollTo(i, position: ScrollToPosition.Center, animate: true);

            // Ensure carousel items are full width (1 card at a time)
            if (!_sizeHooked)
            {
                _sizeHooked = true;

                FeatureCarousel.SizeChanged += (_, __) =>
                {
                    if (FeatureCarousel.Width > 0)
                        vm.CarouselItemWidth = FeatureCarousel.Width;
                };
            }

            // Initial width (SizeChanged will correct if needed)
            vm.CarouselItemWidth = FeatureCarousel.Width;

            await vm.InitializeAsync();
        }
    }
}
