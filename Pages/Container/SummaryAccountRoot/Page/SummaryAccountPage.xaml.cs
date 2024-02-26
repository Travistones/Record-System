using Microsoft.Graphics.Canvas.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using Record_System.Pages.Container.Root.Page;
using Record_System.Pages.Content.Account.Page;
using Record_System.Pages.Content.Summary.Pages.Overall.Page;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.WebUI;

namespace Record_System.Pages.Container.SummaryAccountRoot.Page
{
    public sealed partial class SummaryAccountPage : Microsoft.UI.Xaml.Controls.Page
    {
        public SummaryAccountPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is not Type)
                throw new Exception();

            var pageType = (Type)e.Parameter;

            string pageTitle = string.Empty;

            if (pageType == typeof(AccountPage))
                pageTitle = "Account";
            else if (pageType == typeof(OverallPage))
                pageTitle = "Summary";
            else
                throw new Exception();

            titleTextBlock.Text = pageTitle;

            summaryAccountMainFrame.Navigate(pageType, null, new DrillInNavigationTransitionInfo());
        }

        private void Home_Click(object sender, RoutedEventArgs e)
        {
            if ((this.Parent as Frame).CanGoBack)
                (this.Parent as Frame).Navigate(typeof(RootPage), null, new DrillInNavigationTransitionInfo());
        }

        private void GoBack_Close(object sender, RoutedEventArgs e)
        {
            if (!summaryAccountMainFrame.CanGoBack)
                throw new Exception();

            summaryAccountMainFrame.GoBack();
        }
    }
}
