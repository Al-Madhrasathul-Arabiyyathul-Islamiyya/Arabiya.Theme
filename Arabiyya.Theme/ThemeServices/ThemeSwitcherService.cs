using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.Styling;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Arabiyya.Theme.ThemeServices;

public class ThemeService : INotifyPropertyChanged
{
    private static ThemeService? _instance;
    private Window? _mainWindow;
    private bool _isDarkTheme;
    private bool _initWithSystemTheme = false;

    public event PropertyChangedEventHandler? PropertyChanged;

    public static ThemeService Instance => _instance ??= new ThemeService();

    public bool InitWithSystemTheme
    {
        get => _initWithSystemTheme;
        set => _initWithSystemTheme = value;
    }

    public bool IsDarkTheme
    {
        get => _isDarkTheme;
        set
        {
            if (_isDarkTheme != value)
            {
                _isDarkTheme = value;
                OnPropertyChanged();
                ApplyTheme(value);
            }
        }
    }

    private ThemeService() => _isDarkTheme = false;

    public void Initialize(Window window)
    {
        _mainWindow = window;

        if (_initWithSystemTheme)
        {
            if (Application.Current?.PlatformSettings != null)
            {
                Application.Current.PlatformSettings.ColorValuesChanged += (s, e) => DetectSystemTheme();

                DetectSystemTheme();
            }
            else
            {
                IsDarkTheme = false;
            }
        }
        else
        {
            ApplyTheme(IsDarkTheme);
        }
    }

    private void DetectSystemTheme()
    {
        if (Application.Current?.PlatformSettings != null)
        {
            var themeVariant = Application.Current.PlatformSettings.GetColorValues().ThemeVariant;
            IsDarkTheme = themeVariant == PlatformThemeVariant.Dark;
        }
    }

    private void ApplyTheme(bool isDark)
    {
        if (_mainWindow == null)
            return;

        if (Application.Current != null)
        {
            Application.Current.RequestedThemeVariant = isDark ? ThemeVariant.Dark : ThemeVariant.Light;
        }

        _mainWindow.RequestedThemeVariant = isDark ? ThemeVariant.Dark : ThemeVariant.Light;
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
