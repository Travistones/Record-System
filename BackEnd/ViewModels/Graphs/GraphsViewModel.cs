using OxyPlot;
using Record_System.BackEnd.ObjectPropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Record_System.BackEnd.ViewModels.Graphs
{
    public class GraphsViewModel : BindableBase
    {
        public GraphsViewModel()
        {
            _initGraphs();
        }

        private void _initGraphs()
        {
            ProfitGraphDataPoints = new();
            TotalAmountUsedInTrucksDataPoints = new();
            TotalAmountUsedInOtherExpensesDataPoints = new();
            TotalSalesRevenuesDataPoints = new();
            TotalOtherRevenuesDataPoints = new();
            TotalAmountInCreditsDataPoints = new();
            TotalAmountInOrdersDataPoints = new();
            TotalAmountEarnedFromGradeAPebblesDataPoints = new();
            TotalAmountEarnedFromGradeBPebblesDataPoints = new();
            TotalAmountEarnedFromGradeCPebblesDataPoints = new();
        }

        private ObservableCollection<DataPoint> _profitGraphDataPoints;
        private ObservableCollection<DataPoint> _totalAmountUsedInTrucksDataPoints;
        private ObservableCollection<DataPoint> _totalAmountUsedInOtherExpensesDataPoints;
        private ObservableCollection<DataPoint> _totalSalesRevenuesDataPoints;
        private ObservableCollection<DataPoint> _totalOtherRevenuesDataPoints;
        private ObservableCollection<DataPoint> _totalAmountInCreditsDataPoints;
        private ObservableCollection<DataPoint> _totalAmountInOrdersDataPoints;
        private ObservableCollection<DataPoint> _totalAmountEarnedFromGradeAPebblesDataPoints;
        private ObservableCollection<DataPoint> _totalAmountEarnedFromGradeBPebblesDataPoints;
        private ObservableCollection<DataPoint> _totalAmountEarnedFromGradeCPebblesDataPoints;


        public ObservableCollection<DataPoint> TotalAmountEarnedFromGradeAPebblesDataPoints
        {
            get => _totalAmountEarnedFromGradeAPebblesDataPoints;

            private set => _totalAmountEarnedFromGradeAPebblesDataPoints = value;
        }
        
        public ObservableCollection<DataPoint> TotalAmountEarnedFromGradeBPebblesDataPoints
        {
            get => _totalAmountEarnedFromGradeBPebblesDataPoints;

            private set => _totalAmountEarnedFromGradeBPebblesDataPoints = value;
        }
        
        public ObservableCollection<DataPoint> TotalAmountEarnedFromGradeCPebblesDataPoints
        {
            get => _totalAmountEarnedFromGradeCPebblesDataPoints;

            private set => _totalAmountEarnedFromGradeCPebblesDataPoints = value;
        }

        public ObservableCollection<DataPoint> TotalAmountInOrdersDataPoints
        {
            get => _totalAmountInOrdersDataPoints;

            private set => _totalAmountInOrdersDataPoints = value;
        }

        public ObservableCollection<DataPoint> TotalAmountInCreditsDataPoints
        {
            get => _totalAmountInCreditsDataPoints;

            private set => _totalAmountInCreditsDataPoints = value;
        }

        public ObservableCollection<DataPoint> ProfitGraphDataPoints
        {
            get => _profitGraphDataPoints;

            private set => _profitGraphDataPoints = value;
        }

        public ObservableCollection<DataPoint> TotalAmountUsedInTrucksDataPoints
        {
            get => _totalAmountUsedInTrucksDataPoints;

            private set => _totalAmountUsedInTrucksDataPoints = value;
        }

        public ObservableCollection<DataPoint> TotalAmountUsedInOtherExpensesDataPoints
        {
            get => _totalAmountUsedInOtherExpensesDataPoints;

            private set => _totalAmountUsedInOtherExpensesDataPoints = value;
        }

        public ObservableCollection<DataPoint> TotalSalesRevenuesDataPoints
        {
            get => _totalSalesRevenuesDataPoints;

            private set => _totalSalesRevenuesDataPoints = value;
        }

        public ObservableCollection<DataPoint> TotalOtherRevenuesDataPoints
        {
            get => _totalOtherRevenuesDataPoints;

            private set => _totalOtherRevenuesDataPoints = value;
        }
    }
}
