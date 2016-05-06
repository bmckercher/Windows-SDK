using System;
using MASFoundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;

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

            MAS.Logger = this;
            MAS.LogLevel = LogLevel.Full;
            MAS.LoginRequested += MAS_LoginRequested;

            // British Gas test configuration only supports user device registration
            MAS.ConfigFileName = "msso_config.json";
            MAS.RegistrationKind = RegistrationKind.User;
            BritishGasTestConfigBtn.IsChecked = true;

            BritishGasTestConfigBtn.Checked += BritishGasTestConfigBtn_Checked;
            CATestConfigBtn.Checked += CATestConfigBtn_Checked;
        }

        private async void StartBtn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            IsBusy = true;

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

        private async void MAS_LoginRequested(object sender, object e)
        {
            try
            {
                var user = await User.LoginAsync("winsdktest2", "P@$$w0rd01");
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
                await User.LoginAsync("winsdktest2", "P@$$w0rd01");
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
                await Device.Current.UnregisterAsync();
            }
            catch (Exception exp)
            {
                ((ILogger)this).Error("Unregister failed " + exp.ToString());
            }

            IsBusy = false;
        }

        private async void LogoutDeviceBtn_Click(object sender, RoutedEventArgs e)
        {
            IsBusy = true;
            try
            {
                await Device.Current.LogoutAsync(true);
            }
            catch (Exception exp)
            {
                ((ILogger)this).Error("Logout device failed " + exp.ToString());
            }

            IsBusy = false;
        }

        private async void LogoffBtn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            IsBusy = true;
            try
            {
                if (User.Current != null)
                {
                    await User.Current.LogoffAsync();
                }
                else
                {
                    ((ILogger)this).Info("No current user!");
                }
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
                if (User.Current != null)
                {
                    await User.Current.GetInfoAsync();
                }
                else
                {
                    ((ILogger)this).Info("No current user!");
                }
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

        private async void BritishGasTestConfigBtn_Checked(object sender, RoutedEventArgs e)
        {
            await TryReset();

            // British Gas test configuration only supports user device registration
            MAS.RegistrationKind = RegistrationKind.User;
            MAS.ConfigFileName = "msso_config.json";
        }

        private async void CATestConfigBtn_Checked(object sender, RoutedEventArgs e)
        {
            await TryReset();

            // CA test configuration supports client device registration
            MAS.RegistrationKind = RegistrationKind.Client;
            MAS.ConfigFileName = "msso_config_with_client_reg.json";
        }

        async Task TryReset()
        {
            try
            {
                await MAS.ResetAsync();
            }
            catch
            {
            }
        }

        private void ClearLogBtn_Click(object sender, RoutedEventArgs e)
        {
            DebugInfo.Text = "";
        }


    }
}
