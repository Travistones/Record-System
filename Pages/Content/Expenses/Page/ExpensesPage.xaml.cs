using CommunityToolkit.Common;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Record_System.BackEnd.Data;
using Record_System.BackEnd.User;
using Record_System.ContentDialogs.DeleteData;
using Record_System.Pages.Container.Root.Page;
using Record_System.Pages.Content.Expenses.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace Record_System.Pages.Content.Expenses.Page
{
    public sealed partial class ExpensesPage : Microsoft.UI.Xaml.Controls.Page
    {
        private ExpensesPageViewModel viewModel;

        private NumberBox currentNumberBox;

        public ExpensesPage()
        {
            currentNumberBox = null;

            viewModel = new();

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

        private void AddDataFormToggleButton_Click(object sender, RoutedEventArgs e)
        {
            var toggleButton = (sender as ToggleButton);

            if (toggleButton == TrucksToggleButton)
            {
                viewModel.NewExpense.IsTrucksExpense = true;
            }
            else if (toggleButton == OtherProductsOrServicesToggleButton)
            {
                viewModel.NewExpense.IsTrucksExpense = false;
            }

            toggleButton.IsChecked = true;
        }

        private void accountInput_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            //if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            //{
            //    var suitableItems = new List<string>();
            //    var splitText = sender.Text.ToLower().Split(" ");
            //    foreach (var account in accounts)
            //    {
            //        var found = splitText.All((key) =>
            //        {
            //            return account.ToLower().Contains(key);
            //        });
            //        if (found)
            //        {
            //            suitableItems.Add(account);
            //        }
            //    }

            //    sender.ItemsSource = suitableItems;
            //}
        }

        private void accountInput_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {

        }

        private void Close_BottomPane(object sender, RoutedEventArgs e)
        {
            this.ClosePane();
        }

        private async void Add_NewExpense(object sender, RoutedEventArgs e)
        {
            if (!App.MainViewModel.HasInternetConnection)
            {
                await rootPage.ShowOfflineTeachingTip();
                return;
            }

            if (bottomPaneSplitView.IsPaneOpen)
            {
                this.ClosePane();

                await RunOnlineTask(this.viewModel.AddToExpenses());
            }
        }

        private async void Delete_SelectedExpenses(object sender, RoutedEventArgs e)
        {
            var deleteContentDialog = new DeleteDataContentDialog(this.XamlRoot, Table.SelectedItems.Count);

            await deleteContentDialog.ShowAsync();

            if (deleteContentDialog.Result == ContentDialogs.Result.ContentDialogsResult.Done)
            {
                if (!App.MainViewModel.HasInternetConnection)
                {
                    await rootPage.ShowOfflineTeachingTip();
                    return;
                }

                var listOfItemsToDelete = new List<Expense>();

                foreach (var selectedItem in Table.SelectedItems)
                {
                    listOfItemsToDelete.Add((Expense)selectedItem);
                }

                await RunOnlineTask(viewModel.DeleteExpenses(listOfItemsToDelete));
            }
        }

        private void Table_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            viewModel.CanDeleteExpenses = (sender as DataGrid).SelectedItems.Count > 0;
        }

        private void NumberInput_LostFocus(object sender, RoutedEventArgs e)
        {
            (e.OriginalSource as TextBox).TextChanged -= NumberBox_TextChanged;
        }

        private void Open_BottomPane(object sender, RoutedEventArgs e)
        {
            viewModel.CreateNewExpense();
            this.OpenPane();
        }

        private async void Table_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            Expense editedExpense = (Expense)((DataGrid)sender).SelectedItem;

            if (!editedExpense.CanEdit)
            {
                await rootPage.ShowNoEditingTeachingTip();

                return;
            }

            if (!App.MainViewModel.HasInternetConnection)
            {
                await rootPage.ShowOfflineTeachingTip();
                return;
            }

            string columnHeader = (string)e.Column.Header;

            Task updatingTask = null;

            switch(columnHeader)
            {
                case "Account":
                    updatingTask = editedExpense.UpdateAccount(((TextBox)e.EditingElement).Text);

                    break;

                case "Product Or Service":
                    if (editedExpense.IsTrucksExpense)
                        return;

                    updatingTask = editedExpense.UpdateProductOrService(((TextBox)((Grid)e.EditingElement).Children[1]).Text);
                    
                    break;

                case "Quantity":
                    updatingTask = editedExpense.UpdateQuantity(((NumberBox)e.EditingElement).Value);

                    break;

                case "Price Per Each (Tsh)":
                    updatingTask = editedExpense.UpdatePricePerEach(((NumberBox)e.EditingElement).Value);

                    break;

                case "Net Paid (Tsh)":
                    updatingTask = editedExpense.UpdateNetPaid(((NumberBox)e.EditingElement).Value);

                    break;

                case "Details":
                    updatingTask = editedExpense.UpdateDetails(((TextBox)e.EditingElement).Text);

                    break;
            }

            if (updatingTask == null)
                return;

            await RunOnlineTask(updatingTask);
        }

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
                        var results = from expense in viewModel.ExpensesViewModel.Expenses
                                      where
                                      expense.Account.ToLower().Contains(keyword) ||
                                      expense.Details.ToLower().Contains(keyword) ||
                                      expense.ProductOrService.ToLower().Contains(keyword) ||
                                      expense.Quantity.ToString().ToLower().Contains(keyword) ||
                                      expense.NetPaid.ToString().ToLower().Contains(keyword)
                                      select expense;

                        Table.ItemsSource = results;

                        return;
                    }

                    Table.ItemsSource = viewModel.ExpensesViewModel.Expenses;
                }
            );
        }

        private async Task RunOnlineTask(Task onlineTask)
        {
            try
            {
                App.MainViewModel.IncrementUploadTasks();

                await onlineTask;

                App.MainViewModel.DecrementUploadTasks();
            }
            catch
            {
                App.MainViewModel.DecrementUploadTasks();

                await rootPage.ShowWeakConnectionTeachingTip();
            }
        }

    }
}
