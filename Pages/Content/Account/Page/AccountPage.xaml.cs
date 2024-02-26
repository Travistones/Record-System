using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Record_System.Pages.Content.Account.ContentDialogs.ChangeCompanyLogoOrName.ContentDialog;
using Record_System.Pages.Content.Account.ContentDialogs.ChangePricing.ContentDialog;
using Record_System.Pages.Content.Account.ContentDialogs.EditProfile.ContentDialog;
using Record_System.Pages.Content.Account.ContentDialogs.Logout.ContentDialog;
using Record_System.Pages.Content.Account.ContentDialogs.PasswordConfirmation.ContentDialog;
using Record_System.Pages.Content.Account.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace Record_System.Pages.Content.Account.Page
{
    public sealed partial class AccountPage : Microsoft.UI.Xaml.Controls.Page, INotifyPropertyChanged
    {
        private AccountPageViewModel viewModel;

        public AccountPage()
        {
            viewModel = new();

            this.InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        //private async void Logout_Click(object sender, RoutedEventArgs e)
        //{
        //    var logoutContentDialog = new LogoutContentDialog(this.XamlRoot);

        //    await logoutContentDialog.ShowAsync();

        //    if(logoutContentDialog.Result == Record_System.ContentDialogs.Result.ContentDialogsResult.Done)
        //    {

        //    }
        //}

        //private async void ChangeCompanyLogoOrName_Click(object sender, RoutedEventArgs e)
        //{
        //    if (await CheckPassword())
        //    {
        //        var changeCompanyLogoOrNameContentDialog = new ChangeCompanyLogoOrNameContentDialog(this.XamlRoot, viewModel.SettingsViewModel);

        //        await changeCompanyLogoOrNameContentDialog.ShowAsync();

        //        if(changeCompanyLogoOrNameContentDialog.UpdatingTask != null)
        //        {
        //            viewModel.NumberOfUpdatingEvents++;

        //            await changeCompanyLogoOrNameContentDialog.UpdatingTask;

        //            viewModel.NumberOfUpdatingEvents--;
        //        }
        //    }
        //}

        private async void ChangePricing_Click(object sender, RoutedEventArgs e)
        {
            await ChangePricing();
        }

        //private async void EditMyAccount_Click(object sender, RoutedEventArgs e)
        //{
        //    if (await CheckPassword())
        //    {
        //        var editProfileContentDialog = new EditProfileContentDialog(this.XamlRoot, viewModel.AccountViewModel);

        //        await editProfileContentDialog.ShowAsync();

        //        if (editProfileContentDialog.UpdatingTask == null)
        //            return;

        //        await editProfileContentDialog.UpdatingTask;
        //    }
        //}

        private async Task<bool> CheckPassword()
        {
            var passwordContentDialog = new PasswordConfirmationContentDialog(this.XamlRoot);

            await passwordContentDialog.ShowAsync();

            if (passwordContentDialog.Result != ContentDialogs.PasswordConfirmation.Result.PasswordConfirmationContentDialogResults.Correct)
                return false;

            return true;
        }

        private async Task<bool> ChangePricing()
        {
            if (await CheckPassword())
            {
                var changePricingContentDialog = new ChangePricingContentDialog(this.XamlRoot, this.viewModel.SettingsViewModel);

                await changePricingContentDialog.ShowAsync();

                if (changePricingContentDialog.UpdatingTask != null)
                {
                    viewModel.NumberOfUpdatingEvents++;

                    await changePricingContentDialog.UpdatingTask;

                    viewModel.NumberOfUpdatingEvents--;

                    return true;
                }

                return false;
            }

            return false;
        }

        private bool isPricePerVolumeLoaded = false;

        private bool canRunToggledFunction = true;

        private async void AutoFillPricePerVolume_Toggled(object sender, RoutedEventArgs e)
        {
            if (!isPricePerVolumeLoaded)
                return;

            if (!canRunToggledFunction)
                return;

            canRunToggledFunction = false;

            if (((ToggleSwitch)sender).IsOn)
            {
                if (!viewModel.SettingsViewModel.ApplicationSettings.ArePricesSet /* && viewModel.AccountViewModel.Account.EmployeeAccount.CanModifyPrices*/)
                {
                    viewModel.SettingsViewModel.ApplicationSettings.CanUseCurrentPricePerVolume = await ChangePricing();
                    ((ToggleSwitch)sender).IsOn = viewModel.SettingsViewModel.ApplicationSettings.CanUseCurrentPricePerVolume;

                    canRunToggledFunction = true;
                    return;
                }

            }

            viewModel.SettingsViewModel.ApplicationSettings.CanUseCurrentPricePerVolume = !viewModel.SettingsViewModel.ApplicationSettings.CanUseCurrentPricePerVolume;
            ((ToggleSwitch)sender).IsOn = viewModel.SettingsViewModel.ApplicationSettings.CanUseCurrentPricePerVolume;

            canRunToggledFunction = true;
        }

        private void AutoFillPricePerVolume_Loaded(object sender, RoutedEventArgs e)
        {
            ((ToggleSwitch)sender).IsOn = viewModel.SettingsViewModel.ApplicationSettings.CanUseCurrentPricePerVolume;

            isPricePerVolumeLoaded = true;
        }

        private async void ChangePasswordAndUserName_Click(object sender, RoutedEventArgs e)
        {
            if (await CheckPassword())
            {
                var editProfileContentDialog = new EditProfileContentDialog(this.XamlRoot, viewModel.AccountViewModel);

                await editProfileContentDialog.ShowAsync();

                if (editProfileContentDialog.UpdatingTask == null)
                    return;

                await editProfileContentDialog.UpdatingTask;
            }
        }
    }
}
