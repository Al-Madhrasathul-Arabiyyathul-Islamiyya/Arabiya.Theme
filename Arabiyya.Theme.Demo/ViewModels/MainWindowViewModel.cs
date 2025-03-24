using Arabiyya.Theme.ThemeServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Arabiyya.Theme.Demo.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly Dictionary<int, object> _views;

    [ObservableProperty]
    private object? _currentView;

    [ObservableProperty]
    private int _selectedNavIndex;

    public MainViewModel()
    {
        _views = new Dictionary<int, object>
        {
            { 0, new ColorPaletteViewModel() },
            { 1, new TypographyViewModel() },
            { 2, new ButtonsViewModel() },
            { 3, new GlassPanelsViewModel() },
            { 4, new TextInputsViewModel() },
            { 5, new InputsViewModel() },
            { 6, new GradientsViewModel() },
        }!;

        SelectedNavIndex = 0;
        CurrentView = _views.GetValueOrDefault(SelectedNavIndex);
    }

    partial void OnSelectedNavIndexChanged(int value)
    {
        if (_views.TryGetValue(value, out var viewModel))
        {
            CurrentView = viewModel;
        }
    }
}
