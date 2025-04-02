using Microsoft.Extensions.DependencyInjection;

namespace Arabiyya.Theme.Navigation.Services;

/// <summary>
/// IViewFactory implementation that resolves views from a dependency injection container
/// </summary>
public class DependencyInjectionViewFactory : IViewFactory
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Creates a new instance of DependencyInjectionViewFactory
    /// </summary>
    /// <param name="serviceProvider">The service provider to resolve views from</param>
    public DependencyInjectionViewFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <summary>
    /// Creates or retrieves a view instance for the specified view type
    /// </summary>
    /// <param name="viewId">The unique identifier for the view</param>
    /// <param name="viewType">The type of view to create</param>
    /// <returns>The view instance</returns>
    public object GetView(string viewId, Type viewType)
    {
        System.Diagnostics.Debug.WriteLine($"DependencyInjectionViewFactory.GetView called for {viewType.Name}");
        return _serviceProvider.GetService(viewType) ?? Activator.CreateInstance(viewType)!;
    }
        // Try to resolve the view from the service provider

    /// <summary>
    /// Creates or retrieves a view instance using a factory function
    /// </summary>
    /// <param name="viewId">The unique identifier for the view</param>
    /// <param name="factory">The factory function to create the view</param>
    /// <returns>The view instance</returns>
    public object GetView(string viewId, Func<object> factory)
    {
        return factory();
    }

    /// <summary>
    /// Releases a view instance when it's no longer needed
    /// </summary>
    /// <param name="viewId">The unique identifier for the view</param>
    public void ReleaseView(string viewId)
    {
        // Nothing to do, as the DI container manages the lifetime
    }
}
