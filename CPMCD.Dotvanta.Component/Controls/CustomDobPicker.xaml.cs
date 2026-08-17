using System.Collections.ObjectModel;
using System.Globalization;

namespace CPMCD.Dotvanta.Component.Controls;

public partial class CustomDobPicker : ContentView
{
    private const double RowHeight = 48;
    private const double WheelHeight = 240;
    private const double SpacerHeight = (WheelHeight - RowHeight) / 2;

    private DateTime _temporaryDate;

    private bool _isInitializing;
    private bool _isProgrammaticScroll;

    private CancellationTokenSource? _monthScrollCts;
    private CancellationTokenSource? _dayScrollCts;
    private CancellationTokenSource? _yearScrollCts;

    private int _selectedMonth;
    private int _selectedDay;
    private int _selectedYear;


    // ============================================================
    // COLLECTIONS
    // ============================================================

    public ObservableCollection<string> Months { get; } = new();

    public ObservableCollection<int> Days { get; } = new();

    public ObservableCollection<int> Years { get; } = new();


    // ============================================================
    // EVENT
    // ============================================================

    public event EventHandler<DateTime>? DateOfBirthChanged;


    // ============================================================
    // CONSTRUCTOR
    // ============================================================

    public CustomDobPicker()
    {
        InitializeComponent();

        _isInitializing = true;

        InitializeDateLimits();

        _temporaryDate = NormalizeDate(DateOfBirth);

        _selectedMonth = _temporaryDate.Month;
        _selectedDay = _temporaryDate.Day;
        _selectedYear = _temporaryDate.Year;

        BuildMonths();
        BuildYears();
        BuildDays();

        BuildMonthWheel();
        BuildDayWheel();
        BuildYearWheel();

        _isInitializing = false;

        UpdateDateText();
        UpdateAge();
        UpdatePreview();
    }


    // ============================================================
    // LABEL
    // ============================================================

    public static readonly BindableProperty LabelProperty =
        BindableProperty.Create(
            nameof(Label),
            typeof(string),
            typeof(CustomDobPicker),
            "Date of Birth");

    public string Label
    {
        get => (string)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }


    // ============================================================
    // DATE OF BIRTH
    // ============================================================

    public static readonly BindableProperty DateOfBirthProperty =
        BindableProperty.Create(
            nameof(DateOfBirth),
            typeof(DateTime),
            typeof(CustomDobPicker),
            DateTime.Today.AddYears(-18),
            BindingMode.TwoWay,
            propertyChanged: OnDobChanged);

    public DateTime DateOfBirth
    {
        get => (DateTime)GetValue(DateOfBirthProperty);
        set => SetValue(DateOfBirthProperty, value);
    }


    // ============================================================
    // MINIMUM AGE
    // ============================================================

    public static readonly BindableProperty MinimumAgeProperty =
        BindableProperty.Create(
            nameof(MinimumAge),
            typeof(int),
            typeof(CustomDobPicker),
            0,
            propertyChanged: OnAgeLimitChanged);

    public int MinimumAge
    {
        get => (int)GetValue(MinimumAgeProperty);
        set => SetValue(
            MinimumAgeProperty,
            Math.Max(0, value));
    }


    // ============================================================
    // MAXIMUM AGE
    // ============================================================

    public static readonly BindableProperty MaximumAgeProperty =
        BindableProperty.Create(
            nameof(MaximumAge),
            typeof(int),
            typeof(CustomDobPicker),
            100,
            propertyChanged: OnAgeLimitChanged);

    public int MaximumAge
    {
        get => (int)GetValue(MaximumAgeProperty);
        set => SetValue(
            MaximumAgeProperty,
            Math.Max(1, value));
    }


    // ============================================================
    // MINIMUM DATE
    // ============================================================

    public static readonly BindableProperty MinimumDateProperty =
        BindableProperty.Create(
            nameof(MinimumDate),
            typeof(DateTime),
            typeof(CustomDobPicker),
            DateTime.Today.AddYears(-100));

    public DateTime MinimumDate
    {
        get => (DateTime)GetValue(MinimumDateProperty);
        private set => SetValue(
            MinimumDateProperty,
            value);
    }


    // ============================================================
    // MAXIMUM DATE
    // ============================================================

    public static readonly BindableProperty MaximumDateProperty =
        BindableProperty.Create(
            nameof(MaximumDate),
            typeof(DateTime),
            typeof(CustomDobPicker),
            DateTime.Today);

    public DateTime MaximumDate
    {
        get => (DateTime)GetValue(MaximumDateProperty);
        private set => SetValue(
            MaximumDateProperty,
            value);
    }


    // ============================================================
    // DATE FORMAT
    // ============================================================

    public static readonly BindableProperty DateFormatProperty =
        BindableProperty.Create(
            nameof(DateFormat),
            typeof(string),
            typeof(CustomDobPicker),
            "dd MMM yyyy",
            propertyChanged: OnDateFormatChanged);

    public string DateFormat
    {
        get => (string)GetValue(DateFormatProperty);
        set => SetValue(
            DateFormatProperty,
            value);
    }


    // ============================================================
    // SHOW AGE
    // ============================================================

    public static readonly BindableProperty ShowAgeProperty =
        BindableProperty.Create(
            nameof(ShowAge),
            typeof(bool),
            typeof(CustomDobPicker),
            true);

    public bool ShowAge
    {
        get => (bool)GetValue(ShowAgeProperty);
        set => SetValue(
            ShowAgeProperty,
            value);
    }


    // ============================================================
    // ERROR TEXT
    // ============================================================

    public static readonly BindableProperty ErrorTextProperty =
        BindableProperty.Create(
            nameof(ErrorText),
            typeof(string),
            typeof(CustomDobPicker),
            string.Empty);

    public string ErrorText
    {
        get => (string)GetValue(ErrorTextProperty);
        set => SetValue(
            ErrorTextProperty,
            value);
    }


    // ============================================================
    // AGE DISPLAY
    // ============================================================

    public static readonly BindableProperty AgeDisplayProperty =
        BindableProperty.Create(
            nameof(AgeDisplay),
            typeof(string),
            typeof(CustomDobPicker),
            string.Empty);

    public string AgeDisplay
    {
        get => (string)GetValue(AgeDisplayProperty);
        private set => SetValue(
            AgeDisplayProperty,
            value);
    }


    // ============================================================
    // DATE TEXT
    // ============================================================

    public static readonly BindableProperty DateTextProperty =
        BindableProperty.Create(
            nameof(DateText),
            typeof(string),
            typeof(CustomDobPicker),
            string.Empty);

    public string DateText
    {
        get => (string)GetValue(DateTextProperty);
        private set => SetValue(
            DateTextProperty,
            value);
    }


    // ============================================================
    // CALCULATED AGE
    // ============================================================

    public int CalculatedAge
    {
        get
        {
            var today = DateTime.Today;

            var age =
                today.Year -
                DateOfBirth.Year;

            if (DateOfBirth.Date >
                today.AddYears(-age))
            {
                age--;
            }

            return Math.Max(0, age);
        }
    }


    // ============================================================
    // INITIAL DATE LIMITS
    // ============================================================

    private void InitializeDateLimits()
    {
        MaximumDate =
            DateTime.Today.AddYears(
                -MinimumAge);

        MinimumDate =
            DateTime.Today.AddYears(
                -MaximumAge);

        if (MinimumDate > MaximumDate)
        {
            MinimumDate =
                MaximumDate.AddYears(-100);
        }
    }


    // ============================================================
    // BUILD MONTHS
    // ============================================================

    private void BuildMonths()
    {
        Months.Clear();

        for (int i = 1; i <= 12; i++)
        {
            Months.Add(
                CultureInfo.CurrentCulture
                    .DateTimeFormat
                    .GetAbbreviatedMonthName(i));
        }
    }


    // ============================================================
    // BUILD YEARS
    // ============================================================

    private void BuildYears()
    {
        Years.Clear();

        for (
            int year = MinimumDate.Year;
            year <= MaximumDate.Year;
            year++)
        {
            Years.Add(year);
        }
    }


    // ============================================================
    // BUILD DAYS
    // ============================================================

    private void BuildDays()
    {
        Days.Clear();

        int daysInMonth =
            DateTime.DaysInMonth(
                _temporaryDate.Year,
                _temporaryDate.Month);

        for (
            int day = 1;
            day <= daysInMonth;
            day++)
        {
            Days.Add(day);
        }
    }


    // ============================================================
    // BUILD MONTH WHEEL
    // ============================================================

    private void BuildMonthWheel()
    {
        MonthContainer.Children.Clear();

        AddSpacer(MonthContainer);

        for (int i = 0; i < Months.Count; i++)
        {
            int monthNumber = i + 1;

            var label =
                CreateWheelLabel(
                    Months[i],
                    monthNumber == _selectedMonth);

            MonthContainer.Children.Add(label);
        }

        AddSpacer(MonthContainer);
    }


    // ============================================================
    // BUILD DAY WHEEL
    // ============================================================

    private void BuildDayWheel()
    {
        DayContainer.Children.Clear();

        AddSpacer(DayContainer);

        foreach (int day in Days)
        {
            var label =
                CreateWheelLabel(
                    day.ToString("00"),
                    day == _selectedDay);

            DayContainer.Children.Add(label);
        }

        AddSpacer(DayContainer);
    }


    // ============================================================
    // BUILD YEAR WHEEL
    // ============================================================

    private void BuildYearWheel()
    {
        YearContainer.Children.Clear();

        AddSpacer(YearContainer);

        foreach (int year in Years)
        {
            var label =
                CreateWheelLabel(
                    year.ToString(),
                    year == _selectedYear);

            YearContainer.Children.Add(label);
        }

        AddSpacer(YearContainer);
    }


    // ============================================================
    // SPACER
    // ============================================================

    private static void AddSpacer(
        VerticalStackLayout container)
    {
        container.Children.Add(
            new BoxView
            {
                HeightRequest = SpacerHeight,
                BackgroundColor = Colors.Transparent
            });
    }


    // ============================================================
    // CREATE WHEEL LABEL
    // ============================================================

    private Label CreateWheelLabel(
        string text,
        bool selected)
    {
        var label =
            new Label
            {
                Text = text,

                HeightRequest = RowHeight,

                FontSize = selected
                    ? 16
                    : 14,

                FontAttributes = selected
                    ? FontAttributes.Bold
                    : FontAttributes.None,

                HorizontalTextAlignment =
                    TextAlignment.Center,

                VerticalTextAlignment =
                    TextAlignment.Center,

                TextColor =
                    selected
                        ? GetSelectedTextColor()
                        : GetNormalTextColor(),

                Opacity =
                    selected
                        ? 1.0
                        : 0.45
            };

        return label;
    }


    // ============================================================
    // COLORS
    // ============================================================

    private Color GetSelectedTextColor()
    {
        return Application.Current?
            .RequestedTheme == AppTheme.Dark
                ? Color.FromArgb("#60A5FA")
                : Color.FromArgb("#2563EB");
    }


    private Color GetNormalTextColor()
    {
        return Application.Current?
            .RequestedTheme == AppTheme.Dark
                ? Color.FromArgb("#9CA3AF")
                : Color.FromArgb("#64748B");
    }


    // ============================================================
    // DATE FIELD TAP
    // ============================================================

    private async void OnDateTapped(
        object sender,
        TappedEventArgs e)
    {
        await OpenPickerAsync();
    }


    // ============================================================
    // OPEN
    // ============================================================

    private async Task OpenPickerAsync()
    {
        _temporaryDate =
            NormalizeDate(DateOfBirth);

        _selectedMonth =
            _temporaryDate.Month;

        _selectedDay =
            _temporaryDate.Day;

        _selectedYear =
            _temporaryDate.Year;

        BuildDays();

        BuildMonthWheel();
        BuildDayWheel();
        BuildYearWheel();

        UpdatePreview();

        PickerOverlay.IsVisible = true;

        await Task.Delay(50);

        await ScrollToSelectedAsync();
    }


    // ============================================================
    // SCROLL TO SELECTED
    // ============================================================

    private async Task ScrollToSelectedAsync()
    {
        _isProgrammaticScroll = true;

        try
        {
            await MonthScroll.ScrollToAsync(
                0,
                GetScrollYForIndex(
                    _selectedMonth - 1),
                false);

            await DayScroll.ScrollToAsync(
                0,
                GetScrollYForIndex(
                    _selectedDay - 1),
                false);

            int yearIndex =
                _selectedYear -
                MinimumDate.Year;

            await YearScroll.ScrollToAsync(
                0,
                GetScrollYForIndex(yearIndex),
                false);

            await Task.Delay(50);
        }
        finally
        {
            _isProgrammaticScroll = false;
        }
    }


    // ============================================================
    // GET SCROLL Y
    // ============================================================

    private static double GetScrollYForIndex(
        int index)
    {
        return index * RowHeight;
    }


    // ============================================================
    // MONTH SCROLL
    // ============================================================

    private async void MonthScrollScrolled(
        object sender,
        ScrolledEventArgs e)
    {
        if (_isProgrammaticScroll)
            return;

        await HandleMonthScrollAsync(e.ScrollY);
    }


    private async Task HandleMonthScrollAsync(
        double scrollY)
    {
        _monthScrollCts?.Cancel();

        var cts =
            new CancellationTokenSource();

        _monthScrollCts = cts;

        try
        {
            await Task.Delay(
                100,
                cts.Token);

            int index =
                GetNearestIndex(
                    scrollY,
                    Months.Count);

            await SnapMonthAsync(index);
        }
        catch (TaskCanceledException)
        {
        }
    }


    // ============================================================
    // DAY SCROLL
    // ============================================================

    private async void DayScrollScrolled(
        object sender,
        ScrolledEventArgs e)
    {
        if (_isProgrammaticScroll)
            return;

        await HandleDayScrollAsync(e.ScrollY);
    }


    private async Task HandleDayScrollAsync(
        double scrollY)
    {
        _dayScrollCts?.Cancel();

        var cts =
            new CancellationTokenSource();

        _dayScrollCts = cts;

        try
        {
            await Task.Delay(
                100,
                cts.Token);

            int index =
                GetNearestIndex(
                    scrollY,
                    Days.Count);

            await SnapDayAsync(index);
        }
        catch (TaskCanceledException)
        {
        }
    }


    // ============================================================
    // YEAR SCROLL
    // ============================================================

    private async void YearScrollScrolled(
        object sender,
        ScrolledEventArgs e)
    {
        if (_isProgrammaticScroll)
            return;

        await HandleYearScrollAsync(e.ScrollY);
    }


    private async Task HandleYearScrollAsync(
        double scrollY)
    {
        _yearScrollCts?.Cancel();

        var cts =
            new CancellationTokenSource();

        _yearScrollCts = cts;

        try
        {
            await Task.Delay(
                100,
                cts.Token);

            int index =
                GetNearestIndex(
                    scrollY,
                    Years.Count);

            await SnapYearAsync(index);
        }
        catch (TaskCanceledException)
        {
        }
    }


    // ============================================================
    // FIND NEAREST INDEX
    // ============================================================

    private static int GetNearestIndex(
        double scrollY,
        int count)
    {
        if (count <= 0)
            return 0;

        int index =
            (int)Math.Round(
                scrollY / RowHeight,
                MidpointRounding.AwayFromZero);

        return Math.Clamp(
            index,
            0,
            count - 1);
    }


    // ============================================================
    // SNAP MONTH
    // ============================================================

    private async Task SnapMonthAsync(
        int index)
    {
        int month =
            index + 1;

        if (month < 1 ||
            month > 12)
            return;

        await SnapScrollAsync(
            MonthScroll,
            index);

        if (month == _selectedMonth)
            return;

        _selectedMonth =
            month;

        UpdateTemporaryDate();
        RebuildMonthSelection();
        UpdatePreview();
    }


    // ============================================================
    // SNAP DAY
    // ============================================================

    private async Task SnapDayAsync(
        int index)
    {
        if (index < 0 ||
            index >= Days.Count)
            return;

        await SnapScrollAsync(
            DayScroll,
            index);

        int day =
            Days[index];

        if (day == _selectedDay)
            return;

        _selectedDay =
            day;

        UpdateTemporaryDate();
        RebuildDaySelection();
        UpdatePreview();
    }


    // ============================================================
    // SNAP YEAR
    // ============================================================

    private async Task SnapYearAsync(
        int index)
    {
        if (index < 0 ||
            index >= Years.Count)
            return;

        await SnapScrollAsync(
            YearScroll,
            index);

        int year =
            Years[index];

        if (year == _selectedYear)
            return;

        _selectedYear =
            year;

        UpdateTemporaryDate();

        BuildDays();

        BuildDayWheel();

        _isProgrammaticScroll = true;

        try
        {
            await DayScroll.ScrollToAsync(
                0,
                GetScrollYForIndex(
                    _selectedDay - 1),
                false);
        }
        finally
        {
            _isProgrammaticScroll = false;
        }

        UpdatePreview();
    }


    // ============================================================
    // SNAP SCROLL
    // ============================================================

    private async Task SnapScrollAsync(
        ScrollView scrollView,
        int index)
    {
        _isProgrammaticScroll = true;

        try
        {
            double target =
                GetScrollYForIndex(index);

            await scrollView.ScrollToAsync(
                0,
                target,
                true);

            await Task.Delay(40);
        }
        finally
        {
            _isProgrammaticScroll = false;
        }
    }


    // ============================================================
    // TEMPORARY DATE
    // ============================================================

    private void UpdateTemporaryDate()
    {
        int maxDay =
            DateTime.DaysInMonth(
                _selectedYear,
                _selectedMonth);

        _selectedDay =
            Math.Min(
                _selectedDay,
                maxDay);

        _temporaryDate =
            new DateTime(
                _selectedYear,
                _selectedMonth,
                _selectedDay);

        BuildDays();
    }


    // ============================================================
    // REBUILD MONTH SELECTION
    // ============================================================

    private void RebuildMonthSelection()
    {
        for (int i = 1;
             i < MonthContainer.Children.Count - 1;
             i++)
        {
            if (MonthContainer.Children[i]
                is Label label)
            {
                int month =
                    i;

                bool selected =
                    month == _selectedMonth;

                ApplySelectionStyle(
                    label,
                    selected);
            }
        }
    }


    // ============================================================
    // REBUILD DAY SELECTION
    // ============================================================

    private void RebuildDaySelection()
    {
        for (int i = 1;
             i < DayContainer.Children.Count - 1;
             i++)
        {
            if (DayContainer.Children[i]
                is Label label)
            {
                int day =
                    i;

                bool selected =
                    day == _selectedDay;

                ApplySelectionStyle(
                    label,
                    selected);
            }
        }
    }


    // ============================================================
    // APPLY SELECTION STYLE
    // ============================================================

    private void ApplySelectionStyle(
        Label label,
        bool selected)
    {
        label.FontSize =
            selected
                ? 16
                : 14;

        label.FontAttributes =
            selected
                ? FontAttributes.Bold
                : FontAttributes.None;

        label.TextColor =
            selected
                ? GetSelectedTextColor()
                : GetNormalTextColor();

        label.Opacity =
            selected
                ? 1.0
                : 0.45;
    }


    // ============================================================
    // UPDATE PREVIEW
    // ============================================================

    private void UpdatePreview()
    {
        if (SelectedDatePreview == null)
            return;

        SelectedDatePreview.Text =
            _temporaryDate.ToString(
                DateFormat,
                CultureInfo.CurrentCulture);

        int age =
            CalculateAge(
                _temporaryDate);

        PreviewAge.Text =
            $"{age} yrs";

        SheetAgeText.Text =
            $"{age} years old";
    }


    // ============================================================
    // UPDATE MAIN DATE TEXT
    // ============================================================

    private void UpdateDateText()
    {
        DateText =
            DateOfBirth.ToString(
                DateFormat,
                CultureInfo.CurrentCulture);
    }


    // ============================================================
    // UPDATE AGE
    // ============================================================

    private void UpdateAge()
    {
        AgeDisplay =
            $"{CalculatedAge} yrs";
    }


    // ============================================================
    // AGE CALCULATION
    // ============================================================

    private static int CalculateAge(
        DateTime dateOfBirth)
    {
        var today =
            DateTime.Today;

        int age =
            today.Year -
            dateOfBirth.Year;

        if (dateOfBirth.Date >
            today.AddYears(-age))
        {
            age--;
        }

        return Math.Max(
            0,
            age);
    }


    // ============================================================
    // NORMALIZE DATE
    // ============================================================

    private DateTime NormalizeDate(
        DateTime date)
    {
        if (date < MinimumDate)
            return MinimumDate.Date;

        if (date > MaximumDate)
            return MaximumDate.Date;

        return date.Date;
    }


    // ============================================================
    // DONE
    // ============================================================

    private void OkClicked(
        object sender,
        EventArgs e)
    {
        DateTime selected =
            NormalizeDate(
                _temporaryDate);

        DateOfBirth =
            selected;

        PickerOverlay.IsVisible =
            false;
    }


    // ============================================================
    // CANCEL
    // ============================================================

    private void CancelClicked(
        object sender,
        EventArgs e)
    {
        PickerOverlay.IsVisible =
            false;
    }


    // ============================================================
    // OUTSIDE TAP
    // ============================================================

    private void OnOverlayTapped(
        object sender,
        TappedEventArgs e)
    {
        PickerOverlay.IsVisible =
            false;
    }


    // ============================================================
    // DOB CHANGED
    // ============================================================

    private static void OnDobChanged(
        BindableObject bindable,
        object oldValue,
        object newValue)
    {
        var picker =
            (CustomDobPicker)bindable;

        if (newValue is not DateTime date)
            return;

        picker._temporaryDate =
            picker.NormalizeDate(date);

        picker._selectedMonth =
            picker._temporaryDate.Month;

        picker._selectedDay =
            picker._temporaryDate.Day;

        picker._selectedYear =
            picker._temporaryDate.Year;

        picker.UpdateDateText();
        picker.UpdateAge();
        picker.UpdatePreview();

        if (!picker._isInitializing)
        {
            picker.DateOfBirthChanged?.Invoke(
                picker,
                picker.DateOfBirth);
        }
    }


    // ============================================================
    // AGE LIMIT CHANGED
    // ============================================================

    private static void OnAgeLimitChanged(
        BindableObject bindable,
        object oldValue,
        object newValue)
    {
        var picker =
            (CustomDobPicker)bindable;

        picker.MaximumDate =
            DateTime.Today.AddYears(
                -picker.MinimumAge);

        picker.MinimumDate =
            DateTime.Today.AddYears(
                -picker.MaximumAge);

        if (picker.MinimumDate >
            picker.MaximumDate)
        {
            picker.MinimumDate =
                picker.MaximumDate.AddYears(-100);
        }

        picker.BuildYears();

        DateTime corrected =
            picker.NormalizeDate(
                picker.DateOfBirth);

        picker._temporaryDate =
            corrected;

        picker._selectedMonth =
            corrected.Month;

        picker._selectedDay =
            corrected.Day;

        picker._selectedYear =
            corrected.Year;

        picker.BuildDays();

        if (picker.MonthContainer != null)
        {
            picker.BuildMonthWheel();
            picker.BuildDayWheel();
            picker.BuildYearWheel();
        }

        picker.UpdateDateText();
        picker.UpdateAge();
        picker.UpdatePreview();
    }


    // ============================================================
    // DATE FORMAT CHANGED
    // ============================================================

    private static void OnDateFormatChanged(
        BindableObject bindable,
        object oldValue,
        object newValue)
    {
        var picker =
            (CustomDobPicker)bindable;

        picker.UpdateDateText();
        picker.UpdatePreview();
    }
}