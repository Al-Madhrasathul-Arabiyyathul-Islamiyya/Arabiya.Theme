// NavigationGuardPipeline.cs
using Arabiyya.Theme.Navigation.Models;

namespace Arabiyya.Theme.Navigation.Services;

/// <summary>
/// Manages a pipeline of navigation guards
/// </summary>
public class NavigationGuardPipeline
{
    private readonly List<INavigationGuard> _guards = [];

    /// <summary>
    /// Adds a guard to the pipeline
    /// </summary>
    public void AddGuard(INavigationGuard guard)
    {
        ArgumentNullException.ThrowIfNull(guard);
        _guards.Add(guard);
    }

    /// <summary>
    /// Removes a guard from the pipeline
    /// </summary>
    public void RemoveGuard(INavigationGuard guard)
    {
        if (guard != null)
        {
            _guards.Remove(guard);
        }
    }

    /// <summary>
    /// Executes all guards in the pipeline
    /// </summary>
    public async Task<NavigationGuardResult> ExecuteAsync(NavigationItem source, NavigationItem target)
    {
        foreach (var guard in _guards)
        {
            var result = await guard.CheckAccessAsync(source, target);
            if (!result.IsAllowed)
            {
                return result;
            }
        }

        return NavigationGuardResult.Allow();
    }
}
