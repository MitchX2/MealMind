using System.Windows.Input;

namespace MealMind.Views.Controls;

public partial class SearchAccordionView : ContentView
{
    public SearchAccordionView()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty SearchTextProperty =
    BindableProperty.Create(
        nameof(SearchText),
        typeof(string),
        typeof(SearchAccordionView),
        "",
        defaultBindingMode: BindingMode.TwoWay);

    public string SearchText
    {
        get => (string)GetValue(SearchTextProperty);
        set => SetValue(SearchTextProperty, value);
    }

    public static readonly BindableProperty IngredientsTextProperty =
    BindableProperty.Create(
        nameof(IngredientsText),
        typeof(string),
        typeof(SearchAccordionView),
        "",
        defaultBindingMode: BindingMode.TwoWay);

    public string IngredientsText
    {
        get => (string)GetValue(IngredientsTextProperty);
        set => SetValue(IngredientsTextProperty, value);
    }

    public static readonly BindableProperty IsExpandedProperty =
        BindableProperty.Create(nameof(IsExpanded), typeof(bool), typeof(SearchAccordionView), false);

    public bool IsExpanded
    {
        get => (bool)GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    public static readonly BindableProperty ToggleExpandedCommandProperty =
        BindableProperty.Create(nameof(ToggleExpandedCommand), typeof(ICommand), typeof(SearchAccordionView));

    public ICommand? ToggleExpandedCommand
    {
        get => (ICommand?)GetValue(ToggleExpandedCommandProperty);
        set => SetValue(ToggleExpandedCommandProperty, value);
    }

    public static readonly BindableProperty SearchCommandProperty =
        BindableProperty.Create(nameof(SearchCommand), typeof(ICommand), typeof(SearchAccordionView));

    public ICommand? SearchCommand
    {
        get => (ICommand?)GetValue(SearchCommandProperty);
        set => SetValue(SearchCommandProperty, value);
    }

    public static readonly BindableProperty AdvancedSearchCommandProperty =
        BindableProperty.Create(nameof(AdvancedSearchCommand), typeof(ICommand), typeof(SearchAccordionView));

    public ICommand? AdvancedSearchCommand
    {
        get => (ICommand?)GetValue(AdvancedSearchCommandProperty);
        set => SetValue(AdvancedSearchCommandProperty, value);
    }
}