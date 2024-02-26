using Record_System.BackEnd.Database;
using Record_System.BackEnd.ObjectPropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Record_System.Pages.Content.Dashboard.ViewModel
{
    public class DashboardPageViewModel : BindableBase
    {
        private Microsoft.UI.Dispatching.DispatcherQueue _dispatcherQueue;

        public OverallTime OverallTime;

        public DashboardPageViewModel(Microsoft.UI.Dispatching.DispatcherQueue _dispatcherQueue)
        {
            this._dispatcherQueue = _dispatcherQueue;

            OverallTime = App.MainViewModel.OverallTimeViewModel.OverallTime;

            _updateToday();

            _initTimer();
        }

        private Timer dashboardTimer;

        private void _initTimer()
        {
            dashboardTimer = new();
            dashboardTimer.Interval = 30000;
            dashboardTimer.Elapsed += Timer_Elapsed;
            dashboardTimer.Enabled = true;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _updateToday();
        }

        private Day _today;

        public Day Today
        {
            get => _today;

            private set => SetProperty(ref _today, value);
        }

        private string _todayDateString;

        public string TodayDateString
        {
            get => _todayDateString;

            private set => SetProperty(ref _todayDateString, value);
        }

        private void _updateTodayDateString(DateTime nowDateTime)
        {
            TodayDateString = "Today " + nowDateTime.ToString("(MMMM dd, yyyy)");
        }

        private void _updateToday()
        {
            var currentTime = DateTime.Now;

            _dispatcherQueue.TryEnqueue
            (
                Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () =>
                {
                    try
                    {
                        var year =
                            (
                                from y in OverallTime.Years
                                where y.YearNumber == currentTime.Year
                                select y
                            )
                            .ToList()[0];

                        var month =
                            (
                                from m in year.Months
                                where m.MonthName == currentTime.ToString("MMMM")
                                select m
                            )
                            .ToList()[0];

                        Today =
                            (
                                from d in month.Days
                                where d.DayNumber == currentTime.Day
                                select d
                            )
                            .ToList()[0];
                    }
                    catch
                    {
                        if (Today == null)
                            Today = new Day();
                    }

                    _updateTodayDateString(currentTime);
                }
            );
            
        }
    }
}
