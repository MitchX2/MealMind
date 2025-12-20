using MealMind.Models;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace MealMind.Views.Controls;

public partial class IngredientListView : ContentView
{
    public IngredientListView()
    {
        InitializeComponent();
        
        
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

    public static readonly BindableProperty ItemsProperty =
    BindableProperty.Create(
        nameof(Items),
        typeof(ObservableCollection<IngredientLine>),
        typeof(IngredientListView),
        null,
        propertyChanged: (b, o, n) => ((IngredientListView)b).OnItemsChanged(o, n)
    );

    public ObservableCollection<IngredientLine>? Items
    {
        get => (ObservableCollection<IngredientLine>?)GetValue(ItemsProperty);
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




    private void OnItemsChanged(object? oldValue, object? newValue)
    {
        if (oldValue is ObservableCollection<IngredientLine> oldCol)
            oldCol.CollectionChanged -= OnCollectionChanged;

        if (newValue is ObservableCollection<IngredientLine> newCol)
            newCol.CollectionChanged += OnCollectionChanged;

        UpdateEmptyFlags();
    }

    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        UpdateEmptyFlags();
    }

    private void UpdateEmptyFlags()
    {
        // correcting null issue for items
        var hasItems = Items != null && Items.Count > 0;
        IsEmpty = !hasItems;
        IsNotEmpty = hasItems;
    }


    // changed from OnAppearing to OnParentSet to react to Page changes
    protected override void OnParentSet()
    {
        base.OnParentSet();
        UpdateEmptyFlags();
    }
}

