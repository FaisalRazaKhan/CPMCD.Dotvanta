using System.Globalization;

namespace CPMCD.Dotvanta.Component.Converters
{
    /// <summary>String khali nahi hai to true - Label/Error jaise optional text dikhane/chupane ke liye.</summary>
    public class StringToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => !string.IsNullOrWhiteSpace(value as string);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
