using Arabiyya.Theme.Navigation.Services;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Arabiyya.Theme.Navigation.Controls
{
    public partial class NavigationHost : ContentControl
    {
        public static readonly StyledProperty<INavigationService> NavigationServiceProperty =
            AvaloniaProperty.Register<NavigationHost, INavigationService>(nameof(NavigationService));

        public static readonly StyledProperty<string> TransitionTypeProperty =
            AvaloniaProperty.Register<NavigationHost, string>(nameof(TransitionType), "fade");

        /// <summary>
        /// Gets or sets the navigation service
        /// </summary>
        public INavigationService NavigationService
        {
            get => GetValue(NavigationServiceProperty);
            set
            {
                SetValue(NavigationServiceProperty, value);
                OnNavigationServiceChanged(value);
            }
        }

        /// <summary>
        /// Gets or sets the type of transition to use
        /// </summary>
        public string TransitionType
        {
            get => GetValue(TransitionTypeProperty);
            set
            {
                SetValue(TransitionTypeProperty, value);
                OnTransitionTypeChanged(value);
            }
        }

        public NavigationHost()
        {
            InitializeComponent();

            // Set initial classes
            var presenter = this.FindControl<ContentControl>("ContentPresenter");
            presenter?.Classes.Add(TransitionType);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnNavigationServiceChanged(INavigationService navigationService)
        {
            if (navigationService != null)
            {
                // Set initial content
                Content = navigationService.CurrentContent;

                // Listen for changes
                navigationService.Navigated += OnNavigated;
            }
        }

        private void OnNavigated(object sender, NavigatedEventArgs e)
        {
            // Update the content
            Content = e.Content;

            // Apply transition
            var presenter = this.FindControl<ContentControl>("ContentPresenter");
            if (presenter != null && !string.IsNullOrEmpty(TransitionType))
            {
                presenter.Classes.Clear();
                presenter.Classes.Add(TransitionType);
            }
        }

        private void OnTransitionTypeChanged(string transitionType)
        {
            var presenter = this.FindControl<ContentControl>("ContentPresenter");
            if (presenter != null)
            {
                presenter.Classes.Clear();

                if (!string.IsNullOrEmpty(transitionType))
                {
                    presenter.Classes.Add(transitionType);
                }
            }
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == NavigationServiceProperty)
            {
                OnNavigationServiceChanged((INavigationService)change.NewValue);
            }
            else if (change.Property == TransitionTypeProperty)
            {
                OnTransitionTypeChanged((string)change.NewValue);
            }
        }
    }
}
