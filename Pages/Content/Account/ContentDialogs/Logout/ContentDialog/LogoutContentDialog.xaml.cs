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

namespace Record_System.Pages.Content.Account.ContentDialogs.Logout.ContentDialog
{
    public sealed partial class LogoutContentDialog : Microsoft.UI.Xaml.Controls.ContentDialog
    {
        public ContentDialogsResult Result;

        public LogoutContentDialog(XamlRoot xamlRoot)
        {
            Result = ContentDialogsResult.Cancel;

            this.XamlRoot = xamlRoot;

            this.InitializeComponent();
        }

        private void Close() => this.Hide();

        private void No_Click(object sender, RoutedEventArgs e)
        {
            Result = ContentDialogsResult.Cancel;
            this.Close();
        }

        private void Yes_Click(object sender, RoutedEventArgs e)
        {
            Result = ContentDialogsResult.Done;
            this.Close();
        }
    }
}
