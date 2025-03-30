using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Data.Converters;

namespace Arabiyya.Theme.Navigation.Converters;

/// <summary>
/// Converts a boolean expanded state to an icon character
/// </summary>
public class ExpandIconConverter : IValueConverter
{
    /// <summary>
    /// Converts a boolean to an icon
    /// </summary>
    public object Convert(object? value, Type? targetType, object? parameter, CultureInfo culture) => value is bool expanded && expanded
            ? "\uE335" // Collapse icon (left arrow)
            : "\uE336"; // Expand icon (right arrow)

    /// <summary>
    /// Not implemented
    /// </summary>
    public object ConvertBack(object? value, Type? targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
}
