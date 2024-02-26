using CommunityToolkit.Common;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Record_System.ApplicationWindows.Main.Window;
using Record_System.BackEnd.Data;
using Record_System.ContentDialogs.DeleteData;
using Record_System.Pages.Content.Employees.ContentDialogs.ChangePicture;
using Record_System.Pages.Content.Employees.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using Windows.Storage;
using WinRT.Interop;
using Microsoft.UI.Xaml.Media.Imaging;
using Record_System.Pages.Container.Root.Page;
using Record_System.Pages.Content.Account.ContentDialogs.PasswordConfirmation.ContentDialog;
using Record_System.Pages.Content.Employees.ContentDialogs.GrantSystemPermission;
using Record_System.Pages.Content.Employees.ContentDialogs.RemoveUserConfirmation;
using Record_System.Pages.Content.Employees.ContentDialogs.ResetPasswordAndUserName;
using Record_System.Pages.Content.Employees.ContentDialogs.PayEmployees;

namespace Record_System.Pages.Content.Employees.Page
{
    public sealed partial class EmployeesPage : Microsoft.UI.Xaml.Controls.Page
    {
        private EmployeesPageViewModel viewModel;

        private NumberBox currentNumberBox;

        public EmployeesPage()
        {
            viewModel = new();

            currentNumberBox = null;

            SizeChanged += PageSizeChanged;
            this.InitializeComponent();

            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        private RootPage rootPage;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter == null)
                return;

            rootPage = e.Parameter as RootPage;
        }

        private void ClosePane()
        {
            this.bottomPaneSplitView.IsPaneOpen = false;
        }

        private void OpenPane()
        {
            this.bottomPaneSplitView.IsPaneOpen = true;
        }

        private void PageSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.bottomPaneSplitView.OpenPaneLength = e.NewSize.Width;
        }

        private void NumberInput_GettingFocus(UIElement sender, GettingFocusEventArgs args)
        {
            currentNumberBox = (sender as NumberBox);

            (args.OriginalSource as TextBox).TextChanged += NumberBox_TextChanged;
        }

        private void NumberBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((sender as TextBox).Text.IsDecimal())
            {
                try
                {
                    currentNumberBox.Value = double.Parse((sender as TextBox).Text);
                }
                catch
                {
                    currentNumberBox.Value = double.NaN;
                }
            }
            else
                currentNumberBox.Value = double.NaN;
        }

        private void Close_BottomPane(object sender, RoutedEventArgs e)
        {
            this.ClosePane();
        }

        private async void Add_NewEmployee(object sender, RoutedEventArgs e)
        {
            if (!App.MainViewModel.HasInternetConnection)
            {
                await rootPage.ShowOfflineTeachingTip();
                return;
            }

            if (!viewModel.IsEdit)
            {
                if (!bottomPaneSplitView.IsPaneOpen)
                    return;

                this.ClosePane();

                App.MainViewModel.IncrementUploadTasks();

                await this.viewModel.AddToEmployees();

                App.MainViewModel.DecrementUploadTasks();
                
                return;
            }

            this.ClosePane();

            viewModel.IsEdit = false;

            Task task = (EmployeesGridView.SelectedItem as Employee).UpdateEmployee(viewModel.NewEmployee);

            if (task == null)
                return;

            App.MainViewModel.IncrementUploadTasks();

            await task;

            App.MainViewModel.DecrementUploadTasks();
        }

        private async void Delete_SelectedEmployees(object sender, RoutedEventArgs e)
        {
            var deleteContentDialog = new DeleteDataContentDialog(this.XamlRoot, EmployeesGridView.SelectedItems.Count);

            await deleteContentDialog.ShowAsync();

            if (deleteContentDialog.Result == Record_System.ContentDialogs.Result.ContentDialogsResult.Done)
            {
                if (!App.MainViewModel.HasInternetConnection)
                {
                    await rootPage.ShowOfflineTeachingTip();
                    return;
                }

                var listOfItemsToDelete = new List<Employee>();

                foreach (var selectedItem in EmployeesGridView.SelectedItems)
                {
                    listOfItemsToDelete.Add((Employee)selectedItem);
                }

                await viewModel.DeleteEmployees(listOfItemsToDelete);
            }
        }

        private void NumberInput_LostFocus(object sender, RoutedEventArgs e)
        {
            (e.OriginalSource as TextBox).TextChanged -= NumberBox_TextChanged;
        }

        private void Open_BottomPane(object sender, RoutedEventArgs e)
        {
            viewModel.CreateNewEmployee();
            this.OpenPane();
        }

        private void EmployeesGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(EmployeesSplitView.IsPaneOpen)
            {
                EmployeesSplitView.IsPaneOpen = (sender as GridView).SelectedItem != null;
            }

            viewModel.CanDeleteEmployees = (sender as GridView).SelectedItems.Count > 0;

            viewModel.SelectedEmployee = (sender as GridView).SelectedItem as Employee;
        }

        private async void ChangeProfilePicture_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel.NewEmployee.HasNoProfilePicture)
            {
                viewModel.NewEmployee.ProfilePictureDownloadUrl = new BitmapImage(new Uri((await App.MainViewModel.GetLocalImage()).Path));
                return;
            }

            var ChangeProfilePictureContentDialog = new ChangePictureContentDialog(this.XamlRoot);

            await ChangeProfilePictureContentDialog.ShowAsync();

            if (ChangeProfilePictureContentDialog.Result == ContentDialogs.ChangePicture.ChangePictureContentDialogResult.Cancel)
                return;

            else if(ChangeProfilePictureContentDialog.Result == ContentDialogs.ChangePicture.ChangePictureContentDialogResult.ChangePicture)
            {
                viewModel.NewEmployee.ProfilePictureDownloadUrl = new BitmapImage(new Uri((await App.MainViewModel.GetLocalImage()).Path));
            }
            else if(ChangeProfilePictureContentDialog.Result == ContentDialogs.ChangePicture.ChangePictureContentDialogResult.RemoveCurrent)
            {
                viewModel.NewEmployee.ProfilePictureDownloadUrl = null;
            }
        }

        private void Employee_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (EmployeesGridView.SelectedItem != null)
                EmployeesSplitView.IsPaneOpen = true;
        }

        private void CloseEmployeePane_BottomPane(object sender, RoutedEventArgs e)
        {
            if (!EmployeesSplitView.IsPaneOpen)
                return;

            EmployeesSplitView.IsPaneOpen = false;
        }

        private void EditEmployee_Click(object sender, RoutedEventArgs e)
        {
            viewModel.EditEmployee((Employee)EmployeesGridView.SelectedItem);

            bottomPaneSplitView.IsPaneOpen = true;
        }

        //private async void GrantSystemPermission_Click(object sender, RoutedEventArgs e)
        //{
        //    if (!App.MainViewModel.HasInternetConnection)
        //    {
        //        await rootPage.ShowOfflineTeachingTip();
        //        return;
        //    }

        //    if (viewModel.SelectedEmployee.IsUser)
        //        return;

        //    if (!(await CheckPassword()))
        //        return;

        //    var grantPermissionContentDialog = new GrantSystemPermissionContentDialog(this.XamlRoot);

        //    await grantPermissionContentDialog.ShowAsync();

        //    if (grantPermissionContentDialog.Result == Record_System.ContentDialogs.Result.ContentDialogsResult.Cancel)
        //        return;

        //    App.MainViewModel.IncrementUploadTasks();

        //    await App.MainViewModel.FirestoreViewModel.AddSystemUser(viewModel.SelectedEmployee, grantPermissionContentDialog.CanModifyPrices, grantPermissionContentDialog.CanModifyEmployees, grantPermissionContentDialog.CanModifyCompany);

        //    App.MainViewModel.DecrementUploadTasks();
        //}

        private async Task<bool> CheckPassword()
        {
            var passwordContentDialog = new PasswordConfirmationContentDialog(this.XamlRoot);

            await passwordContentDialog.ShowAsync();

            return passwordContentDialog.Result == Account.ContentDialogs.PasswordConfirmation.Result.PasswordConfirmationContentDialogResults.Correct;
        }

        //private async void ChangeUserPermissions_Click(object sender, RoutedEventArgs e)
        //{
        //    if (!App.MainViewModel.HasInternetConnection)
        //    {
        //        await rootPage.ShowOfflineTeachingTip();
        //        return;
        //    }

        //    if (!(await CheckPassword()))
        //        return;

        //    var grantPermissionContentDialog = new GrantSystemPermissionContentDialog(this.XamlRoot);

        //    grantPermissionContentDialog.CanModifyCompany = viewModel.SelectedEmployee.CanModifyCompany;
        //    grantPermissionContentDialog.CanModifyEmployees = viewModel.SelectedEmployee.CanModifyEmployees;
        //    grantPermissionContentDialog.CanModifyPrices = viewModel.SelectedEmployee.CanModifyPrices;

        //    await grantPermissionContentDialog.ShowAsync();

        //    if (grantPermissionContentDialog.Result == Record_System.ContentDialogs.Result.ContentDialogsResult.Cancel)
        //        return;

        //    Task updateTask = viewModel.SelectedEmployee.UpdateUserPermissions(grantPermissionContentDialog.CanModifyCompany, grantPermissionContentDialog.CanModifyEmployees, grantPermissionContentDialog.CanModifyPrices);

        //    if (updateTask == null)
        //        return;

        //    App.MainViewModel.IncrementUploadTasks();

        //    await updateTask;

        //    App.MainViewModel.DecrementUploadTasks();
        //}

        //private async void RemoveUserFromTheSystem_Click(object sender, RoutedEventArgs e)
        //{
        //    if (!App.MainViewModel.HasInternetConnection)
        //    {
        //        await rootPage.ShowOfflineTeachingTip();
        //        return;
        //    }

        //    if (!(await CheckPassword()))
        //        return;

        //    var removeUserContentDialog = new RemoveUserConfirmationContentDialog(this.XamlRoot, viewModel.SelectedEmployee.FullName);

        //    await removeUserContentDialog.ShowAsync();

        //    if (removeUserContentDialog.Result != Record_System.ContentDialogs.Result.ContentDialogsResult.Done)
        //        return;

        //    App.MainViewModel.IncrementUploadTasks();

        //    await App.MainViewModel.FirestoreViewModel.RemoveUser(viewModel.SelectedEmployee);

        //    App.MainViewModel.DecrementUploadTasks();
        //}

        private void EmployeesSplitView_PaneOpening(SplitView sender, object args)
        {
            EmployeesGridView.Padding = new Thickness(90, 21, 14, 14);
        }

        private void EmployeesSplitView_PaneClosing(SplitView sender, SplitViewPaneClosingEventArgs args)
        {
            EmployeesGridView.Padding = new Thickness(90, 21, 90, 14);
        }

        private async void PayAll_Click(object sender, RoutedEventArgs e)
        {
            var payEmployeesContentDialog = new PayEmployeesContentDialog(this.XamlRoot, App.MainViewModel.EmployeesViewModel.TotalSalary);

            await payEmployeesContentDialog.ShowAsync();

            if (payEmployeesContentDialog.Result != Record_System.ContentDialogs.Result.ContentDialogsResult.Done)
                return;

            App.MainViewModel.IncrementUploadTasks();

            await App.MainViewModel.EmployeesViewModel.PayAll();

            App.MainViewModel.DecrementUploadTasks();
        }

        private async void Pay_Click(object sender, RoutedEventArgs e)
        {
            var payEmployeesContentDialog = new PayEmployeesContentDialog(this.XamlRoot, viewModel.SelectedEmployee.SalaryPerMonth);

            await payEmployeesContentDialog.ShowAsync();

            if (payEmployeesContentDialog.Result != Record_System.ContentDialogs.Result.ContentDialogsResult.Done)
                return;

            App.MainViewModel.IncrementUploadTasks();

            await App.MainViewModel.EmployeesViewModel.PayEmployee(viewModel.SelectedEmployee);

            App.MainViewModel.DecrementUploadTasks();
        }

        //private async void ResetPasswordUserName_Click(object sender, RoutedEventArgs e)
        //{
        //    if (!App.MainViewModel.HasInternetConnection)
        //    {
        //        await rootPage.ShowOfflineTeachingTip();
        //        return;
        //    }

        //    if (!(await CheckPassword()))
        //        return;

        //    var resetConfirmationContentDialog = new ResetPasswordAndUserNameContentDialog(this.XamlRoot);

        //    await resetConfirmationContentDialog.ShowAsync();

        //    if (resetConfirmationContentDialog.Result != Record_System.ContentDialogs.Result.ContentDialogsResult.Done)
        //        return;

        //    App.MainViewModel.IncrementUploadTasks();

        //    await viewModel.SelectedEmployee.ResetPasswordAndUserName();

        //    App.MainViewModel.DecrementUploadTasks();

        //}

        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DispatcherQueue.TryEnqueue
            (
                Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal,
                () =>
                {
                    var textBox = (TextBox)sender;

                    var keyword = textBox.Text.ToLower();

                    if (keyword.Count() > 0)
                    {
                        var results = from employee in viewModel.EmployeesViewModel.Employees
                                      where
                                      employee.FullName.ToLower().Contains(keyword) ||
                                      employee.Email.ToLower().Contains(keyword) ||
                                      employee.SalaryPerMonth.ToString().ToLower().Contains(keyword) ||
                                      employee.Location.ToLower().Contains(keyword)
                                      select employee;

                        EmployeesGridView.ItemsSource = results;

                        return;
                    }

                    EmployeesGridView.ItemsSource = viewModel.EmployeesViewModel.Employees;
                }
            );
        }
    }
}
