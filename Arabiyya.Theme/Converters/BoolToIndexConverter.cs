using System.Globalization;
using Avalonia.Data.Converters;

namespace Arabiyya.Theme.Converters;
public class BoolToIndexConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is bool boolValue && boolValue ? 1 : 0;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is int intValue && intValue == 1;
    }
}
