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
using Record_System.ContentDialogs.DeleteData;
using Record_System.Pages.Container.Root.Page;
using Record_System.Pages.Content.Revenues.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace Record_System.Pages.Content.Revenues.Page
{
    public sealed partial class RevenuesPage : Microsoft.UI.Xaml.Controls.Page
    {
        List<string> accounts = new List<string>
        {
            "Alex",
            "Petro",
            "Glory",
            "Innocent",
            "Petter"
        };

        private RevenuesPageViewModel viewModel;

        private NumberBox currentNumberBox = null;

        public RevenuesPage()
        {
            this.DataContext = this;

            viewModel = new();

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

        private void Open_BottomPane(object sender, RoutedEventArgs e)
        {
            viewModel.CreateNewRevenue();
            this.OpenPane();
        }

        private void accountInput_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var suitableItems = new List<string>();
                var splitText = sender.Text.ToLower().Split(" ");
                foreach (var account in accounts)
                {
                    var found = splitText.All((key) =>
                    {
                        return account.ToLower().Contains(key);
                    });
                    if (found)
                    {
                        suitableItems.Add(account);
                    }
                }

                sender.ItemsSource = suitableItems;
            }

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
            if((sender as TextBox).Text.IsDecimal())
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
            this.ClosePane();
        }

        private async void Add_NewRevenue(object sender, RoutedEventArgs e)
        {
            if (!App.MainViewModel.HasInternetConnection)
            {
                await rootPage.ShowOfflineTeachingTip();
                return;
            }

            if (bottomPaneSplitView.IsPaneOpen)
            {
                this.ClosePane();

                await RunOnlineTask(this.viewModel.AddToRevenues());
            }
        }

        private void Table_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            viewModel.CanDeleteRevenues = (sender as DataGrid).SelectedItems.Count > 0;
        }

        private async void Delete_SelectedRevenues(object sender, RoutedEventArgs e)
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

                var listOfItemsToDelete = new List<Revenue>();

                foreach (var selectedItem in Table.SelectedItems)
                {
                    listOfItemsToDelete.Add((Revenue)selectedItem);
                }
                await RunOnlineTask(viewModel.DeleteRevenues(listOfItemsToDelete));
            }
        }

        private void AddDataFormToggleButton_Click(object sender, RoutedEventArgs e)
        {
            var toggleButton = (sender as ToggleButton);

            if(toggleButton == SalesRevenueToggleButton)
            {
                viewModel.NewRevenue.IsSale = true;
            }
            else if(toggleButton == OtherTypeOfRevenuesToggleButton)
            {
                viewModel.NewRevenue.IsSale = false;
            }

            toggleButton.IsChecked = true;
        }

        private async void Table_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            Revenue editedRevenue = (Revenue)((DataGrid)sender).SelectedItem;

            if (!editedRevenue.CanEdit)
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

            switch(columnHeader)
            {
                case "Account":
                    updatingTask = editedRevenue.UpdateAccount(((TextBox)e.EditingElement).Text);

                    break;

                case "Grade":
                    if (!editedRevenue.IsSale)
                        return;

                    updatingTask = editedRevenue.UpdateGrade((Grade)((ComboBox)e.EditingElement).SelectedItem);

                    break;

                case "Volume (m\u00B3)":
                    if (!editedRevenue.IsSale)
                        return;

                    updatingTask = editedRevenue.UpdateVolume(((NumberBox)e.EditingElement).Value);

                    break;

                case "Discount (Tsh)":
                    if (!editedRevenue.IsSale)
                        return;

                    updatingTask = editedRevenue.UpdateDiscount(((NumberBox)e.EditingElement).Value);

                    break;

                case "Details":
                    updatingTask = editedRevenue.UpdateDetails(((TextBox)e.EditingElement).Text);

                    break;
            }

            if (updatingTask == null)
                return;

            await RunOnlineTask(updatingTask);
        }

        private void Table_Sorting(object sender, DataGridColumnEventArgs e)
        {
            
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
                        var results = from revenue in viewModel.RevenuesViewModel.Revenues
                                      where
                                      revenue.Account.ToLower().Contains(keyword) ||
                                      revenue.Details.ToLower().Contains(keyword) ||
                                      revenue.Grade.Quality.ToString().ToLower().Contains(keyword) ||
                                      revenue.Volume.ToString().ToLower().Contains(keyword) ||
                                      revenue.NetReceived.ToString().ToLower().Contains(keyword)
                                      select revenue;

                        Table.ItemsSource = results;

                        return;
                    }

                    Table.ItemsSource = viewModel.RevenuesViewModel.Revenues;
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
