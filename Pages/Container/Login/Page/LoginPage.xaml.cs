using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.Threading.Tasks;
using Record_System.Pages.Container.Login.ViewModel;
using Record_System.Pages.Container.Initial.Page;
using Microsoft.UI.Xaml.Media.Animation;

namespace Record_System.Pages.Container.Login.Page
{
    public sealed partial class LoginPage : Microsoft.UI.Xaml.Controls.Page
    {
        LoginPageViewModel viewModel;

        public LoginPage()
        {
            viewModel = new();

            this.InitializeComponent();
        }

        private void _showTeachingTip(TeachingTip _teachingTip)
        {
            this.DispatcherQueue.TryEnqueue
            (
                Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal,
                async () =>
                {
                    if (_teachingTip.IsOpen)
                        return;

                    _teachingTip.IsOpen = true;

                    await Task.Delay(millisecondsDelay: 2500);

                    _teachingTip.IsOpen = false;
                }
            );
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            viewModel.IsLoginIn = true;

            if (!App.MainViewModel.HasInternetConnection)
            {
                _showTeachingTip(offlineTeachingTip);

                viewModel.IsLoginIn = false;

                return;
            }

            var loginResult = await App.MainViewModel.FirestoreViewModel.Login(userNameInputBox.Text, passwordInputBox.Password);

            if(loginResult == BackEnd.Login.LoginResults.IncorrectPasswordOrUserName)
            {
                _showTeachingTip(incorrectPasswordUserNameTeachingTip);

                viewModel.IsLoginIn = false;

                return;
            }

            if(loginResult == BackEnd.Login.LoginResults.LostConnection)
            {
                _showTeachingTip(offlineTeachingTip);

                viewModel.IsLoginIn = false;

                return;
            }

            if(loginResult == BackEnd.Login.LoginResults.Success)
            {
                viewModel.IsLoginIn = false;

                ((Frame)this.Parent).Navigate(typeof(InitialPage), null, new DrillInNavigationTransitionInfo());
            }
            else
            {
                throw new ArgumentException();
            }
        }
    }
}
