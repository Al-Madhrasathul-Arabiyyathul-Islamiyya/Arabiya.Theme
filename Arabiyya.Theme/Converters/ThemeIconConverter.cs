using System.Globalization;
using Avalonia.Data.Converters;

namespace Arabiyya.Theme.Converters;
public class ThemeIconConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is bool boolValue && boolValue
            ? "\uE330"
            : "\uE472";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
