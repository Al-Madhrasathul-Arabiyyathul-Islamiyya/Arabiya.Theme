using Arabiyya.Theme.Navigation.Models;
using CommunityToolkit.Mvvm.Input;

namespace Arabiyya.Theme.Navigation.Services;

/// <summary>
/// Interface defining the navigation service functionality.
/// Manages navigation state, content loading, history, and guards.
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// Gets the current navigation configuration settings.
    /// </summary>
    NavigationConfig Config { get; }

    /// <summary>
    /// Gets the read-only list of top-level navigation items.
    /// </summary>
    IReadOnlyList<NavigationItem>? Items { get; }

    /// <summary>
    /// Gets the currently selected navigation item.
    /// </summary>
    NavigationItem? SelectedItem { get; }

    /// <summary>
    /// Gets the current content (View/ViewModel) being displayed by the navigation host.
    /// </summary>
    object? CurrentContent { get; }

    /// <summary>
    /// Gets a value indicating whether navigation back is possible.
    /// </summary>
    bool CanGoBack { get; }

    /// <summary>
    /// Gets a value indicating whether navigation forward is possible.
    /// </summary>
    bool CanGoForward { get; }

    /// <summary>
    /// Raised just before navigation occurs, allowing cancellation or redirection via guards.
    /// </summary>
    event EventHandler<NavigatingEventArgs>? Navigating;

    /// <summary>
    /// Raised after navigation to a new item has successfully completed.
    /// </summary>
    event EventHandler<NavigatedEventArgs>? Navigated;

    /// <summary>
    /// Gets the command to navigate to a specific NavigationItem.
    /// Parameter should be the NavigationItem to navigate to.
    /// </summary>
    IRelayCommand<NavigationItem?> NavigateCommand { get; }

    /// <summary>
    /// Gets the command to navigate back in the history stack.
    /// </summary>
    IAsyncRelayCommand GoBackCommand { get; }

    /// <summary>
    /// Gets the command to navigate forward in the history stack.
    /// </summary>
    IAsyncRelayCommand GoForwardCommand { get; }

    /// <summary>
    /// Initializes the navigation service with the specified configuration and potentially navigates to the initial item.
    /// </summary>
    /// <param name="config">The navigation configuration.</param>
    void Initialize(NavigationConfig config);

    /// <summary>
    /// Parses a URL/route string and navigates to the corresponding item if found.
    /// </summary>
    /// <param name="url">The URL or route path to parse.</param>
    /// <returns>A task representing the asynchronous operation, returning true if navigation occurred.</returns>
    Task<bool> ParseRouteAsync(string url);

    /// <summary>
    /// Navigates asynchronously to the specified navigation item.
    /// </summary>
    /// <param name="item">The navigation item to navigate to.</param>
    /// <param name="skipHistory">If true, this navigation will not be added to the history stack.</param>
    /// <returns>A task representing the asynchronous operation, returning true if navigation was successful.</returns>
    Task<bool> NavigateToAsync(NavigationItem item, bool skipHistory = false);

    /// <summary>
    /// Navigates asynchronously to the item with the specified ID.
    /// </summary>
    /// <param name="itemId">The unique ID of the item to navigate to.</param>
    /// <returns>A task representing the asynchronous operation, returning true if navigation was successful.</returns>
    Task<bool> NavigateToAsync(string itemId);

    /// <summary>
    /// Navigates asynchronously to the item matching the specified path.
    /// </summary>
    /// <param name="path">The path of the item to navigate to.</param>
    /// <returns>A task representing the asynchronous operation, returning true if navigation was successful.</returns>
    Task<bool> NavigateToPathAsync(string path);

    /// <summary>
    /// Adds a new navigation item dynamically.
    /// </summary>
    /// <param name="item">The item to add.</param>
    void AddItem(NavigationItem item);

    /// <summary>
    /// Removes a navigation item dynamically.
    /// </summary>
    /// <param name="item">The item to remove.</param>
    /// <returns>True if the item was found and removed, false otherwise.</returns>
    bool RemoveItem(NavigationItem item);

    /// <summary>
    /// Removes the navigation item with the specified ID dynamically.
    /// </summary>
    /// <param name="itemId">The ID of the item to remove.</param>
    /// <returns>True if the item was found and removed, false otherwise.</returns>
    bool RemoveItem(string itemId);

    /// <summary>
    /// Removes all navigation items and clears the history stack.
    /// </summary>
    void ClearItems();

    /// <summary>
    /// Toggles the expanded state of the navigation panel (if applicable).
    /// </summary>
    void ToggleExpanded();

    /// <summary>
    /// Adds a navigation guard to the pipeline. Guards are executed in the order they are added.
    /// </summary>
    /// <param name="guard">The guard instance to add.</param>
    void AddGuard(INavigationGuard guard);

    /// <summary>
    /// Removes a navigation guard from the pipeline.
    /// </summary>
    /// <param name="guard">The guard instance to remove.</param>
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
