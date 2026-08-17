namespace CPMCD.Dotvanta.Component.Controls;

public partial class CustomPopup : ContentView
{
    public event EventHandler PrimaryClicked;
    public event EventHandler SecondaryClicked;

    public CustomPopup()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(CustomPopup), string.Empty);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly BindableProperty PopupContentProperty =
        BindableProperty.Create(nameof(PopupContent), typeof(View), typeof(CustomPopup), null);

    /// <summary>Popup ke andar koi bhi custom view daalo (form, message, list, etc.)</summary>
    public View PopupContent
    {
        get => (View)GetValue(PopupContentProperty);
        set => SetValue(PopupContentProperty, value);
    }

    public static readonly BindableProperty PrimaryButtonTextProperty =
        BindableProperty.Create(nameof(PrimaryButtonText), typeof(string), typeof(CustomPopup), "OK");

    public string PrimaryButtonText
    {
        get => (string)GetValue(PrimaryButtonTextProperty);
        set => SetValue(PrimaryButtonTextProperty, value);
    }

    public static readonly BindableProperty SecondaryButtonTextProperty =
        BindableProperty.Create(nameof(SecondaryButtonText), typeof(string), typeof(CustomPopup), "Cancel");

    public string SecondaryButtonText
    {
        get => (string)GetValue(SecondaryButtonTextProperty);
        set => SetValue(SecondaryButtonTextProperty, value);
    }

    public static readonly BindableProperty IsSecondaryButtonVisibleProperty =
        BindableProperty.Create(nameof(IsSecondaryButtonVisible), typeof(bool), typeof(CustomPopup), true);

    public bool IsSecondaryButtonVisible
    {
        get => (bool)GetValue(IsSecondaryButtonVisibleProperty);
        set => SetValue(IsSecondaryButtonVisibleProperty, value);
    }

    private void OnPrimaryClicked(object sender, EventArgs e) => PrimaryClicked?.Invoke(this, EventArgs.Empty);

    private void OnSecondaryClicked(object sender, EventArgs e) => SecondaryClicked?.Invoke(this, EventArgs.Empty);
}
