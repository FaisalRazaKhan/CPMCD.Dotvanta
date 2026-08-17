namespace CPMCD.Dotvanta.Component.Controls;

public partial class BorderlessField : ContentView
{
    public BorderlessField()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(BorderlessField), string.Empty, BindingMode.TwoWay);

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly BindableProperty PlaceholderProperty =
        BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(BorderlessField), string.Empty);

    public string Placeholder
    {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    public static readonly BindableProperty LabelProperty =
        BindableProperty.Create(nameof(Label), typeof(string), typeof(BorderlessField), string.Empty);

    public string Label
    {
        get => (string)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public static readonly BindableProperty ErrorTextProperty =
        BindableProperty.Create(nameof(ErrorText), typeof(string), typeof(BorderlessField), string.Empty);

    /// <summary>Validation error - set karo to laal text dikh jayega, khali karo to hide.</summary>
    public string ErrorText
    {
        get => (string)GetValue(ErrorTextProperty);
        set => SetValue(ErrorTextProperty, value);
    }

    public static readonly BindableProperty IsPasswordProperty =
        BindableProperty.Create(nameof(IsPassword), typeof(bool), typeof(BorderlessField), false);

    public bool IsPassword
    {
        get => (bool)GetValue(IsPasswordProperty);
        set => SetValue(IsPasswordProperty, value);
    }

    public static readonly BindableProperty EntryKeyboardProperty =
        BindableProperty.Create(nameof(EntryKeyboard), typeof(Keyboard), typeof(BorderlessField), Keyboard.Default);

    public Keyboard EntryKeyboard
    {
        get => (Keyboard)GetValue(EntryKeyboardProperty);
        set => SetValue(EntryKeyboardProperty, value);
    }
}
