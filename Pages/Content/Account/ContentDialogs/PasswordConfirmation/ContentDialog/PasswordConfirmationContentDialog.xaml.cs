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
using Record_System.Pages.Content.Account.ContentDialogs.PasswordConfirmation.Result;
using System.Threading.Tasks;

namespace Record_System.Pages.Content.Account.ContentDialogs.PasswordConfirmation.ContentDialog
{
    public sealed partial class PasswordConfirmationContentDialog : Microsoft.UI.Xaml.Controls.ContentDialog
    {
        public PasswordConfirmationContentDialogResults Result;

        public PasswordConfirmationContentDialog(XamlRoot root)
        {
            Result = PasswordConfirmationContentDialogResults.Cancel;

            this.XamlRoot = root;
            this.InitializeComponent();
        }

        private void Close() => this.Hide();

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            if(passwordInputBox.Password != App.MainViewModel.AccountViewModel.Account.Password)
            {
                _ = showIncorrectPassword();
                return;
            }

            Result = PasswordConfirmationContentDialogResults.Correct;

            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Result = PasswordConfirmationContentDialogResults.Cancel;
            this.Close();
        }

        private async Task showIncorrectPassword()
        {
            var isShown = incorrectPasswordTextBlock.Visibility == Visibility.Visible;

            if (isShown)
                return;

            incorrectPasswordTextBlock.Visibility = Visibility.Visible;

            await Task.Delay(millisecondsDelay: 2500);

            incorrectPasswordTextBlock.Visibility = Visibility.Collapsed;
        }
    }
}
