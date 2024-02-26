using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using Record_System.Pages.Container.Initial.Page;
using Record_System.Pages.Container.Login.Page;
using Record_System.Pages.Container.Root.Page;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace Record_System.Pages.Container.Main.Page
{
    public sealed partial class MainPage : Microsoft.UI.Xaml.Controls.Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.ContentMainFrame.Navigate(typeof(LoginPage), null, new DrillInNavigationTransitionInfo());
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            App.MainViewModel.DispatcherQueue = this.DispatcherQueue;

            App.MainViewModel.InitTimer();

            App.MainViewModel.InitializeViewModels();
        }
    }
}
