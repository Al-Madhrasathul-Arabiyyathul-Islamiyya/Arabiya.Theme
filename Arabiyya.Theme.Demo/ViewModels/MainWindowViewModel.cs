using Arabiyya.Theme.Demo.Views;
using Arabiyya.Theme.Navigation.Models;
using Arabiyya.Theme.Navigation.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace Arabiyya.Theme.Demo.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly INavigationService _navigationService;

    // Observable property for the content
    [ObservableProperty]
    private object _currentContent;

    public INavigationService NavigationService => _navigationService;

    public MainWindowViewModel(ServiceProvider serviceProvider)
    {
        // Get the navigation service
        _navigationService = serviceProvider.GetRequiredService<INavigationService>();

        // Configure navigation
        var config = new NavigationConfig
        {
            Title = "Arabiyya Theme Demo",
            NavigationMode = NavigationMode.Sidebar,
            UseGlassEffect = true,
            ShowIcons = true,
            AllowCollapse = true
        };

        // Add navigation items
        config.Items.Add(new NavigationItem("colors", "Color Palette", "\uE2B1", typeof(ColorPaletteView))
        {
            ContentFactory = () => serviceProvider.GetRequiredService<ColorPaletteView>()
        });
        config.Items.Add(new NavigationItem("typography", "Typography", "\uE248", typeof(TypographyView))
        {
            ContentFactory = () => serviceProvider.GetRequiredService<TypographyView>()
        });
        config.Items.Add(new NavigationItem("buttons", "Buttons", "\uE037", typeof(ButtonsView))
        {
            ContentFactory = () => serviceProvider.GetRequiredService<ButtonsView>()
        });
        config.Items.Add(new NavigationItem("glasspanels", "Glass Panels", "\uE3E4", typeof(GlassPanelsView))
        {
            ContentFactory = () => serviceProvider.GetRequiredService<GlassPanelsView>()
        });
        config.Items.Add(new NavigationItem("textinputs", "Text Inputs", "\uE324", typeof(TextInputsView))
        {
            ContentFactory = () => serviceProvider.GetRequiredService<TextInputsView>()
        });
        config.Items.Add(new NavigationItem("inputs", "Other Inputs", "\uE272", typeof(InputsView))
        {
            ContentFactory = () => serviceProvider.GetRequiredService<InputsView>()
        });
        config.Items.Add(new NavigationItem("tabs", "Tab Controls", "\uE261", typeof(TabControlView))
        {
            ContentFactory = () => serviceProvider.GetRequiredService<TabControlView>()
        });
        config.Items.Add(new NavigationItem("gradients", "Gradients", "\uE3B5", typeof(GradientsView))
        {
            ContentFactory = () => serviceProvider.GetRequiredService<GradientsView>()
        });
        config.Items.Add(new NavigationItem("cards", "Cards", "\uE25A", typeof(CardsView))
        {
            ContentFactory = () => serviceProvider.GetRequiredService<CardsView>()
        });
        config.Items.Add(new NavigationItem("glasscards", "Glass Cards", "\uE3DA", typeof(GlassCardsView))
        {
            ContentFactory = () => serviceProvider.GetRequiredService<GlassCardsView>()
        });

        // Initialize navigation
        _navigationService.Initialize(config);

        // Subscribe to navigation events
        _navigationService.Navigated += OnNavigated;

        // Set initial content
        CurrentContent = _navigationService.CurrentContent;
    }

    private void OnNavigated(object sender, NavigatedEventArgs e)
    {
        // Update current content when navigation occurs
        CurrentContent = e.Content;
    }
}
