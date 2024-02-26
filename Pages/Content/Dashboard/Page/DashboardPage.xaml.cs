using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Record_System.ApplicationWindows.Main.Window;
using Record_System.Pages.Container.SummaryAccountRoot.Page;
using Record_System.Pages.Content.Account.Page;
using Record_System.Pages.Content.Dashboard.ViewModel;
using Record_System.Pages.Content.Summary.Pages.Overall.Page;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace Record_System.Pages.Content.Dashboard.Page
{
    public sealed partial class DashboardPage : Microsoft.UI.Xaml.Controls.Page
    {
        private DashboardPageViewModel viewModel;

        public DashboardPage()
        {
            viewModel = new(this.DispatcherQueue);

            this.InitializeComponent();

            NavigationCacheMode = Microsoft.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        }

        private void Footer_Buttons_Click(object sender, RoutedEventArgs e)
        {
            Type pageType = null;
            var clickedButton = (Button)sender;

            if (clickedButton == summaryButton)
                pageType = typeof(OverallPage);
            else if (clickedButton == accountButton)
                pageType = typeof(AccountPage);
            else
                throw new Exception();

            MainWindow.RootNavigationFrameReference.Navigate(typeof(SummaryAccountPage), pageType, new DrillInNavigationTransitionInfo());
        }

    }
}
