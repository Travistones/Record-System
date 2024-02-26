using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Record_System.BackEnd.Database
{
    public class Year : TimeRange
    {
        public Year()
        {
            Months = new();
        }

        private ObservableCollection<Month> _months;

        public ObservableCollection<Month> Months
        {
            get => _months;

            private set => SetProperty(ref _months, value);
        }

        private int _yearNumber;

        public int YearNumber
        {
            get => _yearNumber;

            set => SetProperty(ref _yearNumber, value);
        }

        private DocumentReference _monthsDocumentReference;

        private FirestoreChangeListener _monthsDocumentChangeListener;

        public DocumentReference MonthsDocumentReference
        {
            get => _monthsDocumentReference;

            set
            {
                _monthsDocumentReference = value;

                _setMonthChangeListener();
            }
        }

        private void _setMonthChangeListener()
        {
            _monthsDocumentChangeListener = MonthsDocumentReference.Listen
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
                                var monthCollections = await snapshot.Reference.ListCollectionsAsync().ToListAsync();

                                await App.MainViewModel.FirestoreViewModel.AddNewMonthsToYear(this, monthCollections);
                            }
                        );
                    }
                }
            );
        }

    }
}
