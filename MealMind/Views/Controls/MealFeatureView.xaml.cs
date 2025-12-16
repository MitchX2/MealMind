using System.Windows.Input;

namespace MealMind.Views.Controls;

public partial class MealFeatureView : ContentView
{
    private readonly SwipeGestureRecognizer _swipeLeft;
    private readonly SwipeGestureRecognizer _swipeRight;

    public MealFeatureView()
    {
        InitializeComponent();

        _swipeLeft = new SwipeGestureRecognizer { Direction = SwipeDirection.Left };
        _swipeRight = new SwipeGestureRecognizer { Direction = SwipeDirection.Right };

        _swipeLeft.SetBinding(SwipeGestureRecognizer.CommandProperty, new Binding(nameof(RightCommand), source: this));
        _swipeRight.SetBinding(SwipeGestureRecognizer.CommandProperty, new Binding(nameof(LeftCommand), source: this));

        UpdateSwipeRecognizers();
    }

    // ===== Display properties =====

    public static readonly BindableProperty BadgeTextProperty =
        BindableProperty.Create(nameof(BadgeText), typeof(string), typeof(MealFeatureView), "Meal of the Day");

    public string BadgeText
    {
        get => (string)GetValue(BadgeTextProperty);
        set => SetValue(BadgeTextProperty, value);
    }

    public static readonly BindableProperty ShowBadgeProperty =
        BindableProperty.Create(nameof(ShowBadge), typeof(bool), typeof(MealFeatureView), true);

    public bool ShowBadge
    {
        get => (bool)GetValue(ShowBadgeProperty);
        set => SetValue(ShowBadgeProperty, value);
    }

    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(MealFeatureView), "");

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly BindableProperty IngredientsPreviewProperty =
        BindableProperty.Create(nameof(IngredientsPreview), typeof(string), typeof(MealFeatureView), "");

    public string IngredientsPreview
    {
        get => (string)GetValue(IngredientsPreviewProperty);
        set => SetValue(IngredientsPreviewProperty, value);
    }

    public static readonly BindableProperty MealImageProperty =
        BindableProperty.Create(nameof(MealImage), typeof(ImageSource), typeof(MealFeatureView), default(ImageSource));

    public ImageSource? MealImage
    {
        get => (ImageSource?)GetValue(MealImageProperty);
        set => SetValue(MealImageProperty, value);
    }

    // ===== Nav buttons =====

    public static readonly BindableProperty ShowNavButtonsProperty =
        BindableProperty.Create(nameof(ShowNavButtons), typeof(bool), typeof(MealFeatureView), false);

    public bool ShowNavButtons
    {
        get => (bool)GetValue(ShowNavButtonsProperty);
        set => SetValue(ShowNavButtonsProperty, value);
    }

    public static readonly BindableProperty IsLeftEnabledProperty =
        BindableProperty.Create(nameof(IsLeftEnabled), typeof(bool), typeof(MealFeatureView), true);

    public bool IsLeftEnabled
    {
        get => (bool)GetValue(IsLeftEnabledProperty);
        set => SetValue(IsLeftEnabledProperty, value);
    }

    public static readonly BindableProperty IsRightEnabledProperty =
        BindableProperty.Create(nameof(IsRightEnabled), typeof(bool), typeof(MealFeatureView), true);

    public bool IsRightEnabled
    {
        get => (bool)GetValue(IsRightEnabledProperty);
        set => SetValue(IsRightEnabledProperty, value);
    }

    // ===== Swipe =====

    public static readonly BindableProperty EnableSwipeProperty =
        BindableProperty.Create(
            nameof(EnableSwipe),
            typeof(bool),
            typeof(MealFeatureView),
            true,
            propertyChanged: (b, o, n) => ((MealFeatureView)b).UpdateSwipeRecognizers());

    public bool EnableSwipe
    {
        get => (bool)GetValue(EnableSwipeProperty);
        set => SetValue(EnableSwipeProperty, value);
    }

    // ===== Commands =====

    public static readonly BindableProperty LeftCommandProperty =
        BindableProperty.Create(nameof(LeftCommand), typeof(ICommand), typeof(MealFeatureView));

    public ICommand? LeftCommand
    {
        get => (ICommand?)GetValue(LeftCommandProperty);
        set => SetValue(LeftCommandProperty, value);
    }

    public static readonly BindableProperty RightCommandProperty =
        BindableProperty.Create(nameof(RightCommand), typeof(ICommand), typeof(MealFeatureView));

    public ICommand? RightCommand
    {
        get => (ICommand?)GetValue(RightCommandProperty);
        set => SetValue(RightCommandProperty, value);
    }

    public static readonly BindableProperty TapCommandProperty =
        BindableProperty.Create(nameof(TapCommand), typeof(ICommand), typeof(MealFeatureView));

    public ICommand? TapCommand
    {
        get => (ICommand?)GetValue(TapCommandProperty);
        set => SetValue(TapCommandProperty, value);
    }

    public static readonly BindableProperty TapCommandParameterProperty =
        BindableProperty.Create(nameof(TapCommandParameter), typeof(object), typeof(MealFeatureView));

    public object? TapCommandParameter
    {
        get => GetValue(TapCommandParameterProperty);
        set => SetValue(TapCommandParameterProperty, value);
    }

    private void UpdateSwipeRecognizers()
    {
        GestureRecognizers.Remove(_swipeLeft);
        GestureRecognizers.Remove(_swipeRight);

        if (EnableSwipe)
        {
            GestureRecognizers.Add(_swipeLeft);
            GestureRecognizers.Add(_swipeRight);
        }
    }
}

