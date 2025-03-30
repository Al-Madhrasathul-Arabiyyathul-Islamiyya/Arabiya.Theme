using Arabiyya.Theme.Navigation.Models;

namespace Arabiyya.Theme.Navigation.Services;

/// <summary>
/// Interface for a navigation guard that controls access to navigation targets
/// </summary>
public interface INavigationGuard
{
    /// <summary>
    /// Checks if navigation from source to target is allowed
    /// </summary>
    /// <param name="source">The source navigation item (may be null for initial navigation)</param>
    /// <param name="target">The target navigation item</param>
    /// <returns>A result indicating whether navigation is allowed</returns>
    Task<NavigationGuardResult> CheckAccessAsync(NavigationItem source, NavigationItem target);
}

/// <summary>
/// Result of a navigation guard check
/// </summary>
public class NavigationGuardResult
{
    /// <summary>
    /// Gets a value indicating whether navigation is allowed
    /// </summary>
    public bool IsAllowed { get; }

    /// <summary>
    /// Gets the redirect path if navigation is not allowed
    /// </summary>
    public string? RedirectPath { get; }

    /// <summary>
    /// Gets any additional data related to the guard result
    /// </summary>
    public object? Data { get; }

    /// <summary>
    /// Creates a new instance of NavigationGuardResult
    /// </summary>
    public NavigationGuardResult(bool isAllowed, string? redirectPath = null, object? data = null)
    {
        IsAllowed = isAllowed;
        RedirectPath = redirectPath;
        Data = data;
    }

    /// <summary>
    /// Creates a result that allows navigation
    /// </summary>
    public static NavigationGuardResult Allow() => new(true);

    /// <summary>
    /// Creates a result that denies navigation
    /// </summary>
    public static NavigationGuardResult Deny() => new(false);

    /// <summary>
    /// Creates a result that redirects to another path
    /// </summary>
    public static NavigationGuardResult Redirect(string path) => new(false, path);
}

