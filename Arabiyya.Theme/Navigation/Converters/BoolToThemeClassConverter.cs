using System.Globalization;
using Avalonia.Data.Converters;

namespace Arabiyya.Theme.Navigation.Converters;

/// <summary>
/// Converts a boolean to a theme class name
/// </summary>
public class BoolToThemeClassConverter : IValueConverter
{
    /// <summary>
    /// Converts a boolean to a theme class
    /// </summary>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo? culture)
    {
        return value is bool useGlass && useGlass
            ? "GlassPanel"
            : "AcrylicPanel";
    }
        

    /// <summary>
    /// Not implemented
    /// </summary>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo? culture)
    {
        throw new NotImplementedException();
    }
}
