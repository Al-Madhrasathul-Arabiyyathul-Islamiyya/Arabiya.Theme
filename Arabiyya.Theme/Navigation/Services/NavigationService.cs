using Arabiyya.Theme.Navigation.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace Arabiyya.Theme.Navigation.Services;

/// <summary>
/// Default implementation of the INavigationService interface
/// </summary>
public partial class NavigationService : ObservableObject, INavigationService
{
    private readonly IViewFactory _viewFactory;
    private readonly IMessenger _messenger;
    private readonly List<INavigationGuard> _guards = new();
    private NavigationItem _selectedItem;
    private NavigationConfig _config;

    /// <summary>
    /// Gets the current navigation configuration
    /// </summary>
    public NavigationConfig Config => _config;

    /// <summary>
    /// Gets the collection of navigation items
    /// </summary>
    public IReadOnlyList<NavigationItem>? Items => _config.Items.ToList().AsReadOnly();

    /// <summary>
    /// Gets the currently selected navigation item
    /// </summary>
    public NavigationItem SelectedItem
    {
        get => _selectedItem;
        private set => SetProperty(ref _selectedItem, value);
    }

    /// <summary>
    /// Gets the current content being displayed
    /// </summary>
    [ObservableProperty]
    private object? _currentContent;

    /// <summary>
    /// Raised when navigation is about to occur
    /// </summary>
    public event EventHandler<NavigatingEventArgs> Navigating;

    /// <summary>
    /// Raised when navigation has been completed
    /// </summary>
    public event EventHandler<NavigatedEventArgs> Navigated;

    /// <summary>
    /// Creates a new instance of NavigationService with a default view factory
    /// </summary>
    public NavigationService() : this(new DefaultViewFactory(), StrongReferenceMessenger.Default)
    {
    }

    /// <summary>
    /// Creates a new instance of NavigationService with the specified view factory
    /// </summary>
    /// <param name="viewFactory">The view factory to use</param>
    /// <param name="messenger">The messenger service to use</param>
    public NavigationService(IViewFactory viewFactory, IMessenger messenger)
    {
        _viewFactory = viewFactory ?? throw new ArgumentNullException(nameof(viewFactory));
        _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
    }

    /// <summary>
    /// Initializes the navigation service with the specified configuration
    /// </summary>
    /// <param name="config">The navigation configuration</param>
    public void Initialize(NavigationConfig config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));

        // Initialize with the first item if available
        if (config.Items.Count > 0)
        {
            string? selectedItemId = config.SelectedItemId;

            if (!string.IsNullOrEmpty(selectedItemId))
            {
                // Try to select the specified item
                var item = config.Items.FirstOrDefault(i => i.Id == selectedItemId);
                if (item != null)
                {
                    _ = NavigateToAsync(item);
                    return;
                }
            }

            // Default to the first item
            _ = NavigateToAsync(config.Items[0]);
        }
    }

    /// <summary>
    /// Navigates to the specified navigation item
    /// </summary>
    /// <param name="item">The navigation item to navigate to</param>
    /// <returns>True if navigation was successful, false otherwise</returns>
    public async Task<bool> NavigateToAsync(NavigationItem item)
    {
        if (item == null)
            return false;

        // Check if the item is enabled
        if (!item.IsEnabled)
            return false;

        // Raise the Navigating event
        var args = new NavigatingEventArgs(SelectedItem, item);
        Navigating?.Invoke(this, args);

        // Check if navigation was canceled
        if (args.Cancel)
            return false;

        // Check navigation guards
        if (_guards.Count > 0)
        {
            foreach (var guard in _guards)
            {
                var result = await guard.CheckAccessAsync(SelectedItem, item);
                if (!result.IsAllowed)
                {
                    // If a redirect is specified, navigate to it
                    if (!string.IsNullOrEmpty(result.RedirectPath))
                    {
                        await NavigateToPathAsync(result.RedirectPath);
                    }

                    // Send a message about the guard rejection
                    _messenger.Send(new NavigationGuardRejectedMessage(SelectedItem, item, result));

                    return false;
                }
            }
        }

        // Update the selected state of all items
        foreach (var navItem in Items!)
        {
            navItem.IsSelected = navItem == item;
        }

        // Update the selected item
        SelectedItem = item;
        _config.SelectedItemId = item.Id;

        // Create or get the content if needed
        if (item.Content == null)
        {
            try
            {
                if (item.ContentFactory != null)
                {
                    // Use the factory function if provided
                    item.Content = _viewFactory.GetView(item.Id, item.ContentFactory);
                }
                else if (item.ContentType != null)
                {
                    // Use the content type if provided
                    item.Content = _viewFactory.GetView(item.Id, item.ContentType);
                }
            }
            catch (Exception ex)
            {
                // Log the error
                System.Diagnostics.Debug.WriteLine($"Error creating content for navigation item: {ex.Message}");
                return false;
            }
        }

        if (item.Content == null)
            { return false; }

        // Update the current content
        CurrentContent = item.Content;

        // Raise the Navigated event
        Navigated?.Invoke(this, new NavigatedEventArgs(item, CurrentContent));

        // Send a message that navigation has completed
        _messenger.Send(new NavigationCompletedMessage(item, CurrentContent));

        return true;
    }

    /// <summary>
    /// Navigates to the item with the specified ID
    /// </summary>
    /// <param name="itemId">The ID of the item to navigate to</param>
    /// <returns>True if navigation was successful, false otherwise</returns>
    public Task<bool> NavigateToAsync(string itemId)
    {
        var item = Items?.FirstOrDefault(i => i.Id == itemId);
        return item != null
            ? NavigateToAsync(item)
            : Task.FromResult(false);
    }

    /// <summary>
    /// Navigates to the specified path
    /// </summary>
    /// <param name="path">The path to navigate to</param>
    /// <returns>True if navigation was successful, false otherwise</returns>
    public Task<bool> NavigateToPathAsync(string path)
    {
        if (string.IsNullOrEmpty(path))
            return Task.FromResult(false);

        // Normalize the path
        path = path.TrimStart('/');

        // Find the item with matching path
        var item = Items?.FirstOrDefault(i => i.Path == path);

        // If found, navigate to it
        if (item != null)
        {
            return NavigateToAsync(item);
        }

        // If not found, check if any item has a matching ID
        // (for backward compatibility and simple cases)
        return NavigateToAsync(path);
    }

    /// <summary>
    /// Adds a new navigation item
    /// </summary>
    /// <param name="item">The item to add</param>
    public void AddItem(NavigationItem item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        _config?.Items.Add(item);
    }

    /// <summary>
    /// Removes a navigation item
    /// </summary>
    /// <param name="item">The item to remove</param>
    /// <returns>True if the item was removed, false otherwise</returns>
    public bool RemoveItem(NavigationItem item)
    {
        if (item == null || _config?.Items == null)
            return false;

        // If removing the currently selected item, try to select another one
        if (item == SelectedItem && _config.Items.Count > 1)
        {
            int index = _config.Items.IndexOf(item);
            var nextItem = index < _config.Items.Count - 1
                ? _config.Items[index + 1]
                : _config.Items[index - 1];

            // Navigate to the next item async
            _ = NavigateToAsync(nextItem);
        }

        return _config.Items.Remove(item);
    }

    /// <summary>
    /// Removes the navigation item with the specified ID
    /// </summary>
    /// <param name="itemId">The ID of the item to remove</param>
    /// <returns>True if the item was removed, false otherwise</returns>
    public bool RemoveItem(string itemId)
    {
        if (string.IsNullOrEmpty(itemId) || _config?.Items == null)
            return false;

        var item = _config.Items.FirstOrDefault(i => i.Id == itemId);
        return item != null && RemoveItem(item);
    }

    /// <summary>
    /// Clears all navigation items
    /// </summary>
    public void ClearItems()
    {
        if (_config?.Items != null)
        {
            // Release all views
            foreach (var item in _config.Items)
            {
                _viewFactory.ReleaseView(item.Id!);
            }

            _config.Items.Clear();
            SelectedItem = null;
            CurrentContent = null;
        }
    }

    /// <summary>
    /// Toggles the expanded state of the navigation panel
    /// </summary>
    public void ToggleExpanded()
    {
        if (_config != null)
        {
            _config.IsExpanded = !_config.IsExpanded;
        }
    }

    /// <summary>
    /// Adds a navigation guard to the service
    /// </summary>
    /// <param name="guard">The guard to add</param>
    public void AddGuard(INavigationGuard guard)
    {
        ArgumentNullException.ThrowIfNull(guard);

        _guards.Add(guard);
    }

    /// <summary>
    /// Removes a navigation guard from the service
    /// </summary>
    /// <param name="guard">The guard to remove</param>
    public void RemoveGuard(INavigationGuard guard)
    {
        if (guard != null)
        {
            _guards.Remove(guard);
        }
    }
}

/// <summary>
/// Message sent when navigation has completed
/// </summary>
public record NavigationCompletedMessage(NavigationItem Item, object Content);

/// <summary>
/// Message sent when a navigation guard rejects navigation
/// </summary>
public record NavigationGuardRejectedMessage(
    NavigationItem SourceItem,
    NavigationItem TargetItem,
    NavigationGuardResult Result);
