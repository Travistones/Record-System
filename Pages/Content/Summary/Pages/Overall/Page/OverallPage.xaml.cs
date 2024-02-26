using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Record_System.Pages.Content.Summary.Pages.Trucks.Page;
using Record_System.Pages.Content.Summary.Pages.Pebbles.Page;
using Record_System.Pages.Content.Summary.Pages.Revenues.Page;
using Record_System.Pages.Content.Summary.Pages.Expenses.Page;
using Record_System.Pages.Content.Summary.Pages.Credits.Page;
using Record_System.Pages.Content.Summary.Pages.Orders.Page;
using Microsoft.UI.Xaml.Media.Animation;
using System.Collections.ObjectModel;
using Record_System.Pages.Content.Summary.ViewModel;

namespace Record_System.Pages.Content.Summary.Pages.Overall.Page
{
    public sealed partial class OverallPage : Microsoft.UI.Xaml.Controls.Page
    {
        SummaryViewModel viewModel;

        public OverallPage()
        {
            viewModel = new();

            this.InitializeComponent();

            NavigationCacheMode = NavigationCacheMode.Enabled;

            Color blackColor50 = (Color)Application.Current.Resources["BlackColor50"];
            ProfitGraphLineSeries.Color = OxyColor.FromArgb(blackColor50.A, blackColor50.R, blackColor50.G, blackColor50.B);
            ProfitGraphPlotModel.PlotAreaBorderThickness = new OxyThickness(0);
        }

        private void summaryItems_Click(object sender, ItemClickEventArgs e)
        {
            Type pageType;

            var clickedItem = (e.ClickedItem as UserControl);

            if (clickedItem == trucksListViewItem)
                pageType = typeof(TrucksSummaryPage);
            else if (clickedItem == pebblesListViewItem)
                pageType = typeof(PebblesSummaryPage);
            else if (clickedItem == revenuesListViewItem)
                pageType = typeof(RevenuesSummaryPage);
            else if (clickedItem == expensesListViewItem)
                pageType = typeof(ExpensesSummaryPage);
            else if (clickedItem == ordersListViewItem)
                pageType = typeof(OrderSummaryPage);
            else if (clickedItem == creditsListViewItem)
                pageType = typeof(CreditsSummaryPage);
            else
                throw new Exception();

            (this.Parent as Frame).Navigate(pageType, null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
        }

        private void trucksSummaryItemPointerExited(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState((sender as Control), "TurcksSummaryItemNormalState", false);
        }

        private void trucksSummaryItemPointerOver(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == Microsoft.UI.Input.PointerDeviceType.Mouse)
                VisualStateManager.GoToState((sender as Control), "TrucksSummaryItemHoveredState", true);
        }

        private void pebblesSummaryItemPointerExited(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState((sender as Control), "PebblesSummaryItemNormalState", false);
        }

        private void pebblesSummaryItemPointerOver(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == Microsoft.UI.Input.PointerDeviceType.Mouse)
                VisualStateManager.GoToState((sender as Control), "PebblesSummaryItemHoveredState", true);
        }

        private void revenuesSummaryItemPointerExited(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState((sender as Control), "RevenuesSummaryItemNormalState", false);
        }

        private void revenuesSummaryItemPointerOver(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == Microsoft.UI.Input.PointerDeviceType.Mouse)
                VisualStateManager.GoToState((sender as Control), "RevenuesSummaryItemHoveredState", true);
        }

        private void expensesSummaryItemPointerExited(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState((sender as Control), "ExpensesSummaryItemNormalState", false);
        }

        private void expensesSummaryItemPointerOver(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == Microsoft.UI.Input.PointerDeviceType.Mouse)
                VisualStateManager.GoToState((sender as Control), "ExpensesSummaryItemHoveredState", true);
        }

        private void ordersSummaryItemPointerExited(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState((sender as Control), "OrdersSummaryItemNormalState", false);
        }

        private void ordersSummaryItemPointerOver(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == Microsoft.UI.Input.PointerDeviceType.Mouse)
                VisualStateManager.GoToState((sender as Control), "OrdersSummaryItemHoveredState", true);
        }

        private void creditsSummaryItemPointerExited(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState((sender as Control), "CreditsSummaryItemNormalState", false);
        }

        private void creditsSummaryItemPointerOver(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == Microsoft.UI.Input.PointerDeviceType.Mouse)
                VisualStateManager.GoToState((sender as Control), "CreditsSummaryItemHoveredState", true);
        }

    }
}
