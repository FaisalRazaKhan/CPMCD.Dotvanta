using System.Text.RegularExpressions;

namespace CPMCD.Dotvanta.Component.Controls;

public partial class CustomEntry : ContentView
{
    public CustomEntry()
    {
        InitializeComponent();
    }

    // ============================================================
    // LABEL
    // ============================================================

    public static readonly BindableProperty LabelProperty =
        BindableProperty.Create(
            nameof(Label),
            typeof(string),
            typeof(CustomEntry),
            string.Empty);

    public string Label
    {
        get => (string)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }


    // ============================================================
    // PLACEHOLDER
    // ============================================================

    public static readonly BindableProperty PlaceholderProperty =
        BindableProperty.Create(
            nameof(Placeholder),
            typeof(string),
            typeof(CustomEntry),
            string.Empty);

    public string Placeholder
    {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }


    // ============================================================
    // TEXT
    // ============================================================

    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(
            nameof(Text),
            typeof(string),
            typeof(CustomEntry),
            string.Empty,
            BindingMode.TwoWay,
            propertyChanged: OnTextPropertyChanged);

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }


    // ============================================================
    // KEYBOARD
    // ============================================================

    public static readonly BindableProperty KeyboardProperty =
        BindableProperty.Create(
            nameof(Keyboard),
            typeof(Keyboard),
            typeof(CustomEntry),
            Keyboard.Text);

    public Keyboard Keyboard
    {
        get => (Keyboard)GetValue(KeyboardProperty);
        set => SetValue(KeyboardProperty, value);
    }


    // ============================================================
    // VALIDATION TYPE
    // ============================================================

    public static readonly BindableProperty ValidationTypeProperty =
        BindableProperty.Create(
            nameof(ValidationType),
            typeof(ValidationType),
            typeof(CustomEntry),
            ValidationType.None,
            propertyChanged: OnValidationTypeChanged);

    public ValidationType ValidationType
    {
        get => (ValidationType)GetValue(ValidationTypeProperty);
        set => SetValue(ValidationTypeProperty, value);
    }


    // ============================================================
    // REQUIRED
    // ============================================================

    public static readonly BindableProperty IsRequiredProperty =
        BindableProperty.Create(
            nameof(IsRequired),
            typeof(bool),
            typeof(CustomEntry),
            true);

    public bool IsRequired
    {
        get => (bool)GetValue(IsRequiredProperty);
        set => SetValue(IsRequiredProperty, value);
    }


    // ============================================================
    // IS VALID
    // ============================================================

    public bool IsValid
    {
        get;
        private set;
    }


    // ============================================================
    // VALIDATION MESSAGE
    // ============================================================

    public string ValidationMessage
    {
        get;
        private set;
    } = string.Empty;


    // ============================================================
    // TEXT CHANGED
    // ============================================================

    private void Entry_TextChanged(
        object sender,
        TextChangedEventArgs e)
    {
        Validate(e.NewTextValue);
    }


    // ============================================================
    // TEXT PROPERTY CHANGED
    // ============================================================

    private static void OnTextPropertyChanged(
        BindableObject bindable,
        object oldValue,
        object newValue)
    {
        var control =
            (CustomEntry)bindable;

        if (control.MainEntry == null)
            return;

        string value =
            newValue?.ToString() ?? string.Empty;

        if (control.MainEntry.Text != value)
        {
            control.MainEntry.Text = value;
        }

        control.Validate(value);
    }


    // ============================================================
    // VALIDATION TYPE CHANGED
    // ============================================================

    private static void OnValidationTypeChanged(
        BindableObject bindable,
        object oldValue,
        object newValue)
    {
        var control =
            (CustomEntry)bindable;

        control.Validate(control.Text);
    }


    // ============================================================
    // VALIDATE
    // ============================================================

    private void Validate(string? value)
    {
        value ??= string.Empty;

        switch (ValidationType)
        {
            case ValidationType.Name:

                ValidateName(value);

                break;


            case ValidationType.Email:

                ValidateEmail(value);

                break;


            case ValidationType.Phone:

                ValidatePhone(value);

                break;


            case ValidationType.None:

            default:

                ResetState();

                break;
        }
    }


    // ============================================================
    // NAME VALIDATION
    // ============================================================

    private void ValidateName(string value)
    {
        string name =
            value.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            if (IsRequired)
            {
                SetError(
                    "Name is required");
            }
            else
            {
                ResetState();
            }

            return;
        }

        if (name.Length < 3)
        {
            SetError(
                "Name must contain at least 3 characters");

            return;
        }

        if (name.Length > 100)
        {
            SetError(
                "Name cannot exceed 100 characters");

            return;
        }

        if (!Regex.IsMatch(
                name,
                @"^[\p{L}][\p{L}\s.'-]*$"))
        {
            SetError(
                "Please enter a valid name");

            return;
        }

        SetSuccess(
            "Name looks good");
    }


    // ============================================================
    // EMAIL VALIDATION
    // ============================================================

    private void ValidateEmail(string value)
    {
        string email =
            value.Trim();

        if (string.IsNullOrWhiteSpace(email))
        {
            if (IsRequired)
            {
                SetError(
                    "Email address is required");
            }
            else
            {
                ResetState();
            }

            return;
        }

        if (email.Length > 254)
        {
            SetError(
                "Email address is too long");

            return;
        }

        bool isValid =
            Regex.IsMatch(
                email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                RegexOptions.IgnoreCase);

        if (!isValid)
        {
            SetError(
                "Please enter a valid email address");

            return;
        }

        SetSuccess(
            "Valid email address");
    }


    // ============================================================
    // PHONE VALIDATION
    // ============================================================

    private void ValidatePhone(string value)
    {
        string phone =
            Regex.Replace(
                value,
                @"\D",
                string.Empty);

        if (string.IsNullOrWhiteSpace(phone))
        {
            if (IsRequired)
            {
                SetError(
                    "Phone number is required");
            }
            else
            {
                ResetState();
            }

            return;
        }

        if (phone.Length != 10)
        {
            SetError(
                "Enter a valid 10-digit phone number");

            return;
        }

        if (phone[0] == '0')
        {
            SetError(
                "Phone number cannot start with 0");

            return;
        }

        SetSuccess(
            "Valid phone number");
    }


    // ============================================================
    // SUCCESS
    // ============================================================

    private void SetSuccess(
        string message)
    {
        IsValid = true;

        ValidationMessage =
            message;

        EntryBorder.Stroke =
            Color.FromArgb("#16A34A");

        EntryBorder.BackgroundColor =
            Application.Current?.RequestedTheme ==
            AppTheme.Dark
                ? Color.FromArgb("#14251B")
                : Color.FromArgb("#F0FDF4");

        StatusIcon.IsVisible = true;

        StatusIcon.Text = "✓";

        StatusIcon.TextColor =
            Color.FromArgb("#16A34A");

        MessageLabel.IsVisible = true;

        MessageLabel.Text =
            message;

        MessageLabel.TextColor =
            Color.FromArgb("#16A34A");
    }


    // ============================================================
    // ERROR
    // ============================================================

    private void SetError(
        string message)
    {
        IsValid = false;

        ValidationMessage =
            message;

        EntryBorder.Stroke =
            Color.FromArgb("#DC2626");

        EntryBorder.BackgroundColor =
            Application.Current?.RequestedTheme ==
            AppTheme.Dark
                ? Color.FromArgb("#2A1717")
                : Color.FromArgb("#FEF2F2");

        StatusIcon.IsVisible = true;

        StatusIcon.Text = "✕";

        StatusIcon.TextColor =
            Color.FromArgb("#DC2626");

        MessageLabel.IsVisible = true;

        MessageLabel.Text =
            message;

        MessageLabel.TextColor =
            Color.FromArgb("#DC2626");
    }


    // ============================================================
    // RESET
    // ============================================================

    private void ResetState()
    {
        IsValid = false;

        ValidationMessage =
            string.Empty;

        EntryBorder.Stroke =
            Application.Current?.RequestedTheme ==
            AppTheme.Dark
                ? Color.FromArgb("#374151")
                : Color.FromArgb("#D1D5DB");

        EntryBorder.BackgroundColor =
            Application.Current?.RequestedTheme ==
            AppTheme.Dark
                ? Color.FromArgb("#1F2937")
                : Color.FromArgb("#FFFFFF");

        StatusIcon.IsVisible =
            false;

        StatusIcon.Text =
            string.Empty;

        MessageLabel.IsVisible =
            false;

        MessageLabel.Text =
            string.Empty;
    }
}