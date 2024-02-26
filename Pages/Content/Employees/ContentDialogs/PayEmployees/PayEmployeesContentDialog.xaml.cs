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
using System.Security.Cryptography;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace Record_System.Pages.Content.Employees.ContentDialogs.PayEmployees
{
    public sealed partial class PayEmployeesContentDialog : ContentDialog
    {
        public ContentDialogsResult Result;

        private double totalSalary;

        public PayEmployeesContentDialog(XamlRoot XamlRoot, double totalSalary)
        {
            Result = ContentDialogsResult.Cancel;

            this.totalSalary = totalSalary;

            this.XamlRoot = XamlRoot;

            this.InitializeComponent();
        }

        private void Close() => this.Hide();

        private void Pay_Click(object sender, RoutedEventArgs e)
        {
            Result = ContentDialogsResult.Done;

            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Result = ContentDialogsResult.Cancel;

            this.Close();
        }
    }
}
