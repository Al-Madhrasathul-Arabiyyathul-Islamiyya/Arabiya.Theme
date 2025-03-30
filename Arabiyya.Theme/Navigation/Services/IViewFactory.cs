namespace Arabiyya.Theme.Navigation.Services;

/// <summary>
/// Interface for a factory that creates or retrieves view instances
/// </summary>
public interface IViewFactory
{
    /// <summary>
    /// Creates or retrieves a view instance for the specified view type
    /// </summary>
    /// <param name="viewId">The unique identifier for the view</param>
    /// <param name="viewType">The type of view to create</param>
    /// <returns>The view instance</returns>
    object GetView(string viewId, Type viewType);

    /// <summary>
    /// Creates or retrieves a view instance using a factory function
    /// </summary>
    /// <param name="viewId">The unique identifier for the view</param>
    /// <param name="factory">The factory function to create the view</param>
    /// <returns>The view instance</returns>
    object GetView(string viewId, Func<object> factory);

    /// <summary>
    /// Releases a view instance when it's no longer needed
    /// </summary>
    /// <param name="viewId">The unique identifier for the view</param>
    void ReleaseView(string viewId);
}
