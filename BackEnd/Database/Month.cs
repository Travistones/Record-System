using Google.Cloud.Firestore;
using Record_System.BackEnd.ViewModels.Main;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Record_System.BackEnd.Database
{
    public class Month : TimeRange
    {
        public Month()
        {
            Days = new();
        }

        private ObservableCollection<Day> _days;

        public ObservableCollection<Day> Days
        {
            get => _days;

            private set => SetProperty(ref _days, value);
        }

        private string _monthName;

        public string MonthName
        {
            get => _monthName;

            set => SetProperty(ref _monthName, value);
        }

        private DocumentReference _daysDocumentReference;

        private FirestoreChangeListener _daysDocumentChangeListener;

        public DocumentReference DaysDocumentReference
        {
            get => _daysDocumentReference;

            set
            {
                _daysDocumentReference = value;

                _setDaysChangeListener();
            }
        }

        private void _setDaysChangeListener()
        {
            _daysDocumentChangeListener = DaysDocumentReference.Listen
            (
                snapshot =>
                {
                    if (App.MainViewModel.OverallTimeViewModel.OverallTime.DispatcherQueue == null)
                        return;

                    if (snapshot.Exists)
                    {
                        App.MainViewModel.OverallTimeViewModel.OverallTime.DispatcherQueue.TryEnqueue
                        (
                            Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, async () =>
                            {
                                var dayCollections = await snapshot.Reference.ListCollectionsAsync().ToListAsync();

                                await App.MainViewModel.FirestoreViewModel.AddNewDaysToMonth(this, dayCollections);
                            }
                        );
                    }
                }
            );
        }

    }
}
