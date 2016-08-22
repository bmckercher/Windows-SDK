/*
Copyright (c) 2016 CA. All rights reserved.
This software may be modified and distributed under the terms
of the MIT license. See the LICENSE file for details.
*/

﻿using MASFoundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MagTestApp
{
    public sealed partial class AppUserControl : UserControl
    {
        public AppUserControl()
        {
            this.InitializeComponent();
            MAS.LogMessage += MAS_LogMessage;
        }

        private void MAS_LogMessage(object sender, string e)
        {
            LogMessage(e);
        }

        void LogMessage(string message)
        {
            DebugText.Text += message + System.Environment.NewLine;
            ScrollToBottom();
        }

        void ScrollToBottom()
        {
            var scrollViewer = FindFirstDescendant<ScrollViewer>(DebugText);
            scrollViewer?.ScrollToVerticalOffset(scrollViewer.ScrollableHeight);
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

        public Frame Frame
        {
            get { return RootFrame; }
        }

        public bool IsBusy
        {
            get
            {
                return BusyStatus.IsActive;
            }

            set
            {
                BusyStatus.IsActive = value;
            }
        }

        private void ClearLogBtn_Click(object sender, RoutedEventArgs e)
        {
            DebugText.Text = string.Empty;
        }
    }
}
