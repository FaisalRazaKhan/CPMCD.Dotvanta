namespace CPMCD.Dotvanta.Component.Controls;

/// <summary>
/// Generic date picker. DOB use-case ke liye bas MaximumDate = DateTime.Today
/// aur MinimumDate = koi reasonable back-limit set kar do (defaults neeche already DOB-friendly hain).
/// </summary>
public partial class CustomDatePicker : ContentView
{
    public CustomDatePicker()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty LabelProperty =
        BindableProperty.Create(nameof(Label), typeof(string), typeof(CustomDatePicker), string.Empty);

    public string Label
    {
        get => (string)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public static readonly BindableProperty SelectedDateProperty =
        BindableProperty.Create(nameof(SelectedDate), typeof(DateTime), typeof(CustomDatePicker), DateTime.Today, BindingMode.TwoWay);

    public DateTime SelectedDate
    {
        get => (DateTime)GetValue(SelectedDateProperty);
        set => SetValue(SelectedDateProperty, value);
    }

    public static readonly BindableProperty MinimumDateProperty =
        BindableProperty.Create(nameof(MinimumDate), typeof(DateTime), typeof(CustomDatePicker), DateTime.Today.AddYears(-100));

    public DateTime MinimumDate
    {
        get => (DateTime)GetValue(MinimumDateProperty);
        set => SetValue(MinimumDateProperty, value);
    }

    public static readonly BindableProperty MaximumDateProperty =
        BindableProperty.Create(nameof(MaximumDate), typeof(DateTime), typeof(CustomDatePicker), DateTime.Today);

    public DateTime MaximumDate
    {
        get => (DateTime)GetValue(MaximumDateProperty);
        set => SetValue(MaximumDateProperty, value);
    }

    public static readonly BindableProperty DateFormatProperty =
        BindableProperty.Create(nameof(DateFormat), typeof(string), typeof(CustomDatePicker), "dd MMM yyyy");

    public string DateFormat
    {
        get => (string)GetValue(DateFormatProperty);
        set => SetValue(DateFormatProperty, value);
    }

    /// <summary>Shortcut - DOB scenario ke liye ek call mein 18+ / 100yr range set kar do.</summary>
    public void ConfigureAsDobPicker(int minAge = 0, int maxAge = 100)
    {
        MaximumDate = DateTime.Today.AddYears(-minAge);
        MinimumDate = DateTime.Today.AddYears(-maxAge);
    }
}
