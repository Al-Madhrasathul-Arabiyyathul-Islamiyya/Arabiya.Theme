using Arabiyya.Theme.Navigation.Controls;
using Arabiyya.Theme.Navigation.Models;
using Arabiyya.Theme.Navigation.Services;
using Avalonia;
using Avalonia.Controls;

namespace Arabiyya.Theme.Navigation.Extensions;

/// <summary>
/// Extension methods for adding navigation to windows and controls
/// </summary>
public static class NavigationExtensions
{
    /// <summary>
    /// Adds side navigation to a window with the specified configuration
    /// </summary>
    /// <param name="window">The window to add navigation to</param>
    /// <param name="navigationService">The navigation service</param>
    /// <param name="position">The sidebar position</param>
    /// <returns>The content control for navigation content</returns>
    public static NavigationHost AddSideNavigation(
        this Window window,
        INavigationService navigationService,
        SidebarPosition position = SidebarPosition.Left)
    {
        if (window == null)
            throw new ArgumentNullException(nameof(window));
        if (navigationService == null)
            throw new ArgumentNullException(nameof(navigationService));

        // Create a grid with two columns
        var grid = new Grid();
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

        // Create the side navigation
        var sideNav = new SideNav
        {
            DataContext = navigationService.Config,
            NavigationService = navigationService
        };

        // Create the content host
        var contentHost = new NavigationHost
        {
            NavigationService = navigationService,
            Margin = new Thickness(10)
        };

        // Add the controls to the grid based on position
        if (position == SidebarPosition.Left)
        {
            Grid.SetColumn(sideNav, 0);
            Grid.SetColumn(contentHost, 1);
        }
        else
        {
            grid.ColumnDefinitions.Clear();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            Grid.SetColumn(sideNav, 1);
            Grid.SetColumn(contentHost, 0);
        }

        // Add the controls to the grid
        grid.Children.Add(sideNav);
        grid.Children.Add(contentHost);

        // Set the grid as the window content
        window.Content = grid;

        // Set the sidebar position in the configuration
        navigationService.Config.SidebarPosition = position;

        return contentHost;
    }
}
