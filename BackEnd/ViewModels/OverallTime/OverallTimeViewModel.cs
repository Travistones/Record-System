using Record_System.BackEnd.ObjectPropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Record_System.BackEnd.ViewModels.OverallTime
{
    public class OverallTimeViewModel : BindableBase
    {
        public OverallTimeViewModel()
        {
            OverallTime = new();
        }

        private Database.OverallTime _overallTime;

        public Database.OverallTime OverallTime
        {
            get => _overallTime;

            private set => _overallTime = value;
        }
    }
}
