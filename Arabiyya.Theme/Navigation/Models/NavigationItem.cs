using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Arabiyya.Theme.Navigation.Models;

/// <summary>
/// Represents a navigation item in the navigation system
/// </summary>
public partial class NavigationItem : ObservableObject
{
    /// <summary>
    /// Gets or sets the unique identifier for this navigation item.
    /// </summary>
    [ObservableProperty]
    private string? _id;

    /// <summary>
    /// Gets or sets the display label for this navigation item.
    /// </summary>
    [ObservableProperty]
    private string? _label;

    /// <summary>
    /// Gets or sets the icon for this navigation item
    /// (Example: Using Phosphor font character code).
    /// </summary>
    [ObservableProperty]
    private string? _icon;

    /// <summary>
    /// Gets or sets the path for URI-based navigation.
    /// </summary>
    [ObservableProperty]
    private string? _path;

    /// <summary>
    /// Gets or sets the content type to display when this item is selected.
    /// </summary>
    [ObservableProperty]
    private Type? _contentType;

    /// <summary>
    /// Gets or sets the actual content instance to display when this item is selected.
    /// </summary>
    [ObservableProperty]
    private object? _content;

    /// <summary>
    /// Gets or sets a value indicating whether this item is selected.
    /// </summary>
    [ObservableProperty]
    private bool _isSelected;

    /// <summary>
    /// Gets or sets a value indicating whether this item is enabled.
    /// </summary>
    [ObservableProperty]
    private bool _isEnabled = true;

    /// <summary>
    /// Gets or sets the badge text to display (e.g. notifications).
    /// </summary>
    [ObservableProperty]
    private string? _badgeText;

    /// <summary>
    /// Gets or sets a value indicating whether to show the badge.
    /// </summary>
    [ObservableProperty]
    private bool _showBadge;

    /// <summary>
    /// Gets or sets any additional data associated with this item.
    /// </summary>
    [ObservableProperty]
    private object? _tag;

    /// <summary>
    /// Gets or sets the child items for hierarchical navigation.
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<NavigationItem> _childItems = [];

    /// <summary>
    /// Gets or sets a value indicating whether this hierarchical item is expanded in the UI.
    /// </summary>
    [ObservableProperty]
    private bool _isExpanded;

    /// <summary>
    /// Gets the factory function to create the content lazily.
    /// </summary>
    public Func<object>? ContentFactory { get; set; }

    /// <summary>
    /// Gets a value indicating whether this item has child items.
    /// </summary>
    public bool HasChildren => ChildItems.Count > 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationItem"/> class.
    /// </summary>
    /// <param name="id">The unique identifier.</param>
    /// <param name="label">The display label.</param>
    /// <param name="icon">The icon representation (e.g., font character code).</param>
    /// <param name="contentType">The type of the content view.</param>
    public NavigationItem(string id, string label, string icon, Type contentType)
    {
        _id = id;
        _label = label;
        _icon = icon;
        _contentType = contentType;

        _childItems.CollectionChanged += ChildItems_CollectionChanged;
    }

    // Constructor overload if contentType is not always required initially
    public NavigationItem(string id, string label, string icon)
    {
        _id = id;
        _label = label;
        _icon = icon;
        _contentType = null;

        _childItems.CollectionChanged += ChildItems_CollectionChanged;
    }

    private void ChildItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        // Notify that the HasChildren property might have changed
        OnPropertyChanged(nameof(HasChildren));
    }

    public bool Equals(string? id) => Id == id;

    public override bool Equals(object? obj)
    {
        if (obj is NavigationItem other)
        {
            return string.Equals(Id, other.Id, StringComparison.Ordinal);
        }
        return false;
    }

    public override int GetHashCode() => Id?.GetHashCode() ?? 0;
}
