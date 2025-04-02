using Arabiyya.Theme.Navigation.Services;

namespace Arabiyya.Theme.Navigation.Extensions;

/// <summary>
/// Extension methods for IViewFactory that provide generic type-safe access
/// </summary>
public static class ViewFactoryExtensions
{
    /// <summary>
    /// Creates or retrieves a view instance for the specified view type with generic type safety
    /// </summary>
    /// <typeparam name="T">The type of view to create</typeparam>
    /// <param name="factory">The view factory</param>
    /// <param name="viewId">The unique identifier for the view</param>
    /// <returns>The view instance as the specified type</returns>
    public static T GetView<T>(this IViewFactory factory, string viewId) where T : class
    {
        return (T)factory.GetView(viewId, typeof(T));
    }

    /// <summary>
    /// Creates or retrieves a view instance using a factory function with generic type safety
    /// </summary>
    /// <typeparam name="T">The type of view to create</typeparam>
    /// <param name="factory">The view factory</param>
    /// <param name="viewId">The unique identifier for the view</param>
    /// <param name="viewFactory">The factory function to create the view</param>
    /// <returns>The view instance as the specified type</returns>
    public static T GetView<T>(this IViewFactory factory, string viewId, Func<T> viewFactory) where T : class
    {
        return (T)factory.GetView(viewId, () => viewFactory());
    }
}
