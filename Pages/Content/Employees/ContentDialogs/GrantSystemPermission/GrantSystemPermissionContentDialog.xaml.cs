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
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace Record_System.Pages.Content.Employees.ContentDialogs.GrantSystemPermission
{
    public sealed partial class GrantSystemPermissionContentDialog : ContentDialog, INotifyPropertyChanged
    {
        private bool _canModifyPrices;

        private bool _canModifyEmployees;

        private bool _canModifyCompany;

        public bool CanModifyPrices
        {
            get => _canModifyPrices;

            set
            {
                _canModifyPrices = value;
                RaisePropertyChanged(nameof(CanModifyPrices));
            }
        }

        public bool CanModifyEmployees
        {
            get => _canModifyEmployees;

            set
            {
                _canModifyEmployees = value;
                RaisePropertyChanged(nameof(CanModifyEmployees));
            }
        }

        public bool CanModifyCompany
        {
            get => _canModifyCompany;

            set
            {
                _canModifyCompany = value;
                RaisePropertyChanged(nameof(CanModifyCompany));
            }
        }

        public ContentDialogsResult Result;
        
        public GrantSystemPermissionContentDialog(XamlRoot XamlRoot)
        {
            this.XamlRoot = XamlRoot;

            Result = ContentDialogsResult.Cancel;

            this.InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void Close() => this.Hide();

        private void Close_BottomPane(object sender, RoutedEventArgs e)
        {
            Result = ContentDialogsResult.Cancel;

            this.Close();
        }

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            Result = ContentDialogsResult.Done;

            this.Close();
        }
    }
}
