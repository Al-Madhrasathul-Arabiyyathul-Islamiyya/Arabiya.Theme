using Avalonia.Controls;

namespace Arabiyya.Theme.Navigation.Services;
/// <summary>
/// Default implementation of IViewFactory that creates and caches view instances
/// </summary>
public class DefaultViewFactory : IViewFactory
{
    private readonly Dictionary<string, object> _viewCache = [];
    private readonly bool _cacheViews;

    /// <summary>
    /// Creates a new instance of DefaultViewFactory
    /// </summary>
    /// <param name="cacheViews">Whether to cache created views</param>
    public DefaultViewFactory(bool cacheViews = true)
    {
        _cacheViews = cacheViews;
    }

    /// <summary>
    /// Creates or retrieves a view instance for the specified view type
    /// </summary>
    /// <param name="viewId">The unique identifier for the view</param>
    /// <param name="viewType">The type of view to create</param>
    /// <returns>The view instance</returns>
    public object GetView(string viewId, Type viewType)
    {
        System.Diagnostics.Debug.WriteLine($"DefaultViewFactory.GetView called for {viewType.Name}");
        if (_cacheViews && _viewCache.TryGetValue(viewId, out object? cachedView))
        {
            return cachedView;
        }

        object? view = Activator.CreateInstance(viewType);

        if (_cacheViews)
        {
            _viewCache[viewId] = view!;
        }

        return view!;
    }

    /// <summary>
    /// Creates or retrieves a view instance using a factory function
    /// </summary>
    /// <param name="viewId">The unique identifier for the view</param>
    /// <param name="factory">The factory function to create the view</param>
    /// <returns>The view instance</returns>
    public object GetView(string viewId, Func<object> factory)
    {
        if (_cacheViews && _viewCache.TryGetValue(viewId, out object? cachedView))
        {
            return cachedView;
        }

        object view = factory();

        if (_cacheViews)
        {
            _viewCache[viewId] = view;
        }

        return view;
    }

    /// <summary>
    /// Releases a view instance when it's no longer needed
    /// </summary>
    /// <param name="viewId">The unique identifier for the view</param>
    public void ReleaseView(string viewId)
    {
        if (_viewCache.TryGetValue(viewId, out object? view))
        {
            // Dispose the view if it implements IDisposable
            if (view is IDisposable disposable)
            {
                disposable.Dispose();
            }

            // Handle Avalonia-specific cleanup
            if (view is Control control)
            {
                control.DataContext = null;
            }

            _viewCache.Remove(viewId);
        }
    }
}
