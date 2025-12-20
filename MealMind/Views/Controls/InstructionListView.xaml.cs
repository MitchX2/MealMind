using MealMind.Models;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace MealMind.Views.Controls;

public partial class InstructionListView : ContentView
{
    public InstructionListView()
    {
        InitializeComponent();
    }

    // ===== Title =====
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(InstructionListView), "Instructions");

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly BindableProperty ShowTitleProperty =
        BindableProperty.Create(nameof(ShowTitle), typeof(bool), typeof(InstructionListView), true);

    public bool ShowTitle
    {
        get => (bool)GetValue(ShowTitleProperty);
        set => SetValue(ShowTitleProperty, value);
    }

    // ===== Empty State =====
    public static readonly BindableProperty EmptyTextProperty =
        BindableProperty.Create(nameof(EmptyText), typeof(string), typeof(InstructionListView), "No instructions available.");

    public string EmptyText
    {
        get => (string)GetValue(EmptyTextProperty);
        set => SetValue(EmptyTextProperty, value);
    }

    // ===== Checkbox toggle (external) =====
    public static readonly BindableProperty ShowCheckboxProperty =
        BindableProperty.Create(nameof(ShowCheckbox), typeof(bool), typeof(InstructionListView), true,
            propertyChanged: (b, o, n) => ((InstructionListView)b).UpdateEmptyFlags());

    public bool ShowCheckbox
    {
        get => (bool)GetValue(ShowCheckboxProperty);
        set => SetValue(ShowCheckboxProperty, value);
    }

    // ===== Items =====
    public static readonly BindableProperty ItemsProperty =
        BindableProperty.Create(
            nameof(Items),
            typeof(ObservableCollection<InstructionStep>),
            typeof(InstructionListView),
            null,
            propertyChanged: (b, o, n) => ((InstructionListView)b).OnItemsChanged(o, n)
        );

    public ObservableCollection<InstructionStep>? Items
    {
        get => (ObservableCollection<InstructionStep>?)GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    // ===== Flags for XAML =====
    public static readonly BindableProperty IsEmptyProperty =
        BindableProperty.Create(nameof(IsEmpty), typeof(bool), typeof(InstructionListView), true);

    public bool IsEmpty
    {
        get => (bool)GetValue(IsEmptyProperty);
        private set => SetValue(IsEmptyProperty, value);
    }

    public static readonly BindableProperty IsNotEmptyProperty =
        BindableProperty.Create(nameof(IsNotEmpty), typeof(bool), typeof(InstructionListView), false);

    public bool IsNotEmpty
    {
        get => (bool)GetValue(IsNotEmptyProperty);
        private set => SetValue(IsNotEmptyProperty, value);
    }

    // Internal: respects VM flag AND hides when only 1 step
    public static readonly BindableProperty EffectiveShowCheckboxProperty =
        BindableProperty.Create(nameof(EffectiveShowCheckbox), typeof(bool), typeof(InstructionListView), true);

    public bool EffectiveShowCheckbox
    {
        get => (bool)GetValue(EffectiveShowCheckboxProperty);
        private set => SetValue(EffectiveShowCheckboxProperty, value);
    }

    protected override void OnParentSet()
    {
        base.OnParentSet();
        UpdateEmptyFlags();
    }

    private void OnItemsChanged(object? oldValue, object? newValue)
    {
        if (oldValue is ObservableCollection<InstructionStep> oldCol)
            oldCol.CollectionChanged -= OnCollectionChanged;

        if (newValue is ObservableCollection<InstructionStep> newCol)
            newCol.CollectionChanged += OnCollectionChanged;

        UpdateEmptyFlags();
    }

    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        UpdateEmptyFlags();
    }

    private void UpdateEmptyFlags()
    {
        var hasItems = Items != null && Items.Count > 0;
        IsEmpty = !hasItems;
        IsNotEmpty = hasItems;

        // Respect VM setting, but also hide checkbox when only 1 step
        var hideBecauseSingleStep = hasItems && Items!.Count == 1;
        EffectiveShowCheckbox = ShowCheckbox && !hideBecauseSingleStep;
    }
}