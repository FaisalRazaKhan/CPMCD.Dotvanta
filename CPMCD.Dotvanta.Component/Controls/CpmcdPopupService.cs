namespace CPMCD.Dotvanta.Component.Controls;

/// <summary>
/// CustomPopup ek ContentView hai (Popup NuGet pe dependency nahi li),
/// isliye khud show/hide karne ke liye chhota helper - page ke root Grid
/// mein overlay ke taur pe add/remove kar deta hai.
///
/// USAGE (page ka root layout Grid hona chahiye):
///   var popup = new CustomPopup { Title = "Confirm", PrimaryButtonText = "Yes" };
///   popup.PrimaryClicked += (s, e) => CpmcdPopupService.Hide(this, popup);
///   CpmcdPopupService.Show(this, popup);
/// </summary>
public static class CpmcdPopupService
{
    public static void Show(ContentPage page, CustomPopup popup)
    {
        if (page?.Content is not Grid rootGrid)
            throw new InvalidOperationException(
                "CpmcdPopupService.Show ke liye page.Content ek Grid hona chahiye (popup usi ke upar overlay hoga).");

        if (!rootGrid.Children.Contains(popup))
        {
            popup.HorizontalOptions = LayoutOptions.Fill;
            popup.VerticalOptions = LayoutOptions.Fill;
            Grid.SetRowSpan(popup, Math.Max(1, rootGrid.RowDefinitions.Count));
            Grid.SetColumnSpan(popup, Math.Max(1, rootGrid.ColumnDefinitions.Count));
            rootGrid.Children.Add(popup);
        }
    }

    public static void Hide(ContentPage page, CustomPopup popup)
    {
        if (page?.Content is Grid rootGrid && rootGrid.Children.Contains(popup))
        {
            rootGrid.Children.Remove(popup);
        }
    }
}
