using System;
using MASFoundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MagTestApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, ILogger
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void StartBtn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            IsBusy = true;

            MAS.LoginRequested -= MAS_LoginRequested;
            MAS.RegistrationKind = RegistrationKind.User;
            MAS.Logger = this;
            MAS.LogLevel = LogLevel.Full;
            MAS.LoginRequested += MAS_LoginRequested;

            try
            {
                await MAS.StartAsync();
            }
            catch (Exception exp)
            {
                ((ILogger)this).Error("Start failed " + exp.ToString());
            }
            IsBusy = false;
        }

        private async void MAS_LoginRequested(object sender, EventArgs e)
        {
            try
            {
                await MAS.AuthenticateUserAsync("winsdktest2", "P@$$w0rd01");
            }
            catch (Exception exp)
            {
                ((ILogger)this).Error("Login failed " + exp.ToString());
            }
        }

        private async void LoginBtn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            IsBusy = true;
            try
            {
                await MAS.AuthenticateUserAsync("winsdktest2", "P@$$w0rd01");
            }
            catch (Exception exp)
            {
                ((ILogger)this).Error("Login failed " + exp.ToString());
            }

            IsBusy = false;
        }

        private async void UnregisterBtn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            IsBusy = true;
            try
            {
                await MAS.DeregisterCurrentDeviceAsync();
            }
            catch (Exception exp)
            {
                ((ILogger)this).Error("Unregister failed " + exp.ToString());
            }

            IsBusy = false;
        }

        private async void LogoffBtn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            IsBusy = true;
            try
            {
                await MAS.LogoffUserAsync();
            }
            catch (Exception exp)
            {
                ((ILogger)this).Error("LogoffUser failed " + exp.ToString());
            }
            IsBusy = false;
        }

        bool IsBusy
        {
            get
            {
                return BusyStatus.Visibility == Windows.UI.Xaml.Visibility.Visible;
            }
            set
            {
                BusyStatus.Visibility = value ? Windows.UI.Xaml.Visibility.Visible : Windows.UI.Xaml.Visibility.Collapsed;
            }
        }

        private async void GetUserInfoBtn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            IsBusy = true;
            try
            {
                var response = await MAS.GetFromAsync("https://test.pulsenow.co.uk/openid/connect/v1/userinfo", null, null, ResponseType.Json);
            }
            catch (Exception exp)
            {
                ((ILogger)this).Error("GetUserInfo failed " + exp.ToString());
            }
            IsBusy = false;
        }

        private async void ResetBtn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            IsBusy = true;
            await MAS.ResetAsync();
            IsBusy = false;
        }

        void ILogger.Info(string message)
        {
            DebugInfo.Text += "Info: " + message + System.Environment.NewLine;
            ScrollToBottom();
        }

        void ILogger.Warn(string message)
        {
            DebugInfo.Text += "Warn: " + message + System.Environment.NewLine;
            ScrollToBottom();
        }

        void ILogger.Error(string message)
        {
            DebugInfo.Text += "Error: " + message + System.Environment.NewLine;
            ScrollToBottom();
        }

        void ScrollToBottom()
        {
            var scrollViewer = FindFirstDescendant<ScrollViewer>(DebugInfo);
            scrollViewer.ScrollToVerticalOffset(scrollViewer.ScrollableHeight);
        }

        T FindFirstDescendant<T>(DependencyObject parent) where T : DependencyObject
        {
            var count = VisualTreeHelper.GetChildrenCount(parent);

            for (var i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T)
                {
                    return (T)child;
                }
                else
                {
                    var found = FindFirstDescendant<T>(child);
                    if (found != null)
                    {
                        return found;
                    }
                }
            }

            return null;
        }
    }
}
