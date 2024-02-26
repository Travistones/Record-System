using Google.Cloud.Firestore;
using Record_System.BackEnd.Data;
using Record_System.BackEnd.ObjectPropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Axes;
using System.Collections.ObjectModel;

namespace Record_System.BackEnd.Database
{
    public class TimeRange : BindableBase
    {
        public TimeRange()
        {
            
        }

        private DocumentReference _totalsDocumentReference;

        public DocumentReference TotalsDocumentReference
        {
            get => _totalsDocumentReference;

            set
            {
                _totalsDocumentReference = value;

                _setTotalsChangeListener();
            }
        }

        private FirestoreChangeListener _totalsDocumentChangeListener;

        private void _setTotalsChangeListener()
        {
            _totalsDocumentChangeListener = TotalsDocumentReference.Listen
            (
                snapshot =>
                {
                    if (App.MainViewModel.OverallTimeViewModel.OverallTime.DispatcherQueue == null)
                        return;

                    if (snapshot.Exists)
                    {
                        _ = App.MainViewModel.OverallTimeViewModel.OverallTime.DispatcherQueue.TryEnqueue
                        (
                            Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () =>
                            {
                                Dictionary<string, object> totalsDictionary = snapshot.ToDictionary();

                                App.MainViewModel.FirestoreViewModel.SetTotals(totalsDictionary, this);
                            }
                        );
                    }
                }
            );
        }

        private DateTime _lastModified;
        private DateTime _day;
        private double _totalAmountEarnedFromGradeAPebbles;
        private double _totalAmountEarnedFromGradeBPebbles;
        private double _totalAmountEarnedFromGradeCPebbles;
        private double _totalAmountEarnedFromPebbles;
        private double _totalAmountInCredits;
        private double _totalAmountInIncompleteCredits;
        private double _totalAmountInCompleteCredits;
        private double _totalAmountInIncompleteOrders;
        private double _totalAmountInCompleteOrders;
        private double _totalAmountInOrders;
        private double _totalExpenses;
        private double _totalAmountInOtherExpenses;
        private double _totalAmountOfPebblesACredits;
        private double _totalAmountOfPebblesBCredits;
        private double _totalAmountOfPebblesCCredits;
        private double _totalAmountOfPebblesAInIncompleteCredits;
        private double _totalAmountOfPebblesBInIncompleteCredits;
        private double _totalAmountOfPebblesCInIncompleteCredits;
        private double _totalAmountOfPebblesAInCompleteCredits;
        private double _totalAmountOfPebblesBInCompleteCredits;
        private double _totalAmountOfPebblesCInCompleteCredits;
        private double _totalAmountOfPebblesAInIncompleteOrders;
        private double _totalAmountOfPebblesBInIncompleteOrders;
        private double _totalAmountOfPebblesCInIncompleteOrders;
        private double _totalAmountOfPebblesAInCompleteOrders;
        private double _totalAmountOfPebblesBInCompleteOrders;
        private double _totalAmountOfPebblesCInCompleteOrders;
        private double _totalAmountOfPebblesAOrders;
        private double _totalAmountOfPebblesBOrders;
        private double _totalAmountOfPebblesCOrders;
        private double _totalAmountUsedInTrucks;
        private double _totalNumberOfCredits;
        private double _totalNumberOfIncompleteCredits;
        private double _totalNumberOfCompleteCredits;
        private double _totalNumberOfIncompleteOrders;
        private double _totalNumberOfCompleteOrders;
        private double _totalNumberOfOrders;
        private double _totalNumberOfPebblesACredits;
        private double _totalNumberOfPebblesBCredits;
        private double _totalNumberOfPebblesCCredits;
        private double _totalNumberOfPebblesAInIncompleteCredits;
        private double _totalNumberOfPebblesBInIncompleteCredits;
        private double _totalNumberOfPebblesCInIncompleteCredits;
        private double _totalNumberOfPebblesAInCompleteCredits;
        private double _totalNumberOfPebblesBInCompleteCredits;
        private double _totalNumberOfPebblesCInCompleteCredits;
        private double _totalNumberOfPebblesAInIncompleteOrders;
        private double _totalNumberOfPebblesBInIncompleteOrders;
        private double _totalNumberOfPebblesCInIncompleteOrders;
        private double _totalNumberOfPebblesAInCompleteOrders;
        private double _totalNumberOfPebblesBInCompleteOrders;
        private double _totalNumberOfPebblesCInCompleteOrders;
        private double _totalNumberOfPebblesAOrders;
        private double _totalNumberOfPebblesBOrders;
        private double _totalNumberOfPebblesCOrders;
        private double _totalOtherRevenues;
        private double _totalRevenues;
        private double _totalSalesRevenues;
        private double _totalTrucksIn;
        private double _totalVolumeInCredits;
        private double _totalVolumeInIncompleteCredits;
        private double _totalVolumeInCompleteCredits;
        private double _totalVolumeInIncompleteOrders;
        private double _totalVolumeInCompleteOrders;
        private double _totalVolumeInOrders;
        private double _totalVolumeOfGradeAPebbles;
        private double _totalVolumeOfGradeBPebbles;
        private double _totalVolumeOfGradeCPebbles;
        private double _totalVolumeOfPebbles;
        private double _totalVolumeOfPebblesACredits;
        private double _totalVolumeOfPebblesBCredits;
        private double _totalVolumeOfPebblesCCredits;
        private double _totalVolumeOfPebblesAInIncompleteCredits;
        private double _totalVolumeOfPebblesBInIncompleteCredits;
        private double _totalVolumeOfPebblesCInIncompleteCredits;
        private double _totalVolumeOfPebblesAInCompleteCredits;
        private double _totalVolumeOfPebblesBInCompleteCredits;
        private double _totalVolumeOfPebblesCInCompleteCredits;
        private double _totalVolumeOfPebblesAInIncompleteOrders;
        private double _totalVolumeOfPebblesBInIncompleteOrders;
        private double _totalVolumeOfPebblesCInIncompleteOrders;
        private double _totalVolumeOfPebblesAInCompleteOrders;
        private double _totalVolumeOfPebblesBInCompleteOrders;
        private double _totalVolumeOfPebblesCInCompleteOrders;
        private double _totalVolumeOfPebblesAOrders;
        private double _totalVolumeOfPebblesBOrders;
        private double _totalVolumeOfPebblesCOrders;
        private double _netProfit;
        private DataPoint _netProfitDataPoint;
        private DataPoint _totalAmountUsedInTrucksDataPoint;
        private DataPoint _totalAmountInOtherExpensesDataPoint;
        private DataPoint _totalSalesRevenuesDataPoint;
        private DataPoint _totalOtherRevenuesDataPoint;
        private DataPoint _totalAmountInCreditsDataPoint;
        private DataPoint _totalAmountInOrdersDataPoint;
        private DataPoint _totalAmountEarnedFromGradeAPebblesDataPoint;
        private DataPoint _totalAmountEarnedFromGradeBPebblesDataPoint;
        private DataPoint _totalAmountEarnedFromGradeCPebblesDataPoint;

        public DataPoint TotalAmountEarnedFromGradeAPebblesDataPoint
        {
            get => _totalAmountEarnedFromGradeAPebblesDataPoint;

            set => SetProperty(ref _totalAmountEarnedFromGradeAPebblesDataPoint, value);
        }
        
        public DataPoint TotalAmountEarnedFromGradeBPebblesDataPoint
        {
            get => _totalAmountEarnedFromGradeBPebblesDataPoint;

            set => SetProperty(ref _totalAmountEarnedFromGradeBPebblesDataPoint, value);
        }
        
        public DataPoint TotalAmountEarnedFromGradeCPebblesDataPoint
        {
            get => _totalAmountEarnedFromGradeCPebblesDataPoint;

            set => SetProperty(ref _totalAmountEarnedFromGradeCPebblesDataPoint, value);
        }

        public DataPoint TotalAmountInCreditsDataPoint
        {
            get => _totalAmountInCreditsDataPoint;

            set => SetProperty(ref _totalAmountInCreditsDataPoint, value);
        }

        public DataPoint TotalAmountInOrdersDataPoint
        {
            get => _totalAmountInOrdersDataPoint;

            set => SetProperty(ref _totalAmountInOrdersDataPoint, value);
        }

        public DataPoint TotalAmountInOtherExpensesDataPoint
        {
            get => _totalAmountInOtherExpensesDataPoint;

            set => SetProperty(ref _totalAmountInOtherExpensesDataPoint, value);
        }

        public DataPoint TotalSalesRevenuesDataPoint
        {
            get => _totalSalesRevenuesDataPoint;

            set => SetProperty(ref _totalSalesRevenuesDataPoint, value);
        }

        public DataPoint TotalOtherRevenuesDataPoint
        {
            get => _totalOtherRevenuesDataPoint;

            set => SetProperty(ref _totalOtherRevenuesDataPoint, value);
        }

        public DataPoint TotalAmountUsedInTrucksDataPoint
        {
            get => _totalAmountUsedInTrucksDataPoint;

            set => SetProperty(ref _totalAmountUsedInTrucksDataPoint, value);
        }

        public DataPoint NetProfitDataPoint
        {
            get => _netProfitDataPoint;

            set
            {
                SetProperty(ref _netProfitDataPoint, value);
            }
        }

        public DateTime LastModified
        {
            get => _lastModified;

            set
            {
                SetProperty(ref _lastModified, value);
            }
        }

        public DateTime Day
        {
            get => _day;

            set
            {
                SetProperty(ref _day, value);
            }
        }

        public double TotalAmountEarnedFromGradeAPebbles
        {
            get => _totalAmountEarnedFromGradeAPebbles;

            set
            {
                SetProperty(ref _totalAmountEarnedFromGradeAPebbles, value);

                updateGraph(value, App.MainViewModel.GraphsViewModel.TotalAmountEarnedFromGradeAPebblesDataPoints);
            }
        }
        
        public double TotalAmountEarnedFromGradeBPebbles
        {
            get => _totalAmountEarnedFromGradeBPebbles;

            set
            {
                SetProperty(ref _totalAmountEarnedFromGradeBPebbles, value);

                updateGraph(value, App.MainViewModel.GraphsViewModel.TotalAmountEarnedFromGradeBPebblesDataPoints);
            }
        }
        
        public double TotalAmountEarnedFromGradeCPebbles
        {
            get => _totalAmountEarnedFromGradeCPebbles;

            set
            {
                SetProperty(ref _totalAmountEarnedFromGradeCPebbles, value);

                updateGraph(value, App.MainViewModel.GraphsViewModel.TotalAmountEarnedFromGradeCPebblesDataPoints);
            }
        }

        public double TotalAmountEarnedFromPebbles
        {
            get => _totalAmountEarnedFromPebbles;

            set
            {
                SetProperty(ref _totalAmountEarnedFromPebbles, value);
            }
        }

        public double TotalAmountInCredits
        {
            get => _totalAmountInCredits;

            set
            {
                SetProperty(ref _totalAmountInCredits, value);

                updateGraph(value, App.MainViewModel.GraphsViewModel.TotalAmountInCreditsDataPoints);
            }
        }

        public double TotalAmountInIncompleteCredits
        {
            get => _totalAmountInIncompleteCredits;

            set
            {
                SetProperty(ref _totalAmountInIncompleteCredits, value);
            }
        }

        public double TotalAmountInCompleteCredits
        {
            get => _totalAmountInCompleteCredits;

            set
            {
                SetProperty(ref _totalAmountInCompleteCredits, value);
            }
        }

        public double TotalAmountInIncompleteOrders
        {
            get => _totalAmountInIncompleteOrders;

            set
            {
                SetProperty(ref _totalAmountInIncompleteOrders, value);
            }
        }
        
        public double TotalAmountInCompleteOrders
        {
            get => _totalAmountInCompleteOrders;

            set
            {
                SetProperty(ref _totalAmountInCompleteOrders, value);
            }
        }
        
        public double TotalAmountInOrders
        {
            get => _totalAmountInOrders;

            set
            {
                SetProperty(ref _totalAmountInOrders, value);

                updateGraph(value, App.MainViewModel.GraphsViewModel.TotalAmountInOrdersDataPoints);
            }
        }

        public double TotalExpenses
        {
            get => _totalExpenses;

            set
            {
                SetProperty(ref _totalExpenses, value);
            }
        }

        public double TotalAmountInOtherExpenses
        {
            get => _totalAmountInOtherExpenses;

            set
            {
                SetProperty(ref _totalAmountInOtherExpenses, value);

                updateGraph(value, App.MainViewModel.GraphsViewModel.TotalAmountUsedInOtherExpensesDataPoints);
            }
        }

        public double TotalAmountOfPebblesACredits
        {
            get => _totalAmountOfPebblesACredits;

            set
            {
                SetProperty(ref _totalAmountOfPebblesACredits, value);
            }
        }
        
        public double TotalAmountOfPebblesBCredits
        {
            get => _totalAmountOfPebblesBCredits;

            set
            {
                SetProperty(ref _totalAmountOfPebblesBCredits, value);
            }
        }
        
        public double TotalAmountOfPebblesCCredits
        {
            get => _totalAmountOfPebblesCCredits;

            set
            {
                SetProperty(ref _totalAmountOfPebblesCCredits, value);
            }
        }
        
        public double TotalAmountOfPebblesAInIncompleteCredits
        {
            get => _totalAmountOfPebblesAInIncompleteCredits;

            set
            {
                SetProperty(ref _totalAmountOfPebblesAInIncompleteCredits, value);
            }
        }
        
        public double TotalAmountOfPebblesBInIncompleteCredits
        {
            get => _totalAmountOfPebblesBInIncompleteCredits;

            set
            {
                SetProperty(ref _totalAmountOfPebblesBInIncompleteCredits, value);
            }
        }
        
        public double TotalAmountOfPebblesCInIncompleteCredits
        {
            get => _totalAmountOfPebblesCInIncompleteCredits;

            set
            {
                SetProperty(ref _totalAmountOfPebblesCInIncompleteCredits, value);
            }
        }
        
        public double TotalAmountOfPebblesAInCompleteCredits
        {
            get => _totalAmountOfPebblesAInCompleteCredits;

            set
            {
                SetProperty(ref _totalAmountOfPebblesAInCompleteCredits, value);
            }
        }
        
        public double TotalAmountOfPebblesBInCompleteCredits
        {
            get => _totalAmountOfPebblesBInCompleteCredits;

            set
            {
                SetProperty(ref _totalAmountOfPebblesBInCompleteCredits, value);
            }
        }
        
        public double TotalAmountOfPebblesCInCompleteCredits
        {
            get => _totalAmountOfPebblesCInCompleteCredits;

            set
            {
                SetProperty(ref _totalAmountOfPebblesCInCompleteCredits, value);
            }
        }
        
        public double TotalAmountOfPebblesAInIncompleteOrders
        {
            get => _totalAmountOfPebblesAInIncompleteOrders;

            set
            {
                SetProperty(ref _totalAmountOfPebblesAInIncompleteOrders, value);
            }
        }
        
        public double TotalAmountOfPebblesBInIncompleteOrders
        {
            get => _totalAmountOfPebblesBInIncompleteOrders;

            set
            {
                SetProperty(ref _totalAmountOfPebblesBInIncompleteOrders, value);
            }
        }
        
        public double TotalAmountOfPebblesCInIncompleteOrders
        {
            get => _totalAmountOfPebblesCInIncompleteOrders;

            set
            {
                SetProperty(ref _totalAmountOfPebblesCInIncompleteOrders, value);
            }
        }
        
        public double TotalAmountOfPebblesAInCompleteOrders
        {
            get => _totalAmountOfPebblesAInCompleteOrders;

            set
            {
                SetProperty(ref _totalAmountOfPebblesAInCompleteOrders, value);
            }
        }
        
        public double TotalAmountOfPebblesBInCompleteOrders
        {
            get => _totalAmountOfPebblesBInCompleteOrders;

            set
            {
                SetProperty(ref _totalAmountOfPebblesBInCompleteOrders, value);
            }
        }
        
        public double TotalAmountOfPebblesCInCompleteOrders
        {
            get => _totalAmountOfPebblesCInCompleteOrders;

            set
            {
                SetProperty(ref _totalAmountOfPebblesCInCompleteOrders, value);
            }
        }

        public double TotalAmountOfPebblesAOrders
        {
            get => _totalAmountOfPebblesAOrders;

            set
            {
                SetProperty(ref _totalAmountOfPebblesAOrders, value);
            }
        }
        
        public double TotalAmountOfPebblesBOrders
        {
            get => _totalAmountOfPebblesBOrders;

            set
            {
                SetProperty(ref _totalAmountOfPebblesBOrders, value);
            }
        }
        
        public double TotalAmountOfPebblesCOrders
        {
            get => _totalAmountOfPebblesCOrders;

            set
            {
                SetProperty(ref _totalAmountOfPebblesCOrders, value);
            }
        }

        public double TotalAmountUsedInTrucks
        {
            get => _totalAmountUsedInTrucks;

            set
            {
                SetProperty(ref _totalAmountUsedInTrucks, value);

                updateGraph(value, App.MainViewModel.GraphsViewModel.TotalAmountUsedInTrucksDataPoints);
            }
        }

        public double TotalNumberOfCredits
        {
            get => _totalNumberOfCredits;

            set
            {
                SetProperty(ref _totalNumberOfCredits, value);
            }
        }
        
        public double TotalNumberOfIncompleteCredits
        {
            get => _totalNumberOfIncompleteCredits;

            set
            {
                SetProperty(ref _totalNumberOfIncompleteCredits, value);
            }
        }
        
        public double TotalNumberOfCompleteCredits
        {
            get => _totalNumberOfCompleteCredits;

            set
            {
                SetProperty(ref _totalNumberOfCompleteCredits, value);
            }
        }
        
        public double TotalNumberOfIncompleteOrders
        {
            get => _totalNumberOfIncompleteOrders;

            set
            {
                SetProperty(ref _totalNumberOfIncompleteOrders, value);
            }
        }
        
        public double TotalNumberOfCompleteOrders
        {
            get => _totalNumberOfCompleteOrders;

            set
            {
                SetProperty(ref _totalNumberOfCompleteOrders, value);
            }
        }

        public double TotalNumberOfOrders
        {
            get => _totalNumberOfOrders;

            set
            {
                SetProperty(ref _totalNumberOfOrders, value);
            }
        }

        public double TotalNumberOfPebblesACredits
        {
            get => _totalNumberOfPebblesACredits;

            set
            {
                SetProperty(ref _totalNumberOfPebblesACredits, value);
            }
        }
        
        public double TotalNumberOfPebblesBCredits
        {
            get => _totalNumberOfPebblesBCredits;

            set
            {
                SetProperty(ref _totalNumberOfPebblesBCredits, value);
            }
        }
        
        public double TotalNumberOfPebblesCCredits
        {
            get => _totalNumberOfPebblesCCredits;

            set
            {
                SetProperty(ref _totalNumberOfPebblesCCredits, value);
            }
        }
        
        public double TotalNumberOfPebblesAInIncompleteCredits
        {
            get => _totalNumberOfPebblesAInIncompleteCredits;

            set
            {
                SetProperty(ref _totalNumberOfPebblesAInIncompleteCredits, value);
            }
        }
        
        public double TotalNumberOfPebblesBInIncompleteCredits
        {
            get => _totalNumberOfPebblesBInIncompleteCredits;

            set
            {
                SetProperty(ref _totalNumberOfPebblesBInIncompleteCredits, value);
            }
        }
        
        public double TotalNumberOfPebblesCInIncompleteCredits
        {
            get => _totalNumberOfPebblesCInIncompleteCredits;

            set
            {
                SetProperty(ref _totalNumberOfPebblesCInIncompleteCredits, value);
            }
        }
        
        public double TotalNumberOfPebblesAInCompleteCredits
        {
            get => _totalNumberOfPebblesAInCompleteCredits;

            set
            {
                SetProperty(ref _totalNumberOfPebblesAInCompleteCredits, value);
            }
        }
        
        public double TotalNumberOfPebblesBInCompleteCredits
        {
            get => _totalNumberOfPebblesBInCompleteCredits;

            set
            {
                SetProperty(ref _totalNumberOfPebblesBInCompleteCredits, value);
            }
        }
        
        public double TotalNumberOfPebblesCInCompleteCredits
        {
            get => _totalNumberOfPebblesCInCompleteCredits;

            set
            {
                SetProperty(ref _totalNumberOfPebblesCInCompleteCredits, value);
            }
        }
        
        public double TotalNumberOfPebblesAOrders
        {
            get => _totalNumberOfPebblesAOrders;

            set
            {
                SetProperty(ref _totalNumberOfPebblesAOrders, value);
            }
        }
        
        public double TotalNumberOfPebblesBOrders
        {
            get => _totalNumberOfPebblesBOrders;

            set
            {
                SetProperty(ref _totalNumberOfPebblesBOrders, value);
            }
        }
        
        public double TotalNumberOfPebblesCOrders
        {
            get => _totalNumberOfPebblesCOrders;

            set
            {
                SetProperty(ref _totalNumberOfPebblesCOrders, value);
            }
        }
        
        public double TotalNumberOfPebblesAInIncompleteOrders
        {
            get => _totalNumberOfPebblesAInIncompleteOrders;

            set
            {
                SetProperty(ref _totalNumberOfPebblesAInIncompleteOrders, value);
            }
        }
        
        public double TotalNumberOfPebblesBInIncompleteOrders
        {
            get => _totalNumberOfPebblesBInIncompleteOrders;

            set
            {
                SetProperty(ref _totalNumberOfPebblesBInIncompleteOrders, value);
            }
        }
        
        public double TotalNumberOfPebblesCInIncompleteOrders
        {
            get => _totalNumberOfPebblesCInIncompleteOrders;

            set
            {
                SetProperty(ref _totalNumberOfPebblesCInIncompleteOrders, value);
            }
        }
        
        public double TotalNumberOfPebblesAInCompleteOrders
        {
            get => _totalNumberOfPebblesAInCompleteOrders;

            set
            {
                SetProperty(ref _totalNumberOfPebblesAInCompleteOrders, value);
            }
        }
        
        public double TotalNumberOfPebblesBInCompleteOrders
        {
            get => _totalNumberOfPebblesBInCompleteOrders;

            set
            {
                SetProperty(ref _totalNumberOfPebblesBInCompleteOrders, value);
            }
        }
        
        public double TotalNumberOfPebblesCInCompleteOrders
        {
            get => _totalNumberOfPebblesCInCompleteOrders;

            set
            {
                SetProperty(ref _totalNumberOfPebblesCInCompleteOrders, value);
            }
        }

        public double TotalOtherRevenues
        {
            get => _totalOtherRevenues;

            set
            {
                SetProperty(ref _totalOtherRevenues, value);

                updateGraph(value, App.MainViewModel.GraphsViewModel.TotalOtherRevenuesDataPoints);
            }
        }
        
        public double TotalRevenues
        {
            get => _totalRevenues;

            set
            {
                SetProperty(ref _totalRevenues, value);
            }
        }
        
        public double TotalSalesRevenues
        {
            get => _totalSalesRevenues;

            set
            {
                SetProperty(ref _totalSalesRevenues, value);

                if (this is not OverallTime)
                {
                    this.TotalSalesRevenuesDataPoint = new DataPoint(DateTimeAxis.ToDouble(this.LastModified), value);
                }
            }
        }

        public double TotalTrucksIn
        {
            get => _totalTrucksIn;

            set
            {
                SetProperty(ref _totalTrucksIn, value);
            }
        }

        public double TotalVolumeInCredits
        {
            get => _totalVolumeInCredits;

            set
            {
                SetProperty(ref _totalVolumeInCredits, value);
            }
        }
        
        public double TotalVolumeInCompleteCredits
        {
            get => _totalVolumeInCompleteCredits;

            set
            {
                SetProperty(ref _totalVolumeInCompleteCredits, value);
            }
        }
        
        public double TotalVolumeInIncompleteCredits
        {
            get => _totalVolumeInIncompleteCredits;

            set
            {
                SetProperty(ref _totalVolumeInIncompleteCredits, value);
            }
        }
        
        public double TotalVolumeInOrders
        {
            get => _totalVolumeInOrders;

            set
            {
                SetProperty(ref _totalVolumeInOrders, value);
            }
        }
        
        public double TotalVolumeInCompleteOrders
        {
            get => _totalVolumeInCompleteOrders;

            set
            {
                SetProperty(ref _totalVolumeInCompleteOrders, value);
            }
        }
        
        public double TotalVolumeInIncompleteOrders
        {
            get => _totalVolumeInIncompleteOrders;

            set
            {
                SetProperty(ref _totalVolumeInIncompleteOrders, value);
            }
        }

        public double TotalVolumeOfPebbles
        {
            get => _totalVolumeOfPebbles;

            set
            {
                SetProperty(ref _totalVolumeOfPebbles, value);
            }
        }
        
        public double TotalVolumeOfGradeAPebbles
        {
            get => _totalVolumeOfGradeAPebbles;

            set
            {
                SetProperty(ref _totalVolumeOfGradeAPebbles, value);
            }
        }
        
        public double TotalVolumeOfGradeBPebbles
        {
            get => _totalVolumeOfGradeBPebbles;

            set
            {
                SetProperty(ref _totalVolumeOfGradeBPebbles, value);
            }
        }
        
        public double TotalVolumeOfGradeCPebbles
        {
            get => _totalVolumeOfGradeCPebbles;

            set
            {
                SetProperty(ref _totalVolumeOfGradeCPebbles, value);
            }
        }

        public double TotalVolumeOfPebblesACredits
        {
            get => _totalVolumeOfPebblesACredits;

            set
            {
                SetProperty(ref _totalVolumeOfPebblesACredits, value);
            }
        }
        
        public double TotalVolumeOfPebblesBCredits
        {
            get => _totalVolumeOfPebblesBCredits;

            set
            {
                SetProperty(ref _totalVolumeOfPebblesBCredits, value);
            }
        }
        
        public double TotalVolumeOfPebblesCCredits
        {
            get => _totalVolumeOfPebblesCCredits;

            set
            {
                SetProperty(ref _totalVolumeOfPebblesCCredits, value);
            }
        }
        
        public double TotalVolumeOfPebblesAInIncompleteCredits
        {
            get => _totalVolumeOfPebblesAInIncompleteCredits;

            set
            {
                SetProperty(ref _totalVolumeOfPebblesAInIncompleteCredits, value);
            }
        }
        
        public double TotalVolumeOfPebblesBInIncompleteCredits
        {
            get => _totalVolumeOfPebblesBInIncompleteCredits;

            set
            {
                SetProperty(ref _totalVolumeOfPebblesBInIncompleteCredits, value);
            }
        }
        
        public double TotalVolumeOfPebblesCInIncompleteCredits
        {
            get => _totalVolumeOfPebblesCInIncompleteCredits;

            set
            {
                SetProperty(ref _totalVolumeOfPebblesCInIncompleteCredits, value);
            }
        }
        
        public double TotalVolumeOfPebblesAInCompleteCredits
        {
            get => _totalVolumeOfPebblesAInCompleteCredits;

            set
            {
                SetProperty(ref _totalVolumeOfPebblesAInCompleteCredits, value);
            }
        }
        
        public double TotalVolumeOfPebblesBInCompleteCredits
        {
            get => _totalVolumeOfPebblesBInCompleteCredits;

            set
            {
                SetProperty(ref _totalVolumeOfPebblesBInCompleteCredits, value);
            }
        }
        
        public double TotalVolumeOfPebblesCInCompleteCredits
        {
            get => _totalVolumeOfPebblesCInCompleteCredits;

            set
            {
                SetProperty(ref _totalVolumeOfPebblesCInCompleteCredits, value);
            }
        }
        
        public double TotalVolumeOfPebblesAOrders
        {
            get => _totalVolumeOfPebblesAOrders;

            set
            {
                SetProperty(ref _totalVolumeOfPebblesAOrders, value);
            }
        }
        
        public double TotalVolumeOfPebblesBOrders
        {
            get => _totalVolumeOfPebblesBOrders;

            set
            {
                SetProperty(ref _totalVolumeOfPebblesBOrders, value);
            }
        }
        
        public double TotalVolumeOfPebblesCOrders
        {
            get => _totalVolumeOfPebblesCOrders;

            set
            {
                SetProperty(ref _totalVolumeOfPebblesCOrders, value);
            }
        }
        
        public double TotalVolumeOfPebblesAInIncompleteOrders
        {
            get => _totalVolumeOfPebblesAInIncompleteOrders;

            set
            {
                SetProperty(ref _totalVolumeOfPebblesAInIncompleteOrders, value);
            }
        }
        
        public double TotalVolumeOfPebblesBInIncompleteOrders
        {
            get => _totalVolumeOfPebblesBInIncompleteOrders;

            set
            {
                SetProperty(ref _totalVolumeOfPebblesBInIncompleteOrders, value);
            }
        }
        
        public double TotalVolumeOfPebblesCInIncompleteOrders
        {
            get => _totalVolumeOfPebblesCInIncompleteOrders;

            set
            {
                SetProperty(ref _totalVolumeOfPebblesCInIncompleteOrders, value);
            }
        }
        
        public double TotalVolumeOfPebblesAInCompleteOrders
        {
            get => _totalVolumeOfPebblesAInCompleteOrders;

            set
            {
                SetProperty(ref _totalVolumeOfPebblesAInCompleteOrders, value);
            }
        }
        
        public double TotalVolumeOfPebblesBInCompleteOrders
        {
            get => _totalVolumeOfPebblesBInCompleteOrders;

            set
            {
                SetProperty(ref _totalVolumeOfPebblesBInCompleteOrders, value);
            }
        }
        
        public double TotalVolumeOfPebblesCInCompleteOrders
        {
            get => _totalVolumeOfPebblesCInCompleteOrders;

            set
            {
                SetProperty(ref _totalVolumeOfPebblesCInCompleteOrders, value);
            }
        }

        public double NetProfit
        {
            get => _netProfit;

            set
            {
                SetProperty(ref _netProfit, value);

                updateGraph(value, App.MainViewModel.GraphsViewModel.ProfitGraphDataPoints);
            }
        }

        private void updateGraph(double value, ObservableCollection<DataPoint> dataPoints)
        {

            if (this is not OverallTime && this is not Year && this is not Month)
            {
                var thisDayDataPoints = (from dataPoint in dataPoints
                                   where dataPoint.X == DateTimeAxis.ToDouble(this.Day)
                                   select dataPoint).ToList();

                var isPresent = thisDayDataPoints.Count > 0;

                if (!isPresent)
                {
                    this.NetProfitDataPoint = new DataPoint(DateTimeAxis.ToDouble(this.Day), value);

                    dataPoints.Add(this.NetProfitDataPoint);

                    return;
                }

                int pointIndex = dataPoints.IndexOf(thisDayDataPoints[0]);
                
                foreach (var thisDayDataPoint in thisDayDataPoints)
                {
                    dataPoints.Remove(thisDayDataPoint);
                }

                this.NetProfitDataPoint = new DataPoint(DateTimeAxis.ToDouble(this.Day), value);

                dataPoints.Insert(pointIndex, this.NetProfitDataPoint);
            }

        }
    }
}
