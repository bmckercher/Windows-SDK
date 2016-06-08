using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MagTestApp.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StartupErrorPage : Page
    {
        public StartupErrorPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is Exception)
            {
                ErrorText.Text = ((Exception)e.Parameter).ToString();
            }
        }

        private async void RetryBtn_Click(object sender, RoutedEventArgs e)
        {
            App.IsBusy = true;

            ErrorText.Text = string.Empty;

            try
            {
                var app = (App)App.Current;
                await app.StartupApplicationAsync();
            }
            catch (Exception exp)
            {
                ErrorText.Text = exp.ToString();
            }

            App.IsBusy = false;
        }
    }
}
