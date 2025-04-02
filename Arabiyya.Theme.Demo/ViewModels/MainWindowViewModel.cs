using Arabiyya.Theme.Demo.Views;
using Arabiyya.Theme.Navigation.Models;
using Arabiyya.Theme.Navigation.Services;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace Arabiyya.Theme.Demo.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly INavigationService? _navigationService;

    public INavigationService? NavigationService => _navigationService;

    public MainWindowViewModel()
    {
        if (Design.IsDesignMode)
        {
            // Create a mock navigation service with sample data for design time
            var config = new NavigationConfig
            {
                Title = "Design Time Demo",
                NavigationMode = NavigationMode.Sidebar,
                UseGlassEffect = true,
                ShowIcons = true,
                AllowCollapse = true
            };

            // Add a few mock navigation items for design time
            config.Items.Add(new NavigationItem("colors", "Colors", "\uE2B1"));
            config.Items.Add(new NavigationItem("typography", "Typography", "\uE248"));

            // Create a simple navigation service for design time
            _navigationService = new NavigationService();
            _navigationService.Initialize(config);
        }
        else
        {
            throw new InvalidOperationException("ViewModel requires a ServiceProvider at runtime.");
        }
    }

    public MainWindowViewModel(ServiceProvider serviceProvider)
    {
        // Get the navigation service
        _navigationService = serviceProvider.GetRequiredService<INavigationService>();

        // Configure navigation
        var config = new NavigationConfig
        {
            Title = "Theme Demo",
            NavigationMode = NavigationMode.Sidebar,
            UseGlassEffect = true,
            ShowIcons = true,
            AllowCollapse = true
        };

        // Add navigation items
        config.Items.Add(new NavigationItem("colors", "Color Palette", "\uE6C8", typeof(ColorPaletteView)));
        config.Items.Add(new NavigationItem("typography", "Typography", "\uE6EE", typeof(TypographyView)));
        config.Items.Add(new NavigationItem("buttons", "Buttons", "\uE5A2", typeof(ButtonsView)));
        config.Items.Add(new NavigationItem("glasspanels", "Glass Panels", "\uE5DA", typeof(GlassPanelsView)));
        config.Items.Add(new NavigationItem("textinputs", "Text Inputs", "\uEB0A", typeof(TextInputsView)));
        config.Items.Add(new NavigationItem("inputs", "Other Inputs", "\uE644", typeof(InputsView)));
        config.Items.Add(new NavigationItem("tabs", "Tab Controls", "\uEE4E", typeof(TabControlView)));
        config.Items.Add(new NavigationItem("gradients", "Gradients", "\uEB42", typeof(GradientsView)));
        config.Items.Add(new NavigationItem("cards", "Cards", "\uE2C8", typeof(CardsView)));
        config.Items.Add(new NavigationItem("glasscards", "Glass Cards", "\uE2C8", typeof(GlassCardsView)));

        // Initialize navigation
        _navigationService.Initialize(config);
    }
}
