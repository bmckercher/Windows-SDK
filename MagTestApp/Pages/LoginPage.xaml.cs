using MASFoundation;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MagTestApp.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            App.IsBusy = true;
            ErrorText.Text = string.Empty;

            try
            {
                await MASUser.LoginAsync(UsernameText.Text, PasswordText.Password);

                // We have logged in successfully at this point, dismiss and continue
                ((App)App.Current).NavigateToWelcome();
            }
            catch (Exception exp)
            {
                ErrorText.Text = exp.ToString();
            }

            App.IsBusy = false;
        }
    }
}
