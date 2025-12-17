using System.Collections.ObjectModel;

namespace MealMind.Views.Controls;

public partial class IngredientListView : ContentView
{
    public IngredientListView()
    {
        InitializeComponent();
        BindingContext = this;
        UpdateEmptyFlags();
    }

    // ===== Title =====

    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), 
            typeof(IngredientListView), "Ingredients");

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly BindableProperty ShowTitleProperty =
        BindableProperty.Create(nameof(ShowTitle), typeof(bool), typeof(IngredientListView), true);

    public bool ShowTitle
    {
        get => (bool)GetValue(ShowTitleProperty);
        set => SetValue(ShowTitleProperty, value);
    }

    // ===== Empty State =====

    public static readonly BindableProperty EmptyTextProperty =
        BindableProperty.Create(nameof(EmptyText), typeof(string), typeof(IngredientListView), "No ingredients available.");

    public string EmptyText
    {
        get => (string)GetValue(EmptyTextProperty);
        set => SetValue(EmptyTextProperty, value);
    }

    // ===== Items =====

    public static readonly BindableProperty ItemsProperty =
        BindableProperty.Create(
            nameof(Items),
            typeof(ObservableCollection<IngredientRow>),
            typeof(IngredientListView),
            new ObservableCollection<IngredientRow>(),
            propertyChanged: (b, o, n) => ((IngredientListView)b).UpdateEmptyFlags());

    public ObservableCollection<IngredientRow> Items
    {
        get => (ObservableCollection<IngredientRow>)GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    // Flags for XAML
    public static readonly BindableProperty IsEmptyProperty =
        BindableProperty.Create(nameof(IsEmpty), typeof(bool), typeof(IngredientListView), true);

    public bool IsEmpty
    {
        get => (bool)GetValue(IsEmptyProperty);
        private set => SetValue(IsEmptyProperty, value);
    }

    public static readonly BindableProperty IsNotEmptyProperty =
        BindableProperty.Create(nameof(IsNotEmpty), typeof(bool), typeof(IngredientListView), false);

    public bool IsNotEmpty
    {
        get => (bool)GetValue(IsNotEmptyProperty);
        private set => SetValue(IsNotEmptyProperty, value);
    }

    private void UpdateEmptyFlags()
    {
        var hasItems = Items != null && Items.Count > 0;
        IsEmpty = !hasItems;
        IsNotEmpty = hasItems;
    }
}

// Simple display row model (can be replaced later with your Recipe model)
public class IngredientRow
{
    public string Name { get; set; } = "";
    public string Measure { get; set; } = "";
}