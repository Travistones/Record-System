using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Record_System.ContentDialogs.Result;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace Record_System.Pages.Content.Employees.ContentDialogs.RemoveUserConfirmation
{
    public sealed partial class RemoveUserConfirmationContentDialog : ContentDialog
    {
        public ContentDialogsResult Result;

        private string userToRemove;

        public RemoveUserConfirmationContentDialog(XamlRoot XamlRoot, string userToRemove)
        {
            this.userToRemove = userToRemove;

            Result = ContentDialogsResult.Cancel;

            this.XamlRoot = XamlRoot;

            this.InitializeComponent();
        }

        private void Close() => this.Hide();

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            Result = ContentDialogsResult.Done;

            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Result = ContentDialogsResult.Cancel;

            this.Close();
        }
    }
}
