using Arabiyya.Theme.Navigation.Models;

namespace Arabiyya.Theme.Navigation.Services;

/// <summary>
/// Interface defining the navigation service functionality
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// Gets the current navigation configuration
    /// </summary>
    NavigationConfig Config { get; }

    /// <summary>
    /// Gets the collection of navigation items
    /// </summary>
    IReadOnlyList<NavigationItem> Items { get; }

    /// <summary>
    /// Gets the currently selected navigation item
    /// </summary>
    NavigationItem SelectedItem { get; }

    /// <summary>
    /// Gets the current content being displayed
    /// </summary>
    object CurrentContent { get; }

    /// <summary>
    /// Raised when navigation is about to occur
    /// </summary>
    event EventHandler<NavigatingEventArgs> Navigating;

    /// <summary>
    /// Raised when navigation has been completed
    /// </summary>
    event EventHandler<NavigatedEventArgs> Navigated;

    /// <summary>
    /// Initializes the navigation service with the specified configuration
    /// </summary>
    /// <param name="config">The navigation configuration</param>
    void Initialize(NavigationConfig config);

    /// <summary>
    /// Navigates to the specified navigation item
    /// </summary>
    /// <param name="item">The navigation item to navigate to</param>
    /// <returns>True if navigation was successful, false otherwise</returns>
    Task<bool> NavigateToAsync(NavigationItem item);

    /// <summary>
    /// Navigates to the item with the specified ID
    /// </summary>
    /// <param name="itemId">The ID of the item to navigate to</param>
    /// <returns>True if navigation was successful, false otherwise</returns>
    Task<bool> NavigateToAsync(string itemId);

    /// <summary>
    /// Navigates to the specified path
    /// </summary>
    /// <param name="path">The path to navigate to</param>
    /// <returns>True if navigation was successful, false otherwise</returns>
    Task<bool> NavigateToPathAsync(string path);

    /// <summary>
    /// Adds a new navigation item
    /// </summary>
    /// <param name="item">The item to add</param>
    void AddItem(NavigationItem item);

    /// <summary>
    /// Removes a navigation item
    /// </summary>
    /// <param name="item">The item to remove</param>
    /// <returns>True if the item was removed, false otherwise</returns>
    bool RemoveItem(NavigationItem item);

    /// <summary>
    /// Removes the navigation item with the specified ID
    /// </summary>
    /// <param name="itemId">The ID of the item to remove</param>
    /// <returns>True if the item was removed, false otherwise</returns>
    bool RemoveItem(string itemId);

    /// <summary>
    /// Clears all navigation items
    /// </summary>
    void ClearItems();

    /// <summary>
    /// Toggles the expanded state of the navigation panel
    /// </summary>
    void ToggleExpanded();

    /// <summary>
    /// Adds a navigation guard to the service
    /// </summary>
    /// <param name="guard">The guard to add</param>
    void AddGuard(INavigationGuard guard);

    /// <summary>
    /// Removes a navigation guard from the service
    /// </summary>
    /// <param name="guard">The guard to remove</param>
    void RemoveGuard(INavigationGuard guard);
}

/// <summary>
/// Event arguments for the Navigating event
/// </summary>
public class NavigatingEventArgs : EventArgs
{
    /// <summary>
    /// Gets the source navigation item
    /// </summary>
    public NavigationItem SourceItem { get; }

    /// <summary>
    /// Gets the target navigation item
    /// </summary>
    public NavigationItem TargetItem { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the navigation should be canceled
    /// </summary>
    public bool Cancel { get; set; }

    /// <summary>
    /// Creates a new instance of NavigatingEventArgs
    /// </summary>
    /// <param name="sourceItem">The source navigation item</param>
    /// <param name="targetItem">The target navigation item</param>
    public NavigatingEventArgs(NavigationItem sourceItem, NavigationItem targetItem)
    {
        SourceItem = sourceItem;
        TargetItem = targetItem;
    }
}

/// <summary>
/// Event arguments for the Navigated event
/// </summary>
public class NavigatedEventArgs : EventArgs
{
    /// <summary>
    /// Gets the navigation item that was navigated to
    /// </summary>
    public NavigationItem Item { get; }

    /// <summary>
    /// Gets the content that was navigated to
    /// </summary>
    public object Content { get; }

    /// <summary>
    /// Creates a new instance of NavigatedEventArgs
    /// </summary>
    /// <param name="item">The navigation item that was navigated to</param>
    /// <param name="content">The content that was navigated to</param>
    public NavigatedEventArgs(NavigationItem item, object content)
    {
        Item = item;
        Content = content;
    }
}
