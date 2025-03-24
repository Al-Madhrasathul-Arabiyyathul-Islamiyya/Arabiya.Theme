using Arabiyya.Theme.Demo.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Arabiyya.Theme.Demo.Views;

public partial class InputsView : UserControl
{
    public InputsView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        // Get the view model and call its OnLoaded method
        if (DataContext is InputsViewModel viewModel)
        {
            _ = InputsViewModel.OnLoaded(this);
        }
    }
}
