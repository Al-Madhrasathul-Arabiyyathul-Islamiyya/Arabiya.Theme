using Avalonia.Data.Converters;
using System.Globalization;

namespace Arabiyya.Theme.Converters;
public class EnumEqualityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null || parameter == null)
            return false;

        if (Enum.TryParse(value.GetType(), parameter.ToString(), out object? parsedEnum))
        {
            return value.Equals(parsedEnum);
        }

        return value.ToString() == parameter.ToString();
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
