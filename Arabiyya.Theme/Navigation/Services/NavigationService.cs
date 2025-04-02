using System.Collections.ObjectModel;
using Arabiyya.Theme.Navigation.Models;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace Arabiyya.Theme.Navigation.Services;

/// <summary>
/// Default implementation of the <see cref="INavigationService"/> interface.
/// Handles view creation, navigation logic, history management, and guard execution.
/// </summary>
public partial class NavigationService : ObservableObject, INavigationService
{
    private readonly IViewFactory _viewFactory;
    private readonly IMessenger _messenger;
    private readonly NavigationGuardPipeline _guardPipeline = new();
    private readonly Stack<NavigationItem> _backStack = new();
    private readonly Stack<NavigationItem> _forwardStack = new();

    [ObservableProperty]
    private NavigationConfig _config = new();

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(NavigateCommand))]
    [NotifyCanExecuteChangedFor(nameof(GoBackCommand))]
    [NotifyCanExecuteChangedFor(nameof(GoForwardCommand))]
    private NavigationItem? _selectedItem;

    [ObservableProperty]
    private object? _currentContent;

    /// <inheritdoc/>
    public IReadOnlyList<NavigationItem>? Items => Config?.Items?.ToList()?.AsReadOnly();

    /// <inheritdoc/>
    [RelayCommand(CanExecute = nameof(CanNavigate))]
    private async Task NavigateAsyncCmd(NavigationItem? item) => await NavigateToAsyncItem(item);

    // Explicit interface implementation for clarity if using source generator for the public command
    /// <inheritdoc/>
    public IRelayCommand<NavigationItem?> NavigateCommand => NavigateAsyncCmdCommand;


    /// <inheritdoc/>
    [RelayCommand(CanExecute = nameof(CanGoBack))]
    public async Task GoBackAsync()
    {
        if (!CanGoBack)
            return;

        var item = _backStack.Pop();
        if (SelectedItem != null)
        {
            _forwardStack.Push(SelectedItem);
        }
        await NavigateToAsyncInternal(item, true);
        OnPropertyChanged(nameof(CanGoBack));
        OnPropertyChanged(nameof(CanGoForward));
    }

    /// <inheritdoc/>
    [RelayCommand(CanExecute = nameof(CanGoForward))]
    public async Task GoForwardAsync()
    {
        if (!CanGoForward)
            return;

        var item = _forwardStack.Pop();
        if (SelectedItem != null)
        {
            _backStack.Push(SelectedItem);
        }
        await NavigateToAsyncInternal(item, true);
        OnPropertyChanged(nameof(CanGoBack));
        OnPropertyChanged(nameof(CanGoForward));
    }

    /// <inheritdoc/>
    public bool CanGoBack => _backStack.Count > 0;

    /// <inheritdoc/>
    public bool CanGoForward => _forwardStack.Count > 0;

    /// <inheritdoc/>
    public event EventHandler<NavigatingEventArgs>? Navigating;

    /// <inheritdoc/>
    public event EventHandler<NavigatedEventArgs>? Navigated;

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationService"/> class with default dependencies.
    /// </summary>
    public NavigationService() : this(new DefaultViewFactory(), StrongReferenceMessenger.Default)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationService"/> class with specified dependencies.
    /// </summary>
    /// <param name="viewFactory">The factory used to create view instances.</param>
    /// <param name="messenger">The messenger for sending navigation-related messages.</param>
    public NavigationService(IViewFactory viewFactory, IMessenger messenger)
    {
        _viewFactory = viewFactory ?? throw new ArgumentNullException(nameof(viewFactory));
        _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
    }

    /// <inheritdoc/>
    public void Initialize(NavigationConfig config)
    {
        ArgumentNullException.ThrowIfNull(config);
        Config = config;

        _backStack.Clear();
        _forwardStack.Clear();
        SelectedItem = null;
        CurrentContent = null;

        // Initialize with the first navigable item or the one specified in config
        NavigationItem? initialItem = null;
        if (!string.IsNullOrEmpty(Config.SelectedItemId))
        {
            initialItem = Config.Items.FirstOrDefault(i => i.Id == Config.SelectedItemId && i.IsEnabled);
        }

        initialItem ??= Config.Items.FirstOrDefault(i => i.IsEnabled);

        if (initialItem != null)
        {
            // Initial navigation should be synchronous or handled carefully
            // to avoid race conditions if Initialize is called frequently.
            // Using await directly here assumes Initialize isn't called rapidly on UI thread.
            _ = NavigateToAsyncInternal(initialItem, skipHistory: true);
        }

        GoBackCommand.NotifyCanExecuteChanged();
        GoForwardCommand.NotifyCanExecuteChanged();
    }

    /// <inheritdoc/>
    public Task<bool> ParseRouteAsync(string url)
    {
        if (string.IsNullOrEmpty(url))
            return Task.FromResult(false);

        string path = url.Contains("://") ? new Uri(url).AbsolutePath : url;
        path = path.TrimStart('/');
        string[] segments = path.Split('/');

        if (segments.Length == 0)
            return Task.FromResult(false);

        // Find item matching the first segment (by Path or ID)
        var item = FindItemByPathOrId(Config.Items, segments[0]);

        // TODO: Handle deeper segments (segments[1], etc.) for nested navigation if required.

        return item != null ? NavigateToAsync(item) : Task.FromResult(false);
    }


    // Public NavigateToAsync method (called by commands, external code)
    /// <inheritdoc/>
    public async Task<bool> NavigateToAsync(NavigationItem item, bool skipHistory = false)
    {
        return await NavigateToAsyncInternal(item, skipHistory);
    }

    private async Task<bool> NavigateToAsyncItem(NavigationItem? item)
    {
        return item != null && await NavigateToAsyncInternal(item, false);
    }

    /// <inheritdoc/>
    public Task<bool> NavigateToAsync(string itemId)
    {
        var item = FindItemById(Config.Items, itemId);
        return item != null ? NavigateToAsync(item) : Task.FromResult(false);
    }

    /// <inheritdoc/>
    public Task<bool> NavigateToPathAsync(string path)
    {
        if (string.IsNullOrEmpty(path))
            return Task.FromResult(false);
        path = path.TrimStart('/');
        var item = FindItemByPath(Config.Items, path);
        return item != null ? NavigateToAsync(item) : NavigateToAsync(path);
    }

    // Central internal navigation logic
    private async Task<bool> NavigateToAsyncInternal(NavigationItem item, bool skipHistory)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (item.Equals(Config.SelectedItemId))
        {
            System.Diagnostics.Debug.WriteLine($"NavigationService: Skipped navigation to already selected item: {item.Label}");
            return true; 
        }

        // Pass SelectedItem (which can be null) to guards
        var navigatingArgs = new NavigatingEventArgs(SelectedItem!, item);
        Navigating?.Invoke(this, navigatingArgs);
        if (navigatingArgs.Cancel)
        { return false; }

        // Pass SelectedItem (which can be null) to guards
        var guardResult = await _guardPipeline.ExecuteAsync(SelectedItem!, item);
        if (!guardResult.IsAllowed)
        { return false; }

        // --- History Management ---
        if (!skipHistory && SelectedItem != null)
        {
            _backStack.Push(SelectedItem);
            _forwardStack.Clear();
        }

        var previousSelectedItem = SelectedItem;
        SelectedItem = item;
        Config.SelectedItemId = item.Id;

        UpdateSelectionState(Config.Items, item);

        // --- Content Loading ---
        if (item.Content == null)
        {
            System.Diagnostics.Debug.WriteLine($"Creating content for {item.Label} with ID {item.Id}");

            try
            {
                if (item.ContentFactory != null)
                {
                    System.Diagnostics.Debug.WriteLine("Using ContentFactory");
                    item.Content = item.ContentFactory();
                    System.Diagnostics.Debug.WriteLine($"ContentFactory result: {item.Content?.GetType().Name ?? "null"}");
                }
                else if (item.ContentType != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Using ContentType: {item.ContentType.Name}");
                    object view = _viewFactory.GetView(item.Id!, item.ContentType);
                    System.Diagnostics.Debug.WriteLine($"View created: {view?.GetType().Name ?? "null"}");
                    item.Content = view;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("WARNING: No content source available");
                    item.Content = new TextBlock { Text = $"No content defined for {item.Label}" };
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating content: {ex}");
            }
        }

        if (item.Content == null)
        {
            SelectedItem = null;
            return false;
        }

        CurrentContent = item.Content;

        // --- Completion ---
        Navigated?.Invoke(this, new NavigatedEventArgs(item, CurrentContent!));
        _messenger.Send(new NavigationCompletedMessage(item, CurrentContent!));
        System.Diagnostics.Debug.WriteLine($"NavigationService: Navigation successful to: {item.Label}");

        // Explicitly notify CanExecute changes after state modification if needed,
        // although [NotifyCanExecuteChangedFor] on SelectedItem handles most cases.
        GoBackCommand.NotifyCanExecuteChanged();
        GoForwardCommand.NotifyCanExecuteChanged();

        return true;
    }

    /// <inheritdoc/>
    public void AddItem(NavigationItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        Config?.Items.Add(item);
        OnPropertyChanged(nameof(Items)); 
    }

    /// <inheritdoc/>
    public bool RemoveItem(NavigationItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (Config?.Items == null)
            return false;

        bool removed = Config.Items.Remove(item);
        if (removed)
        {
            OnPropertyChanged(nameof(Items));

            // Handle removing from history stacks
            RemoveFromStack(_backStack, item);
            RemoveFromStack(_forwardStack, item);
            OnPropertyChanged(nameof(CanGoBack));
            OnPropertyChanged(nameof(CanGoForward));

            if (item.Equals(Config.SelectedItemId))
            {
                NavigationItem? nextItem = _backStack.Count > 0 ? _backStack.Peek() : Config.Items.FirstOrDefault(i => i.IsEnabled);
                if (nextItem != null)
                {
                    if (_backStack.Count > 0 && nextItem == _backStack.Peek())
                        _backStack.Pop();
                    _ = NavigateToAsyncInternal(nextItem, true);
                }
                else
                {
                    SelectedItem = null;
                    CurrentContent = null;
                }
            }
        }
        return removed;
    }

    /// <inheritdoc/>
    public bool RemoveItem(string itemId)
    {
        var item = FindItemById(Config.Items, itemId);
        return item != null && RemoveItem(item);
    }

    /// <inheritdoc/>
    public void ClearItems()
    {
        if (Config?.Items != null)
        {
            foreach (var item in Config.Items)
            {
                if (item.Id != null)
                    _viewFactory.ReleaseView(item.Id);
            }
            Config.Items.Clear();
            OnPropertyChanged(nameof(Items));
        }
        _backStack.Clear();
        _forwardStack.Clear();
        SelectedItem = null;
        CurrentContent = null;
        OnPropertyChanged(nameof(CanGoBack));
        OnPropertyChanged(nameof(CanGoForward));
    }

    /// <inheritdoc/>
    public void ToggleExpanded()
    {
        if (Config != null)
        {
            Config.IsExpanded = !Config.IsExpanded;
        }
    }

    /// <inheritdoc/>
    public void AddGuard(INavigationGuard guard) => _guardPipeline.AddGuard(guard);

    /// <inheritdoc/>
    public void RemoveGuard(INavigationGuard guard) => _guardPipeline.RemoveGuard(guard);


    // --- Private Helper Methods ---

    private bool CanNavigate(NavigationItem? item) => item != null && item.IsEnabled && item.Id != Config.SelectedItemId;

    // Recursively find item by ID
    private static NavigationItem? FindItemById(IEnumerable<NavigationItem>? items, string id)
    {
        if (items == null || string.IsNullOrEmpty(id))
            return null;
        foreach (var item in items)
        {
            if (item.Id == id)
                return item;
            var foundInChild = FindItemById(item.ChildItems, id);
            if (foundInChild != null)
                return foundInChild;
        }
        return null;
    }

    // Recursively find item by Path
    private static NavigationItem? FindItemByPath(IEnumerable<NavigationItem>? items, string path)
    {
        if (items == null || string.IsNullOrEmpty(path))
            return null;
        foreach (var item in items)
        {
            if (string.Equals(item.Path, path, StringComparison.OrdinalIgnoreCase))
                return item;
            var foundInChild = FindItemByPath(item.ChildItems, path);
            if (foundInChild != null)
                return foundInChild;
        }
        return null;
    }

    // Recursively find item by Path or ID
    private static NavigationItem? FindItemByPathOrId(IEnumerable<NavigationItem>? items, string pathOrId)
    {
        if (items == null || string.IsNullOrEmpty(pathOrId))
            return null;
        foreach (var item in items)
        {
            if (string.Equals(item.Path, pathOrId, StringComparison.OrdinalIgnoreCase) || item.Id == pathOrId)
                return item;
            var foundInChild = FindItemByPathOrId(item.ChildItems, pathOrId);
            if (foundInChild != null)
                return foundInChild;
        }
        return null;
    }

    // Recursively update IsSelected state
    private static void UpdateSelectionState(IEnumerable<NavigationItem>? items, NavigationItem selected)
    {
        if (items == null || !items.Any())
            return;

        foreach (var item in items)
        {
            item.IsSelected = item.Equals(selected);

            if (item.ChildItems.Count > 0)
            {
                UpdateSelectionState(item.ChildItems, selected);
            }
        }
    }

    // Helper to remove an item from a history stack efficiently
    private static void RemoveFromStack(Stack<NavigationItem> stack, NavigationItem itemToRemove)
    {
        if (stack.Count == 0)
            return;

        var temp = new Stack<NavigationItem>(stack.Count);
        while (stack.Count > 0)
        {
            var item = stack.Pop();
            if (!item.Equals(itemToRemove))
            {
                temp.Push(item);
            }
        }
        while (temp.Count > 0)
        {
            stack.Push(temp.Pop());
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
