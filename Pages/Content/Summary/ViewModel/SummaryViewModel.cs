using Record_System.BackEnd.ObjectPropertyChanged;
using Record_System.BackEnd.ViewModels.Graphs;
using Record_System.BackEnd.ViewModels.OverallTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Record_System.Pages.Content.Summary.ViewModel
{
    public class SummaryViewModel : BindableBase
    {
        public GraphsViewModel GraphsViewModel;

        public OverallTimeViewModel OverallTimeViewModel;

        public SummaryViewModel()
        {
            GraphsViewModel = App.MainViewModel.GraphsViewModel;

            OverallTimeViewModel = App.MainViewModel.OverallTimeViewModel;
        }
    }
}
