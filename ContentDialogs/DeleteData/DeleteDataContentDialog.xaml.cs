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

namespace Record_System.ContentDialogs.DeleteData
{
    public sealed partial class DeleteDataContentDialog : ContentDialog
    {
        public ContentDialogsResult Result;
        
        private string deleteMessage;

        private string GetDeleteMessage(int numberOfItems)
        {
            return string.Format("Wait.. Are you Sure? You want to Delete {0}. This action can't be Undone", numberOfItems > 1 ? $"{numberOfItems} Items" : "This Item");
        }

        public DeleteDataContentDialog(XamlRoot root, int numberOfItemsToDelete)
        {
            this.XamlRoot = root;

            Result = ContentDialogsResult.Cancel;

            deleteMessage = GetDeleteMessage(numberOfItemsToDelete);

            this.InitializeComponent();
        }

        private void close() => this.Hide();

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            Result = ContentDialogsResult.Done;
            this.close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (Result != ContentDialogsResult.Cancel)
                Result = ContentDialogsResult.Cancel;

            this.close();
        }
    }
}
