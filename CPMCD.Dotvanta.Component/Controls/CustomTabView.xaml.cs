using System.Collections.ObjectModel;

namespace CPMCD.Dotvanta.Component.Controls;

public enum CpmcdTabOrientation
{
    Horizontal,
    Vertical
}

/// <summary>Ek tab entry - Title dikhega, Content select hone par TabContentHost mein aa jayega.</summary>
public class CpmcdTabItem
{
    public string Title { get; set; }
    public View Content { get; set; }
}

/// <summary>Horizontal ya Vertical dono modes support karta hai - Orientation property se switch karo.</summary>
public partial class CustomTabView : ContentView
{
    public CustomTabView()
    {
        InitializeComponent();
        Items = new ObservableCollection<CpmcdTabItem>();
        ApplyOrientation();
    }

    public static readonly BindableProperty ItemsProperty =
        BindableProperty.Create(nameof(Items), typeof(ObservableCollection<CpmcdTabItem>), typeof(CustomTabView),
            null, propertyChanged: OnItemsChanged);

    public ObservableCollection<CpmcdTabItem> Items
    {
        get => (ObservableCollection<CpmcdTabItem>)GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    public static readonly BindableProperty OrientationProperty =
        BindableProperty.Create(nameof(Orientation), typeof(CpmcdTabOrientation), typeof(CustomTabView),
            CpmcdTabOrientation.Horizontal, propertyChanged: OnOrientationChanged);

    public CpmcdTabOrientation Orientation
    {
        get => (CpmcdTabOrientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public static readonly BindableProperty SelectedIndexProperty =
        BindableProperty.Create(nameof(SelectedIndex), typeof(int), typeof(CustomTabView), -1,
            BindingMode.TwoWay, propertyChanged: OnSelectedIndexChanged);

    public int SelectedIndex
    {
        get => (int)GetValue(SelectedIndexProperty);
        set => SetValue(SelectedIndexProperty, value);
    }

    public event EventHandler<int> TabChanged;

    private static void OnItemsChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (CustomTabView)bindable;
        view.RebuildTabStrip();
        if (view.Items?.Count > 0 && view.SelectedIndex < 0)
            view.SelectedIndex = 0;
    }

    private static void OnOrientationChanged(BindableObject bindable, object oldValue, object newValue)
    {
        ((CustomTabView)bindable).ApplyOrientation();
    }

    private static void OnSelectedIndexChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (CustomTabView)bindable;
        view.ShowSelectedContent();
        view.HighlightSelectedTab();
        view.TabChanged?.Invoke(view, (int)newValue);
    }

    private void ApplyOrientation()
    {
        LayoutRoot.RowDefinitions.Clear();
        LayoutRoot.ColumnDefinitions.Clear();

        if (Orientation == CpmcdTabOrientation.Horizontal)
        {
            LayoutRoot.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            LayoutRoot.RowDefinitions.Add(new RowDefinition(GridLength.Star));
            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

            TabStripScroll.Orientation = ScrollOrientation.Horizontal;
            TabStrip.Direction = Microsoft.Maui.Layouts.FlexDirection.Row;

            Grid.SetRow(TabStripScroll, 0); Grid.SetColumn(TabStripScroll, 0);
            Grid.SetRow(TabContentHost, 1); Grid.SetColumn(TabContentHost, 0);
        }
        else
        {
            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
            LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            LayoutRoot.RowDefinitions.Add(new RowDefinition(GridLength.Star));

            TabStripScroll.Orientation = ScrollOrientation.Vertical;
            TabStrip.Direction = Microsoft.Maui.Layouts.FlexDirection.Column;

            Grid.SetRow(TabStripScroll, 0); Grid.SetColumn(TabStripScroll, 0);
            Grid.SetRow(TabContentHost, 0); Grid.SetColumn(TabContentHost, 1);
        }
    }

    private void RebuildTabStrip()
    {
        TabStrip.Children.Clear();
        if (Items == null) return;

        for (int i = 0; i < Items.Count; i++)
        {
            int index = i;
            var btn = new Button
            {
                Text = Items[i].Title,
                Margin = 4,
                Padding = new Thickness(14, 8),
                CornerRadius = 8,
                BackgroundColor = Colors.Transparent
            };
            btn.SetAppThemeColor(Button.TextColorProperty,
                (Color)Application.Current.Resources["CpmcdTextLight"],
                (Color)Application.Current.Resources["CpmcdTextDark"]);

            btn.Clicked += (s, e) => SelectedIndex = index;
            TabStrip.Children.Add(btn);
        }

        HighlightSelectedTab();
    }

    private void HighlightSelectedTab()
    {
        for (int i = 0; i < TabStrip.Children.Count; i++)
        {
            if (TabStrip.Children[i] is Button btn)
            {
                bool selected = i == SelectedIndex;
                btn.BackgroundColor = selected
                    ? (Application.Current.RequestedTheme == AppTheme.Dark
                        ? (Color)Application.Current.Resources["CpmcdPrimaryDark"]
                        : (Color)Application.Current.Resources["CpmcdPrimaryLight"])
                    : Colors.Transparent;
                btn.TextColor = selected ? Colors.White
                    : (Application.Current.RequestedTheme == AppTheme.Dark
                        ? (Color)Application.Current.Resources["CpmcdTextDark"]
                        : (Color)Application.Current.Resources["CpmcdTextLight"]);
            }
        }
    }

    private void ShowSelectedContent()
    {
        if (Items != null && SelectedIndex >= 0 && SelectedIndex < Items.Count)
        {
            TabContentHost.Content = Items[SelectedIndex].Content;
        }
    }
}
