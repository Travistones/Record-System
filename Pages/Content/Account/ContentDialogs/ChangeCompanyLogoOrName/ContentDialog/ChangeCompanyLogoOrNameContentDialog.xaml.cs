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
using Microsoft.UI.Xaml.Media.Imaging;
using Record_System.Pages.Content.Employees.ContentDialogs.ChangePicture;
using Record_System.BackEnd.ViewModels.Settings;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Record_System.Pages.Content.Account.ContentDialogs.ChangeCompanyLogoOrName.ContentDialog
{
    public sealed partial class ChangeCompanyLogoOrNameContentDialog : Microsoft.UI.Xaml.Controls.ContentDialog, INotifyPropertyChanged
    {
        //private SettingsViewModel settingsViewModel;

        public Task UpdatingTask;

        //private bool imageHasSource;

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public ChangeCompanyLogoOrNameContentDialog(XamlRoot xamlRoot, SettingsViewModel settingsViewModel)
        {
            UpdatingTask = null;

            this.XamlRoot = xamlRoot;

            //this.settingsViewModel = settingsViewModel;

            //imageHasSource = settingsViewModel.ApplicationSettings.HasCompanyLogo;

            this.InitializeComponent();
        }

        //private bool ImageHasSource
        //{
        //    get => imageHasSource;

        //    set
        //    {
        //        imageHasSource = value;
        //        RaisePropertyChanged(nameof(ImageHasSource));
        //    }
        //}

        private void Close() => this.Hide();

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            //UpdatingTask = App.MainViewModel.FirestoreViewModel.UpdateCompanySettings(CompanyLogoImage.Source, CompanyNameTextBox.Text);

            this.Close();
        }

        //private async void ChangeCompanyLogo_Click(object sender, RoutedEventArgs e)
        //{
        //    var pictureStorageFile = await App.MainViewModel.GetLocalImage();

        //    if (pictureStorageFile == null)
        //        return;

        //    CompanyLogoImage.Source = new BitmapImage(new Uri(pictureStorageFile.Path));

        //    ImageHasSource = true;
        //}

        //private void RemoveCompanyLogo_Click(object sender, RoutedEventArgs e)
        //{
        //    CompanyLogoImage.Source = null;

        //    ImageHasSource = false;
        //}
    }
}
