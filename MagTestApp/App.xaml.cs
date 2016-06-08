using MagTestApp.Pages;
using MASFoundation;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace MagTestApp
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            MAS.ConfigFileName = "msso_config.json";
            MAS.RegistrationKind = RegistrationKind.Client;
            MAS.LoginRequested += MAS_LoginRequested;
            MAS.LogLevel = LogLevel.Full;

            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        private void MAS_LoginRequested(object sender, object e)
        {
            _wasLoginRequested = true;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            var control = Window.Current.Content as AppUserControl;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (control == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                control = new AppUserControl();
                control.Frame.NavigationFailed += OnNavigationFailed;
                control.Frame.Navigated += RootFrame_Navigated;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = control;
            }

            if (_emptyNavState == null)
            {
                _emptyNavState = control.Frame.GetNavigationState();
            }

            var navManager = SystemNavigationManager.GetForCurrentView();
            navManager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            navManager.BackRequested += NavManager_BackRequested;
            navManager.AppViewBackButtonVisibility = control.Frame.CanGoBack ? AppViewBackButtonVisibility.Visible :
                AppViewBackButtonVisibility.Collapsed;

            if (e.PrelaunchActivated == false)
            {
                if (control.Frame.Content == null)
                {
                    _startupArgs = e.Arguments;
                    await StartupAndNavigateAsync();
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        private void NavManager_BackRequested(object sender, BackRequestedEventArgs e)
        {
            var rootFrame = Frame;

            if (rootFrame.CanGoBack)
            {
                e.Handled = true;
                rootFrame.GoBack();
            }
        }

        private void RootFrame_Navigated(object sender, NavigationEventArgs e)
        {
            UpdateAppBackButton();
        }

        void UpdateAppBackButton()
        {
            var rootFrame = Frame;
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                rootFrame.CanGoBack ? AppViewBackButtonVisibility.Visible :
                AppViewBackButtonVisibility.Collapsed;
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        Frame Frame
        {
            get
            {
                var control = Window.Current.Content as AppUserControl;
                return control.Frame;
            }
        }

        static internal bool IsBusy
        {
            get
            {
                var control = Window.Current.Content as AppUserControl;
                return control.IsBusy;
            }

            set
            {
                var control = Window.Current.Content as AppUserControl;
                control.IsBusy = value;
            }
        }

        internal async Task StartupApplicationAsync()
        {
            await StartAsync();

            var rootFrame = Frame;
            rootFrame.SetNavigationState(_emptyNavState);
            rootFrame.Navigate(typeof(WelcomePage), _startupArgs);
        }

        internal void NavigateToWelcome()
        {
            var rootFrame = Frame;
            rootFrame.SetNavigationState(_emptyNavState);
            rootFrame.Navigate(typeof(WelcomePage));
        }

        internal void NavigateNoRegister()
        {
            var rootFrame = Frame;
            rootFrame.SetNavigationState(_emptyNavState);
            rootFrame.Navigate(typeof(NotRegisteredPage));
        }

        internal void NavigateNoUser()
        {
            var rootFrame = Frame;
            rootFrame.SetNavigationState(_emptyNavState);
            rootFrame.Navigate(typeof(LoginPage));
        }

        async Task StartAsync()
        {
            if (MASDevice.Current?.IsRegistered != true)
            {
                _wasLoginRequested = false;

                await MAS.StartAsync();
            }
        }

        async Task StartupAndNavigateAsync()
        {
            var rootFrame = Frame;

            try
            {
                await StartAsync();
            }
            catch (Exception exp)
            {
                // Most likely an network error has occured
                rootFrame.Navigate(typeof(StartupErrorPage), exp);
                return;
            }

            rootFrame.SetNavigationState(_emptyNavState);

            if (_wasLoginRequested)
            {
                rootFrame.Navigate(typeof(LoginPage), _startupArgs);
            }
            else
            {
                rootFrame.Navigate(typeof(WelcomePage), _startupArgs);
            }
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        string _startupArgs;
        bool _wasLoginRequested;
        string _emptyNavState;
    }
}
