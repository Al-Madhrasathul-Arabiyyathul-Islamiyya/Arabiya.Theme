using System;
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

            if (Design.IsDesignMode)
            {
                return;
            }

            var serviceProvider = (ServiceProvider)Avalonia.Application.Current!.Resources["ServiceProvider"]!;

            ArgumentNullException.ThrowIfNull(serviceProvider);

            DataContext = new MainWindowViewModel(serviceProvider);
        }
    }
}
