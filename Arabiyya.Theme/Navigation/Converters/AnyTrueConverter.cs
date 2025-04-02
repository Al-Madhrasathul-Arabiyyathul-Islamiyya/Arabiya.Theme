using System.Globalization;
using Avalonia.Data.Converters;

namespace Arabiyya.Theme.Navigation.Converters;
public class AnyTrueConverter : IMultiValueConverter
{
    public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture) => values.OfType<bool>().Any(v => v);
}
