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

        if (DataContext is InputsViewModel)
        {
            _ = InputsViewModel.OnLoaded(this);
        }
    }
}
