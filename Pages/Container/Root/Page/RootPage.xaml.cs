using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Record_System.Pages.Container.Root.ViewModel;
using Record_System.Pages.Content.Credits.Page;
using Record_System.Pages.Content.Dashboard.Page;
using Record_System.Pages.Content.Employees.Page;
using Record_System.Pages.Content.Expenses.Page;
using Record_System.Pages.Content.Orders.Page;
using Record_System.Pages.Content.Revenues.Page;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.Threading.Tasks;

namespace Record_System.Pages.Container.Root.Page
{
    public sealed partial class RootPage : Microsoft.UI.Xaml.Controls.Page
    {
        RootPageViewModel viewModel;

        public RootPage()
        {
            viewModel = new();

            this.InitializeComponent();

            NavigationCacheMode = NavigationCacheMode.Enabled;
        }
        
        private void MainNavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            var selectedItem = (NavigationViewItem)args.SelectedItem;

            Type pageType = null;

            if (selectedItem == dashboardNavigationViewItem)
            {
                pageType = typeof(DashboardPage);
            }
            else if (selectedItem == revenuesNavigationViewItem)
            {
                pageType = typeof(RevenuesPage);
            }
            else if (selectedItem == expensesNavigationViewItem)
            {
                pageType = typeof(ExpensesPage);
            }
            else if(selectedItem == ordersNavigationViewItem)
            {
                pageType = typeof(OrdersPage);
            }
            else if(selectedItem == creditsNavigationViewItem)
            {
                pageType = typeof(CreditsPage);
            }
            else if(selectedItem == employeesNavigationViewItem)
            {
                pageType = typeof(EmployeesPage);
            }

            mainApplicationNavigationFrame.Navigate(pageType, this, args.RecommendedNavigationTransitionInfo);
        }

        private async Task _showTeachingTip(TeachingTip teachingTip)
        {
            if (teachingTip.IsOpen)
                return;

            teachingTip.IsOpen = true;

            await Task.Delay(millisecondsDelay: 2500);

            teachingTip.IsOpen = false;
        }

        private async Task _showWeakConnectionTeachingTip()
        {
            await _showTeachingTip(WeakConnectionTeachingTip);
        }

        private async Task _showOfflineTeachingTip()
        {
            await _showTeachingTip(OfflineTeachingTip);
        }

        private async Task _showNoEditingTeachingTip()
        {
            await _showTeachingTip(NoEditingTeachingTip);
        }

        public async Task ShowOfflineTeachingTip() => await _showOfflineTeachingTip();

        public async Task ShowWeakConnectionTeachingTip() => await _showWeakConnectionTeachingTip();

        public async Task ShowNoEditingTeachingTip() => await _showNoEditingTeachingTip();
    }
}
