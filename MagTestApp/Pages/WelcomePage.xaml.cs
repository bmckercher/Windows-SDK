/*
Copyright (c) 2016 CA. All rights reserved.
This software may be modified and distributed under the terms
of the MIT license. See the LICENSE file for details.
*/

﻿using MASFoundation;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MagTestApp.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WelcomePage : Page
    {
        public WelcomePage()
        {
            InitializeComponent();

            SetUserLoginState(MASUser.Current?.IsLoggedIn == true);
        }

        private void LoginBtn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Frame.Navigate(typeof(LoginPage));
        }

        private async void UnregisterBtn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            App.IsBusy = true;

            try
            {
                await MASDevice.Current?.UnregisterAsync();

                ((App)App.Current).NavigateNoRegister();
            }
            catch (Exception exp)
            {
                ErrorText.Text = exp.ToString();
            }

            App.IsBusy = false;
        }

        private async void ResetBtn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            App.IsBusy = true;

            try
            {
                await MAS.ResetAsync();

                ((App)App.Current).NavigateNoRegister();
            }
            catch (Exception exp)
            {
                ErrorText.Text = exp.ToString();
            }

            App.IsBusy = false;
        }

        private async void LogoffBtn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            App.IsBusy = true;

            try
            {
                if (MASUser.Current != null)
                {
                    await MASUser.Current.LogoffAsync();

                    if (MAS.RegistrationKind == RegistrationKind.User)
                    {
                        ((App)App.Current).NavigateNoUser();
                    }
                    else
                    {
                        SetUserLoginState(false);
                    }
                }
                else
                {
                    ErrorText.Text = "No current user!";
                }
            }
            catch (Exception exp)
            {
                ErrorText.Text = exp.ToString();
            }

            App.IsBusy = false;
        }

        private async void GetUserInfoBtn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            App.IsBusy = true;

            try
            {
                if (MASUser.Current != null)
                {
                    var userInfo = await MASUser.Current.GetInfoAsync();
                    ErrorText.Text = "Name: " + userInfo.Name + " " + userInfo.FamilyName;
                }
                else
                {
                    ErrorText.Text = "No current user!";
                }
            }
            catch (Exception exp)
            {
                ErrorText.Text = exp.ToString();
            }

            App.IsBusy = false;
        }

        private async void ResetUserBtn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            App.IsBusy = true;

            try
            {
                await MAS.ResetAsync();

                ((App)App.Current).NavigateNoRegister();
            }
            catch (Exception exp)
            {
                ErrorText.Text = exp.ToString();
            }

            App.IsBusy = false;
        }

        void SetUserLoginState(bool isUserLogin)
        {
            if (isUserLogin)
            {
                NoUserActions.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                UserActions.Visibility = Windows.UI.Xaml.Visibility.Visible;
                UserLoggedInText.Text = "User logged in";
            }
            else
            {
                NoUserActions.Visibility = Windows.UI.Xaml.Visibility.Visible;
                UserActions.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                UserLoggedInText.Text = string.Empty;
            }
        }
    }
}
