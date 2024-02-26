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
using Record_System.BackEnd.ViewModels.Account;
using Microsoft.UI.Xaml.Media.Imaging;
using Record_System.BackEnd.ViewModels.Settings;
using Record_System.Pages.Content.Employees.ContentDialogs.ChangePicture;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Record_System.Pages.Content.Account.ContentDialogs.EditProfile.ContentDialog
{
    public sealed partial class EditProfileContentDialog : Microsoft.UI.Xaml.Controls.ContentDialog, INotifyPropertyChanged
    {
        private AccountViewModel accountViewModel;

        public Task UpdatingTask;

        //private bool imageHasSource;

        //private bool isCheckingUserName;

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public EditProfileContentDialog(XamlRoot root, AccountViewModel accountViewModel)
        {
            UpdatingTask = null;

            this.accountViewModel = accountViewModel;

            //ImageHasSource = this.accountViewModel.Account.EmployeeAccount.HasProfilePicture;

            this.XamlRoot = root;

            this.InitializeComponent();
        }

        //private async void ChangeProfilePicture_Click(object sender, RoutedEventArgs e)
        //{
        //    var imageStorageFile = await App.MainViewModel.GetLocalImage();

        //    if (imageStorageFile == null)
        //        return;

        //    ProfilePictureImage.Source = new BitmapImage(new Uri(imageStorageFile.Path));

        //    ImageHasSource = true;
        //}

        private void Close() => this.Hide();

        //private bool ImageHasSource
        //{
        //    get => imageHasSource && accountViewModel.Account.IsOwner;

        //    set
        //    {
        //        imageHasSource = value;
        //        RaisePropertyChanged(nameof(ImageHasSource));
        //    }
        //}

        //private bool IsCheckingUserName
        //{
        //    get => isCheckingUserName;

        //    set
        //    {
        //        isCheckingUserName = value;
        //        RaisePropertyChanged(nameof(IsCheckingUserName));
        //        RaisePropertyChanged(nameof(CanChangeFullName));
        //    }
        //}

        //private bool CanChangeFullName
        //{
        //    get => !IsCheckingUserName && accountViewModel.Account.IsOwner;
        //}

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.UpdatingTask = null;

            this.Close();
        }

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            //CloseUserNameTakenInfoBar();

            if (/*string.IsNullOrEmpty(FullNameTextBox.Text) ||*/
                string.IsNullOrEmpty(UserNameTextBox.Text) ||
                string.IsNullOrEmpty(PasswordInputBox.Password))
                return;

            //IsCheckingUserName = true;

            //var isUserNameTaken = await App.MainViewModel.FirestoreViewModel.IsUserNamePresent(UserNameTextBox.Text);

            //IsCheckingUserName = false;

            //if (isUserNameTaken)
            //{
            //    UserNameTakenInfoBar.IsOpen = true;
            //    return;
            //}

            UpdatingTask = App.MainViewModel.FirestoreViewModel.UpdateUserAccount
            (
                //ProfilePictureImage.Source,
                //FullNameTextBox.Text,
                UserNameTextBox.Text,
                PasswordInputBox.Password
            );

            this.Close();
        }

        //private void RemoveProfilePicture_Click(object sender, RoutedEventArgs e)
        //{
        //    //ProfilePictureImage.Source = null;

        //    ImageHasSource = false;
        //}

        //private void UserName_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        //{
        //    args.Cancel = (args.NewText.Contains(@"/") || args.NewText.Contains(@"\"));
        //    CloseUserNameTakenInfoBar();
        //}

        //private void CloseUserNameTakenInfoBar()
        //{
        //    if (UserNameTakenInfoBar.IsOpen)
        //        UserNameTakenInfoBar.IsOpen = false;
        //}
    }
}
