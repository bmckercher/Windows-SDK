using MASFoundation;
using Windows.UI.Xaml.Controls;

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
        }

        private async void StartBtn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            IsBusy = true;
            await MAS.StartAsync();
            IsBusy = false;
        }

        private async void LoginBtn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            IsBusy = true;
            try
            {
                await MAS.AuthenticateUserAsync("winsdktest2", "P@$$w0rd01");
            }
            catch
            {

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
            catch
            {

            }
            IsBusy = false;
        }

        private async void LogoffBtn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            IsBusy = true;
            await MAS.LogoffUserAsync();
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
            var response = await MAS.GetFromAsync("https://test.pulsenow.co.uk/openid/connect/v1/userinfo", null, null, MASResponseType.Json);
            IsBusy = false;
        }

        private async void ResetBtn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            IsBusy = true;
            await MAS.ResetAsync();
            IsBusy = false;
        }
    }
}
