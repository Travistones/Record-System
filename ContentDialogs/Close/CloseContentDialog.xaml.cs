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

namespace Record_System.ContentDialogs.Close
{
    public sealed partial class CloseContentDialog : ContentDialog
    {
        public ContentDialogsResult Result;

        public CloseContentDialog(XamlRoot XamlRoot)
        {
            this.XamlRoot = XamlRoot;

            this.InitializeComponent();
        }

        private void Close() => this.Hide();

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Result = ContentDialogsResult.Cancel;

            this.Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Result = ContentDialogsResult.Done;

            this.Close();
        }
    }
}
