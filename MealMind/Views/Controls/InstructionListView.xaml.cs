using System.Collections.ObjectModel;

namespace MealMind.Views.Controls;

public partial class InstructionListView : ContentView
{
    public InstructionListView()
    {
        InitializeComponent();
        BindingContext = this;
        UpdateEmptyFlags();
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

    // ===== Checkbox toggle =====

    public static readonly BindableProperty ShowCheckboxProperty =
        BindableProperty.Create(nameof(ShowCheckbox), typeof(bool), typeof(InstructionListView), true);

    public bool ShowCheckbox
    {
        get => (bool)GetValue(ShowCheckboxProperty);
        set => SetValue(ShowCheckboxProperty, value);
    }

    // ===== Items =====

    public static readonly BindableProperty ItemsProperty =
        BindableProperty.Create(
            nameof(Items),
            typeof(ObservableCollection<InstructionRow>),
            typeof(InstructionListView),
            new ObservableCollection<InstructionRow>(),
            propertyChanged: (b, o, n) => ((InstructionListView)b).UpdateEmptyFlags());

    public ObservableCollection<InstructionRow> Items
    {
        get => (ObservableCollection<InstructionRow>)GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    // Flags for XAML
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

    private void UpdateEmptyFlags()
    {
        var hasItems = Items != null && Items.Count > 0;
        IsEmpty = !hasItems;
        IsNotEmpty = hasItems;

        // Optional: if only 1 instruction, you might prefer no checkbox
        if (hasItems && Items.Count == 1)
        {
            // Uncomment if you want this behaviour automatically:
            // ShowCheckbox = false;
        }
    }
}

// Simple display row model (can later be replaced with your Recipe model)
public class InstructionRow
{
    public string Number { get; set; } = "1."; // "1.", "2.", etc.
    public string Text { get; set; } = "";
    public bool IsDone { get; set; }
}