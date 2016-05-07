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
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            MAS.LogLevel = LogLevel.Full;
            MAS.LoginRequested += MAS_LoginRequested;
            MAS.LogMessage += MAS_LogMessage;

            // British Gas test configuration only supports user device registration
            MAS.ConfigFileName = "msso_config.json";
            MAS.RegistrationKind = RegistrationKind.User;
            BritishGasTestConfigBtn.IsChecked = true;

            BritishGasTestConfigBtn.Checked += BritishGasTestConfigBtn_Checked;
            CATestConfigBtn.Checked += CATestConfigBtn_Checked;
        }

        private void MAS_LogMessage(object sender, string e)
        {
            LogMessage(e);
        }

        void LogMessage(string message)
        {
            DebugInfo.Text += message + System.Environment.NewLine;
            ScrollToBottom();
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
                LogMessage("Start failed " + exp.ToString());
            }
            IsBusy = false;
        }

        private async void MAS_LoginRequested(object sender, object e)
        {
            try
            {
                var user = await MASUser.LoginAsync("winsdktest2", "P@$$w0rd01");
            }
            catch (Exception exp)
            {
                LogMessage("Login failed " + exp.ToString());
            }
        }

        private async void LoginBtn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            IsBusy = true;
            try
            {
                await MASUser.LoginAsync("winsdktest2", "P@$$w0rd01");
            }
            catch (Exception exp)
            {
                LogMessage("Login failed " + exp.ToString());
            }

            IsBusy = false;
        }

        private async void UnregisterBtn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            IsBusy = true;
            try
            {
                await MASDevice.Current.UnregisterAsync();
            }
            catch (Exception exp)
            {
                LogMessage("Unregister failed " + exp.ToString());
            }

            IsBusy = false;
        }

        private async void LogoutDeviceBtn_Click(object sender, RoutedEventArgs e)
        {
            IsBusy = true;
            try
            {
                await MASDevice.Current.LogoutAsync(true);
            }
            catch (Exception exp)
            {
                LogMessage("Logout device failed " + exp.ToString());
            }

            IsBusy = false;
        }

        private async void LogoffBtn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            IsBusy = true;
            try
            {
                if (MASUser.Current != null)
                {
                    await MASUser.Current.LogoffAsync();
                }
                else
                {
                    LogMessage("No current user!");
                }
            }
            catch (Exception exp)
            {
                LogMessage("LogoffUser failed " + exp.ToString());
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
                if (MASUser.Current != null)
                {
                    await MASUser.Current.GetInfoAsync();
                }
                else
                {
                    LogMessage("No current user!");
                }
            }
            catch (Exception exp)
            {
                LogMessage("GetUserInfo failed " + exp.ToString());
            }
            IsBusy = false;
        }

        private async void ResetBtn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            IsBusy = true;
            await MAS.ResetAsync();
            IsBusy = false;
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
