using Arabiyya.Theme.Navigation.Models;
using Arabiyya.Theme.Navigation.Services;
using AvaGlass.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Arabiyya.Theme.Navigation.Controls;

public partial class SideNav : UserControl
{
    public static readonly StyledProperty<bool> ShowIconsProperty =
        AvaloniaProperty.Register<SideNav, bool>(nameof(ShowIcons), true);

    public static readonly StyledProperty<bool> ShowLabelsProperty =
        AvaloniaProperty.Register<SideNav, bool>(nameof(ShowLabels), true);

    public static readonly StyledProperty<INavigationService> NavigationServiceProperty =
        AvaloniaProperty.Register<SideNav, INavigationService>(nameof(NavigationService));

    public static readonly StyledProperty<double> ExpandedWidthProperty =
        AvaloniaProperty.Register<SideNav, double>(nameof(ExpandedWidth), 220);

    public static readonly StyledProperty<double> CollapsedWidthProperty =
        AvaloniaProperty.Register<SideNav, double>(nameof(CollapsedWidth), 60);

    /// <summary>
    /// Gets or sets a value indicating whether to show icons
    /// </summary>
    public bool ShowIcons
    {
        get => GetValue(ShowIconsProperty);
        set => SetValue(ShowIconsProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to show labels
    /// </summary>
    public bool ShowLabels
    {
        get => GetValue(ShowLabelsProperty);
        set => SetValue(ShowLabelsProperty, value);
    }

    /// <summary>
    /// Gets or sets the navigation service
    /// </summary>
    public INavigationService NavigationService
    {
        get => GetValue(NavigationServiceProperty);
        set => SetValue(NavigationServiceProperty, value);
    }

    /// <summary>
    /// Gets or sets the width of the sidebar when expanded
    /// </summary>
    public double ExpandedWidth
    {
        get => GetValue(ExpandedWidthProperty);
        set => SetValue(ExpandedWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets the width of the sidebar when collapsed
    /// </summary>
    public double CollapsedWidth
    {
        get => GetValue(CollapsedWidthProperty);
        set => SetValue(CollapsedWidthProperty, value);
    }

    public SideNav()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        if (DataContext is NavigationConfig config)
        {
            ShowIcons = config.ShowIcons;
            ShowLabels = config.ShowLabels;
            ExpandedWidth = config.ExpandedWidth;
            CollapsedWidth = config.CollapsedWidth;

            // Update width based on expanded state
            UpdateWidth(config.IsExpanded);

            // Monitor changes to IsExpanded
            config.PropertyChanged += (s, args) =>
            {
                if (args.PropertyName == nameof(NavigationConfig.IsExpanded))
                {
                    UpdateWidth(config.IsExpanded);
                }
            };

            var glassPanel = this.FindControl<GlassmorphicPanel>("GlassPanel");
            if (glassPanel != null)
            {
                UpdateGlassClass(glassPanel, config.UseGlassEffect);
            }

            // Subscribe to changes
            config.PropertyChanged += (s, args) =>
            {
                if (args.PropertyName == nameof(NavigationConfig.UseGlassEffect))
                {
                    if (glassPanel != null)
                    {
                        UpdateGlassClass(glassPanel, config.UseGlassEffect);
                    }
                }
            };
        }
    }

    private void UpdateWidth(bool isExpanded)
    {
        Width = isExpanded ? ExpandedWidth : CollapsedWidth;
        ShowLabels = isExpanded;
    }

    private void UpdateGlassClass(GlassmorphicPanel panel, bool useGlass)
    {
        panel.Classes.Clear();
        panel.Classes.Add(useGlass ? "GlassPanel" : "AcrylicPanel");
    }
}
