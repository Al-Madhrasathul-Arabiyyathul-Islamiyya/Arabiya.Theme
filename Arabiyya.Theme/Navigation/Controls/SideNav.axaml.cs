using Arabiyya.Theme.Navigation.Models;
using Arabiyya.Theme.Navigation.Services;
using AvaGlass.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace Arabiyya.Theme.Navigation.Controls;

/// <summary>
/// Interaction logic for SideNav.xaml.
/// Provides a sidebar navigation UI based on a NavigationConfig and INavigationService.
/// </summary>
public partial class SideNav : UserControl
{
    // Styled Properties for configuration and appearance
    public static readonly StyledProperty<bool> ShowIconsProperty =
        AvaloniaProperty.Register<SideNav, bool>(nameof(ShowIcons), true);

    public static readonly StyledProperty<bool> ShowLabelsProperty =
        AvaloniaProperty.Register<SideNav, bool>(nameof(ShowLabels), true);

    public static readonly StyledProperty<INavigationService?> NavigationServiceProperty =
        AvaloniaProperty.Register<SideNav, INavigationService?>(nameof(NavigationService));

    public static readonly StyledProperty<double> ExpandedWidthProperty =
        AvaloniaProperty.Register<SideNav, double>(nameof(ExpandedWidth), 220);

    public static readonly StyledProperty<double> CollapsedWidthProperty =
        AvaloniaProperty.Register<SideNav, double>(nameof(CollapsedWidth), 60);

    public static readonly StyledProperty<NavigationConfig?> ConfigProperty =
        AvaloniaProperty.Register<SideNav, NavigationConfig?>(nameof(Config));

    /// <summary>
    /// Gets or sets a value indicating whether icons should be displayed next to navigation items.
    /// </summary>
    public bool ShowIcons
    {
        get => GetValue(ShowIconsProperty);
        set => SetValue(ShowIconsProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether labels should be displayed next to navigation items.
    /// Automatically updated based on expanded state if AllowCollapse is true.
    /// </summary>
    public bool ShowLabels
    {
        get => GetValue(ShowLabelsProperty);
        set => SetValue(ShowLabelsProperty, value);
    }

    /// <summary>
    /// Gets or sets the navigation service instance used to control navigation state and actions.
    /// </summary>
    public INavigationService? NavigationService
    {
        get => GetValue(NavigationServiceProperty);
        set => SetValue(NavigationServiceProperty, value);
    }

    /// <summary>
    /// Gets or sets the width of the sidebar when it is fully expanded.
    /// </summary>
    public double ExpandedWidth
    {
        get => GetValue(ExpandedWidthProperty);
        set => SetValue(ExpandedWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets the width of the sidebar when it is collapsed (typically showing only icons).
    /// </summary>
    public double CollapsedWidth
    {
        get => GetValue(CollapsedWidthProperty);
        set => SetValue(CollapsedWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets the navigation configuration containing items, header, footer, and settings.
    /// </summary>
    public NavigationConfig? Config
    {
        get => GetValue(ConfigProperty);
        set => SetValue(ConfigProperty, value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SideNav"/> class.
    /// </summary>
    public SideNav()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    /// <summary>
    /// Handles changes to dependency properties, particularly Config and NavigationService.
    /// </summary>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        // Re-bind ListBox when Config or NavigationService changes
        if (change.Property == ConfigProperty || change.Property == NavigationServiceProperty)
        {
            SetupListBoxBindings();
        }
        // Update width and label visibility when IsExpanded changes (if controlled internally or via Config)
        else if (change.Property == ConfigProperty && change.NewValue is NavigationConfig newConfig)
        {
            // If Config itself changes, re-evaluate width based on its IsExpanded
            UpdateWidth(newConfig.IsExpanded);
        }
    }

    /// <summary>
    /// Sets up bindings for the internal ListBox when the control is attached to the visual tree.
    /// </summary>
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        SetupListBoxBindings();
        UpdateVisualStates();

        // Subscribe to theme changes for dynamic updates
        if (Application.Current != null)
        {
            Application.Current.ActualThemeVariantChanged += OnApplicationThemeChanged;
        }

        // Subscribe to Config property changes for dynamic updates (e.g., IsExpanded)
        if (Config != null)
        {
            Config.PropertyChanged += Config_PropertyChanged;
            UpdateWidth(Config.IsExpanded);
        }
    }

    /// <summary>
    /// Cleans up subscriptions when the control is detached from the visual tree.
    /// </summary>
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        if (Application.Current != null)
        {
            Application.Current.ActualThemeVariantChanged -= OnApplicationThemeChanged;
        }
        if (Config != null)
        {
            Config.PropertyChanged -= Config_PropertyChanged;
        }
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        // Store original values once the control is loaded
        var listBox = this.FindControl<ListBox>("NavItemsListBox");
        if (listBox != null && listBox.Items.Count > 0)
        {
            if (listBox.Items[0] is ListBoxItem firstItem)
            {
                var grid = firstItem.GetLogicalDescendants().OfType<Grid>().FirstOrDefault();
                if (grid != null)
                {
                    var iconTextBlock = grid.GetLogicalDescendants()
                        .OfType<TextBlock>()
                        .FirstOrDefault(tb => Grid.GetColumn(tb) == 0);

                    if (iconTextBlock != null)
                    {
                        _originalIconMargin = iconTextBlock.Margin;
                    }
                }
            }
        }

        var glassPanel = this.FindControl<AvaGlass.Controls.GlassmorphicPanel>("GlassPanel");
        if (glassPanel != null)
        {
            _originalPanelPadding = glassPanel.Padding;
        }

        // Apply initial state
        UpdateWidth(Config?.IsExpanded ?? true);
    }

    /// <summary>
    /// Updates the ListBox ItemsSource and SelectedItem bindings based on the current Config and NavigationService.
    /// </summary>
    private void SetupListBoxBindings()
    {
        var listBox = this.FindControl<ListBox>("NavItemsListBox");
        if (listBox != null)
        {
            // Clear previous bindings first to be safe
            listBox.ClearValue(ItemsControl.ItemsSourceProperty);
            listBox.ClearValue(ListBox.SelectedItemProperty);

            if (Config != null && NavigationService != null)
            {
                // Bind ItemsSource directly to the Config's Items collection
                listBox.ItemsSource = Config.Items;

                // Bind SelectedItem two-way to the NavigationService's SelectedItem
                listBox.Bind(ListBox.SelectedItemProperty,
                    new Binding
                    {
                        Source = NavigationService,
                        Path = nameof(INavigationService.SelectedItem), // Use nameof for type safety
                        Mode = BindingMode.TwoWay // Reflects service changes and updates service on UI change
                    });
            }
        }
    }

    /// <summary>
    /// Updates visual states like glass/acrylic panel class based on current settings.
    /// </summary>
    private void UpdateVisualStates()
    {
        var glassPanel = this.FindControl<GlassmorphicPanel>("GlassPanel");
        if (glassPanel != null && Config != null)
        {
            UpdateGlassClass(glassPanel, Config.UseGlassEffect);
        }
        if (Config != null)
        {
            UpdateWidth(Config.IsExpanded);
        }
    }


    /// <summary>
    /// Handles property changes within the bound NavigationConfig object.
    /// </summary>
    private void Config_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(NavigationConfig.IsExpanded))
        {
            UpdateWidth(Config?.IsExpanded ?? true);
        }
        else if (e.PropertyName == nameof(NavigationConfig.UseGlassEffect))
        {
            UpdateVisualStates();
        }
    }

    /// <summary>
    /// Responds to application theme changes to update the glass/acrylic effect.
    /// </summary>
    private void OnApplicationThemeChanged(object? sender, EventArgs e)
    {
        UpdateVisualStates();
    }

    private Thickness _originalIconMargin = new(12, 0, 8, 0);
    private Thickness _originalPanelPadding = new(16, 0);
    private readonly Thickness _originalHeaderBorderPadding = new(16);
    private readonly Thickness _originalListBoxPadding = new(8,0);

    /// <summary>
    /// Updates the width of the control and the visibility of labels based on the expanded state.
    /// </summary>
    /// <param name="isExpanded">True if the sidebar should be expanded, false otherwise.</param>
    private void UpdateWidth(bool isExpanded)
    {
        bool canCollapse = Config?.AllowCollapse ?? false;
        Width = isExpanded || !canCollapse ? ExpandedWidth : CollapsedWidth;

        ShowLabels = isExpanded;
        ShowIcons = true;

        var glassPanel = this.FindControl<AvaGlass.Controls.GlassmorphicPanel>("GlassPanel");
        var headerBorder = this.FindControl<Border>("HeaderBorder");
        var listBox = this.FindControl<ListBox>("NavItemsListBox");

        if (glassPanel != null)
            glassPanel.Padding = isExpanded ? _originalPanelPadding : new Thickness(4, 0);

        if (headerBorder != null)
            headerBorder.Padding = isExpanded ? _originalHeaderBorderPadding : new Thickness(6);

        if (listBox != null)
        {
            listBox.Padding = isExpanded ? _originalListBoxPadding : new Thickness(0);
            foreach (var item in listBox.GetLogicalDescendants().OfType<ListBoxItem>())
            {
                var grid = item.GetLogicalDescendants().OfType<Grid>().FirstOrDefault();
                if (grid != null)
                {
                    grid.HorizontalAlignment = isExpanded ? HorizontalAlignment.Stretch : HorizontalAlignment.Center;

                    var iconTextBlock = grid.GetLogicalDescendants()
                        .OfType<TextBlock>()
                        .FirstOrDefault(tb => Grid.GetColumn(tb) == 0); // Column 0 should be the icon

                    if (iconTextBlock != null)
                    {
                        iconTextBlock.FontSize = isExpanded ? 14 : 20;
                        iconTextBlock.TextAlignment = isExpanded ? TextAlignment.Start : TextAlignment.Center;
                        iconTextBlock.Margin = isExpanded ? _originalIconMargin : new Thickness(0);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Sets the appropriate CSS class on the background panel based on the glass effect setting.
    /// </summary>
    private static void UpdateGlassClass(GlassmorphicPanel panel, bool useGlass)
    {
        panel.Classes.Set("GlassPanel", useGlass);
        panel.Classes.Set("AcrylicPanel", !useGlass);
    }
}
