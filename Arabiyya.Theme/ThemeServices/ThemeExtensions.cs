using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;

namespace Arabiyya.Theme.ThemeServices;

public static class ThemeExtensions
{
    public static AppBuilder UseArabiyyaTheme(this AppBuilder builder, bool? isDarkTheme = null)
    {
        if (isDarkTheme.HasValue)
        {
            // Use explicitly provided theme
            ThemeService.Instance.IsDarkTheme = isDarkTheme.Value;

            if (Application.Current != null)
            {
                Application.Current.RequestedThemeVariant = isDarkTheme.Value ? ThemeVariant.Dark : ThemeVariant.Light;
            }
        }
        else
        {
            // Follow system theme
            ThemeService.Instance.InitWithSystemTheme = true;

            if (Application.Current != null)
            {
                // This tells Avalonia to use the system theme
                Application.Current.RequestedThemeVariant = ThemeVariant.Default;
            }
        }

        return builder;
    }

    public static void InitializeTheme(this Window window)
    {
        ThemeService.Instance.Initialize(window);
    }

    public static void SetTheme(this Application app, bool isDarkTheme) => ThemeService.Instance.IsDarkTheme = isDarkTheme;
}
