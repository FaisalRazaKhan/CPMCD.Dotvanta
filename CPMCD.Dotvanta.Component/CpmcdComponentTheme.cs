using CPMCD.Dotvanta.Component.Resources.Styles;
using Colors = CPMCD.Dotvanta.Component.Resources.Styles.Colors;

namespace CPMCD.Dotvanta.Component
{
    /// <summary>
    /// Consuming app apni App.xaml.cs (ya MauiProgram) mein ek line likhega:
    ///   CpmcdComponentTheme.Register(Application.Current);
    /// Isse light/dark color dictionary automatically merge ho jayegi,
    /// aur sab CPMCD components turant theme-aware ho jayenge.
    /// </summary>
    public static class CpmcdComponentTheme
    {
        public static void Register(Application app)
        {
            if (app == null) return;

            bool alreadyMerged = app.Resources.MergedDictionaries.Any(d => d is Colors);
            if (!alreadyMerged)
            {
                app.Resources.MergedDictionaries.Add(new Colors());
            }
        }
    }
}
