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
using Record_System.BackEnd.ViewModels.Settings;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;

namespace Record_System.Pages.Content.Account.ContentDialogs.ChangePricing.ContentDialog
{
    public sealed partial class ChangePricingContentDialog : Microsoft.UI.Xaml.Controls.ContentDialog
    {
        private SettingsViewModel settingsViewModel;

        public Task UpdatingTask;

        public ChangePricingContentDialog(XamlRoot xamlRoot, SettingsViewModel settingsViewModel)
        {
            UpdatingTask = null;

            this.settingsViewModel = settingsViewModel;

            this.XamlRoot = xamlRoot;

            this.InitializeComponent();
        }

        private void Close() => this.Hide();

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            if (GradeAPriceNumberBox.Value == double.NaN || GradeCPriceNumberBox.Value == double.NaN || GradeCPriceNumberBox.Value == double.NaN)
                return;

            UpdatingTask = App.MainViewModel.FirestoreViewModel.UpdatePricingSettings(true, GradeAPriceNumberBox.Value, GradeBPriceNumberBox.Value, GradeCPriceNumberBox.Value);

            this.Close();
        }
    }
}
