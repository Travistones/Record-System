using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Record_System.ContentDialogs.Result;
using Record_System.BackEnd.Data;

namespace Record_System.Pages.Content.Credits.ContentDialogs.ClosingUnpaid
{
    public sealed partial class ClosingUnpaidContentDialog : ContentDialog
    {
        private Credit creditToClose;

        public ContentDialogsResult Result;

        public ClosingUnpaidContentDialog(XamlRoot XamlRoot, Credit creditToClose)
        {
            Result = ContentDialogsResult.Cancel;

            this.XamlRoot = XamlRoot;

            this.creditToClose = creditToClose;

            this.InitializeComponent();
        }

        private void Close() => this.Hide();

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Result = ContentDialogsResult.Cancel;

            this.Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Result = ContentDialogsResult.Done;

            this.Close();
        }
    }
}
