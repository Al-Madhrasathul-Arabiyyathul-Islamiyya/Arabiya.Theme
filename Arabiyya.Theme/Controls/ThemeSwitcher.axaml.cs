using System.ComponentModel;
using System.Windows.Input;
using Arabiyya.Theme.ThemeServices;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Arabiyya.Theme.Controls;

public enum ThemeSwitcherVariants
{
    Toggle,
    IconToggle,
    ComboBox,
    IconButton
}

public partial class ThemeSwitcher : UserControl, INotifyPropertyChanged
{
    private bool _isDarkTheme;
    private ThemeSwitcherVariants _variantType = ThemeSwitcherVariants.Toggle;

    public new event PropertyChangedEventHandler? PropertyChanged;

    public bool IsDarkTheme
    {
        get => _isDarkTheme;
        set
        {
            if (_isDarkTheme != value)
            {
                _isDarkTheme = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsDarkTheme)));
                // Apply theme change
                ThemeService.Instance.IsDarkTheme = value;
            }
        }
    }

    public ThemeSwitcherVariants VariantType
    {
        get => _variantType;
        set
        {
            if (_variantType != value)
            {
                _variantType = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(VariantType)));
            }
        }
    }

    public ICommand ToggleThemeCommand { get; }

    public ThemeSwitcher()
    {
        InitializeComponent();

        // Set initial state from service
        _isDarkTheme = ThemeService.Instance.IsDarkTheme;

        // Create toggle command for icon button using our DelegateCommand
        ToggleThemeCommand = new DelegateCommand(() => IsDarkTheme = !IsDarkTheme);

        // Subscribe to service changes
        ThemeService.Instance.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(ThemeService.IsDarkTheme))
            {
                IsDarkTheme = ThemeService.Instance.IsDarkTheme;
            }
        };

        DataContext = this;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        PropertyChanged?.Invoke(this, e);
    }
}

public class DelegateCommand : ICommand
{
    private readonly Action _execute;
    private readonly Func<bool>? _canExecute;

    public event EventHandler? CanExecuteChanged;

    public DelegateCommand(Action execute, Func<bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;

    public void Execute(object? parameter) => _execute();

    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
