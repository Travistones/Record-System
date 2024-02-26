using Record_System.BackEnd.ViewModels.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Record_System.Pages.Container.Root.ViewModel
{
    public class RootPageViewModel
    {
        public MainViewModel MainViewModel;

        public RootPageViewModel()
        {
            MainViewModel = App.MainViewModel;
        }
    }
}
