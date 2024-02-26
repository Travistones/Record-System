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
using Record_System.Pages.Content.Employees.ContentDialogs.ChangePicture;

namespace Record_System.Pages.Content.Employees.ContentDialogs.ChangePicture
{
    public sealed partial class ChangePictureContentDialog : ContentDialog
    {
        public ChangePictureContentDialogResult Result;

        public ChangePictureContentDialog(XamlRoot root)
        {
            this.XamlRoot = root;

            Result = ChangePictureContentDialogResult.Cancel;

            this.InitializeComponent();
        }

        private void Close() => this.Hide();

        private void CloseButton_Clicked(object sender, RoutedEventArgs e) => this.Close();

        private void ChangePicture_Clicked(object sender, RoutedEventArgs e)
        {
            Result = ChangePictureContentDialogResult.ChangePicture;

            this.Close();
        }
        
        private void RemovePicture_Clicked(object sender, RoutedEventArgs e)
        {
            Result = ChangePictureContentDialogResult.ChangePicture;

            this.Close();
        }

    }
}
