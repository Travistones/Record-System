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
using Record_System.ContentDialogs.Close;
using Record_System.ContentDialogs.DeleteData;
using Record_System.ContentDialogs.Result;
using Record_System.Pages.Container.Root.Page;
using Record_System.Pages.Content.Credits.ContentDialogs.ClosingUnpaid;
using Record_System.Pages.Content.Credits.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace Record_System.Pages.Content.Credits.Page
{
    public sealed partial class CreditsPage : Microsoft.UI.Xaml.Controls.Page
    {
        private CreditsPageViewModel viewModel;

        private NumberBox currentNumberBox;

        public CreditsPage()
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

        private async void Add_NewCredit(object sender, RoutedEventArgs e)
        {
            if (!App.MainViewModel.HasInternetConnection)
            {
                await rootPage.ShowOfflineTeachingTip();
                return;
            }

            if (bottomPaneSplitView.IsPaneOpen)
            {
                this.ClosePane();

                await RunOnlineTask(this.viewModel.AddToCredits());
            }
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

        private async void Delete_SelectedCredits(object sender, RoutedEventArgs e)
        {
            var deleteContentDialog = new DeleteDataContentDialog(this.XamlRoot, Table.SelectedItems.Count);

            await deleteContentDialog.ShowAsync();

            if (deleteContentDialog.Result == ContentDialogsResult.Done)
            {
                if (!App.MainViewModel.HasInternetConnection)
                {
                    await rootPage.ShowOfflineTeachingTip();
                    return;
                }

                var listOfItemsToDelete = new List<Credit>();

                foreach (var selectedItem in Table.SelectedItems)
                {
                    listOfItemsToDelete.Add((Credit)selectedItem);
                }

                await RunOnlineTask(viewModel.DeleteCredits(listOfItemsToDelete));
            }
        }

        private void Table_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            viewModel.CanDeleteCredits = (sender as DataGrid).SelectedItems.Count > 0;

            viewModel.SelectedCredit = (Credit)Table.SelectedItem;
        }

        private void NumberInput_LostFocus(object sender, RoutedEventArgs e)
        {
            (e.OriginalSource as TextBox).TextChanged -= NumberBox_TextChanged;
        }

        private void Open_BottomPane(object sender, RoutedEventArgs e)
        {
            viewModel.CreateNewCredit();
            this.OpenPane();
        }

        private async void Table_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            Credit editedCredit = (Credit)((DataGrid)sender).SelectedItem;

            if(editedCredit.IsComplete)
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
                    updatingTask = editedCredit.UpdateAccount(((TextBox)e.EditingElement).Text);

                    break;

                case "Grade":
                    updatingTask = editedCredit.UpdateGrade((Grade)((ComboBox)e.EditingElement).SelectedItem);

                    break;

                case "Volume (m\u00B3)":
                    updatingTask = editedCredit.UpdateVolume(((NumberBox)e.EditingElement).Value);

                    break;

                case "Net Paid (Tsh)":
                    updatingTask = editedCredit.UpdateNetPaid(((NumberBox)e.EditingElement).Value);

                    break;

                case "Details":
                    updatingTask = editedCredit.UpdateDetails(((TextBox)e.EditingElement).Text);

                    break;

                case "Closing Date":
                    updatingTask = editedCredit.UpdateClosingDate(((CalendarDatePicker)e.EditingElement).Date);

                    break;
            }

            if (updatingTask == null)
                return;

            await RunOnlineTask(updatingTask);
        }

        private async void CloseCredit_Click(object sender, RoutedEventArgs e)
        {
            if (!App.MainViewModel.HasInternetConnection)
            {
                await rootPage.ShowOfflineTeachingTip();
                return;
            }

            var creditToClose = viewModel.SelectedCredit;

            if (creditToClose.NetUnpaid > 0)
            {
                var closingUnpaidContentDialog = new ClosingUnpaidContentDialog(this.XamlRoot, creditToClose);

                await closingUnpaidContentDialog.ShowAsync();

                if (closingUnpaidContentDialog.Result == ContentDialogsResult.Cancel)
                    return;
            }
            else
            {
                var closeContentDialog = new CloseContentDialog(this.XamlRoot);

                await closeContentDialog.ShowAsync();

                if (closeContentDialog.Result == ContentDialogsResult.Cancel)
                    return;
            }

            var updatingTask = viewModel.SelectedCredit.Close();

            if (updatingTask == null)
                return;

            viewModel.SelectedCredit.IsUpdatingIsComplete = true;

            await RunOnlineTask(updatingTask);

            viewModel.SelectedCredit.IsUpdatingIsComplete = false;

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
                        var results = from credit in viewModel.CreditsViewModel.Credits
                                      where
                                      credit.Account.ToLower().Contains(keyword) ||
                                      credit.Details.ToLower().Contains(keyword) ||
                                      credit.Grade.Quality.ToString().ToLower().Contains(keyword) ||
                                      credit.Volume.ToString().ToLower().Contains(keyword) ||
                                      credit.NetPaid.ToString().ToLower().Contains(keyword)
                                      select credit;

                        Table.ItemsSource = results;

                        return;
                    }

                    Table.ItemsSource = viewModel.CreditsViewModel.Credits;
                }
            );
        }
    }
}
