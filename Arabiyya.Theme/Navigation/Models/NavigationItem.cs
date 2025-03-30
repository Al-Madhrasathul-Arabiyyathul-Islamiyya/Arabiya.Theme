using CommunityToolkit.Mvvm.ComponentModel;

namespace Arabiyya.Theme.Navigation.Models;

/// <summary>
/// Represents a navigation item in the navigation system
/// </summary>
public partial class NavigationItem(string id, string label, string icon, Type contentType) : ObservableObject
{
    /// <summary>
    /// Gets or sets the unique identifier for this navigation item
    /// </summary>
    [ObservableProperty]
    private string? _id = id;

    /// <summary>
    /// Gets or sets the display label for this navigation item
    /// </summary>
    [ObservableProperty]
    private string? _label = label;

    /// <summary>
    /// Gets or sets the icon for this navigation item
    /// (Using Phosphor font character code)
    /// </summary>
    [ObservableProperty]
    private string? _icon = icon;

    /// <summary>
    /// Gets or sets the path for URI-based navigation
    /// </summary>
    [ObservableProperty]
    private string? _path;

    /// <summary>
    /// Gets or sets the content type to display when this item is selected
    /// </summary>
    [ObservableProperty]
    private Type? _contentType = contentType;

    /// <summary>
    /// Gets or sets the actual content instance to display when this item is selected
    /// </summary>
    [ObservableProperty]
    private object? _content;

    /// <summary>
    /// Gets or sets a value indicating whether this item is selected
    /// </summary>
    [ObservableProperty]
    private bool _isSelected;

    /// <summary>
    /// Gets or sets a value indicating whether this item is enabled
    /// </summary>
    [ObservableProperty]
    private bool _isEnabled = true;

    /// <summary>
    /// Gets or sets the badge text to display (e.g. notifications)
    /// </summary>
    [ObservableProperty]
    private string? _badgeText;

    /// <summary>
    /// Gets or sets a value indicating whether to show the badge
    /// </summary>
    [ObservableProperty]
    private bool _showBadge;

    /// <summary>
    /// Gets or sets any additional data associated with this item
    /// </summary>
    [ObservableProperty]
    private object? _tag;

    /// <summary>
    /// Gets or sets the factory function to create the content lazily
    /// </summary>
    public required Func<object> ContentFactory { get; set; }
}
