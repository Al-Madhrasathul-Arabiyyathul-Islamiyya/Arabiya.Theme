using Arabiyya.Theme.Demo.ViewModels;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace Arabiyya.Theme.Demo.Views
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            var serviceProvider = (ServiceProvider)Avalonia.Application.Current!.Resources["ServiceProvider"]!;

            DataContext = new MainWindowViewModel(serviceProvider!);
        }
    }
}
