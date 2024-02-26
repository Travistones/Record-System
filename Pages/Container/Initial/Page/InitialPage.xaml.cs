using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using Record_System.Pages.Container.Login.Page;
using Record_System.Pages.Container.Main.Page;
using Record_System.Pages.Container.Root.Page;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace Record_System.Pages.Container.Initial.Page
{
    public sealed partial class InitialPage : Microsoft.UI.Xaml.Controls.Page
    {
        public InitialPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //Retrieve Data From The Database Right Here;

            await App.MainViewModel.FirestoreViewModel.RetrieveData();

            App.MainViewModel.SettingsViewModel.ApplicationSettings.SetDatabaseListener();

            //TODO: Navigate to the first Page of the application

            (this.Parent as Frame).Navigate(typeof(RootPage), null, new DrillInNavigationTransitionInfo());
        }
    }
}
