using Arabiyya.Theme.Navigation.Services;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Arabiyya.Theme.Navigation.Extensions;

/// <summary>
/// Extension methods for registering navigation services with DI
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds navigation services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddNavigation(this IServiceCollection services)
    {
        // Register the default view factory
        services.AddSingleton<IViewFactory, DefaultViewFactory>();

        // Register the navigation service
        services.AddSingleton<INavigationService, NavigationService>();

        return services;
    }

    /// <summary>
    /// Adds navigation services with a DI-aware view factory
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddNavigationWithDI(this IServiceCollection services)
    {
        services.AddSingleton<IMessenger>(StrongReferenceMessenger.Default);

        services.AddSingleton<IViewFactory>(sp => new DependencyInjectionViewFactory(sp));

        services.AddSingleton<INavigationService>(sp =>
        {
            var viewFactory = sp.GetRequiredService<IViewFactory>();
            var messenger = sp.GetRequiredService<IMessenger>();

            System.Diagnostics.Debug.WriteLine($"Creating NavigationService with {viewFactory.GetType().Name}");
            return new NavigationService(viewFactory, messenger);
        });

        return services;
    }

    /// <summary>
    /// Adds a navigation guard to the service collection
    /// </summary>
    /// <typeparam name="TGuard">The type of guard to add</typeparam>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddNavigationGuard<TGuard>(this IServiceCollection services)
        where TGuard : class, INavigationGuard
    {
        services.AddSingleton<INavigationGuard, TGuard>();
        return services;
    }
}
