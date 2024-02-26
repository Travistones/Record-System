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
using Record_System.Pages.Content.Orders.ViewModel;
using CommunityToolkit.Common;
using Record_System.BackEnd.Data;
using Record_System.ContentDialogs.DeleteData;
using CommunityToolkit.WinUI.UI.Controls;
using Record_System.BackEnd.User;
using System.Threading.Tasks;
using Record_System.Pages.Container.Root.Page;
using Record_System.ContentDialogs.Close;
using Record_System.ContentDialogs.Result;

namespace Record_System.Pages.Content.Orders.Page
{
    public sealed partial class OrdersPage : Microsoft.UI.Xaml.Controls.Page
    {
        private OrdersPageViewModel viewModel;

        private NumberBox currentNumberBox;

        public OrdersPage()
        {
            viewModel = new();
            currentNumberBox = null;

            this.SizeChanged += PageSizeChanged;
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

        private void PageSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.bottomPaneSplitView.OpenPaneLength = e.NewSize.Width;
        }

        private void ClosePane()
        {
            this.bottomPaneSplitView.IsPaneOpen = false;
        }

        private void OpenPane()
        {
            this.bottomPaneSplitView.IsPaneOpen = true;
        }

        private async void Delete_SelectedOrders(object sender, RoutedEventArgs e)
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

                var ordersToDelete = new List<Order>();

                foreach (var selectedItem in Table.SelectedItems)
                {
                    ordersToDelete.Add((Order)selectedItem);
                }

                await RunOnlineTask(viewModel.DeleteOrders(ordersToDelete));
            }
        }

        private void Open_BottomPane(object sender, RoutedEventArgs e)
        {
            viewModel.CreateNewOrder();
            this.OpenPane();
        }

        private void Table_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            viewModel.CanDeleteOrders = (sender as DataGrid).SelectedItems.Count > 0;

            viewModel.SelectedOrder = (Order)Table.SelectedItem;
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

        private void NumberInput_LostFocus(object sender, RoutedEventArgs e)
        {
            (e.OriginalSource as TextBox).TextChanged -= NumberBox_TextChanged;
        }

        private void Close_BottomPane(object sender, RoutedEventArgs e)
        {
            ClosePane();
        }

        private async void Add_NewOrder(object sender, RoutedEventArgs e)
        {
            if (!App.MainViewModel.HasInternetConnection)
            {
                await rootPage.ShowOfflineTeachingTip();
                return;
            }

            if (bottomPaneSplitView.IsPaneOpen)
            {
                this.ClosePane();

                await RunOnlineTask(this.viewModel.AddToOrders());
            }
        }

        private async void Table_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            Order editedOrder = (Order)((DataGrid)sender).SelectedItem;

            if (editedOrder.IsComplete)
            {
                await rootPage.ShowNoEditingTeachingTip();

                return;
            }

            if (!App.MainViewModel.HasInternetConnection)
            {
                await rootPage.ShowOfflineTeachingTip();
                return;
            }

            string columnHeader = e.Column.Header as string;

            Task updatingTask = null;

            switch (columnHeader)
            {
                case "Account":
                    updatingTask = editedOrder.UpdateAccount(((TextBox)e.EditingElement).Text);

                    break;

                case "Grade":
                    updatingTask = editedOrder.UpdateGrade((Grade)((ComboBox)e.EditingElement).SelectedItem);

                    break;

                case "Volume (m\u00B3)":
                    updatingTask = editedOrder.UpdateVolume(((NumberBox)e.EditingElement).Value);

                    break;

                case "Discount (Tsh)":
                    updatingTask = editedOrder.UpdateDiscount(((NumberBox)e.EditingElement).Value);

                    break;

                case "Details":
                    updatingTask = editedOrder.UpdateDetails(((TextBox)e.EditingElement).Text);

                    break;

                case "Closing Date":
                    updatingTask = editedOrder.UpdateClosingDate(((CalendarDatePicker)e.EditingElement).Date);

                    break;
            }

            if (updatingTask == null)
                return;

            await RunOnlineTask(updatingTask);
        }

        private async void CloseOrder_Click(object sender, RoutedEventArgs e)
        {
            if (!App.MainViewModel.HasInternetConnection)
            {
                await rootPage.ShowOfflineTeachingTip();
                return;
            }

            var closeContentDialog = new CloseContentDialog(this.XamlRoot);

            await closeContentDialog.ShowAsync();

            if (closeContentDialog.Result == ContentDialogsResult.Cancel)
                return;

            var updatingTask = viewModel.SelectedOrder.Close();

            if (updatingTask == null)
                return;

            viewModel.SelectedOrder.IsUpdatingIsComplete = true;

            await RunOnlineTask(updatingTask);

            viewModel.SelectedOrder.IsUpdatingIsComplete = false;
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
                        var results = from order in viewModel.OrdersViewModel.Orders
                                      where
                                      order.Account.ToLower().Contains(keyword) ||
                                      order.Details.ToLower().Contains(keyword) ||
                                      order.Grade.Quality.ToString().ToLower().Contains(keyword) ||
                                      order.Volume.ToString().ToLower().Contains(keyword) ||
                                      order.NetReceived.ToString().ToLower().Contains(keyword)
                                      select order;

                        Table.ItemsSource = results;

                        return;
                    }

                    Table.ItemsSource = viewModel.OrdersViewModel.Orders;
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
