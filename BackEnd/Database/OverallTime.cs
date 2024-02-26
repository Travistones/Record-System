using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Record_System.BackEnd.Database
{
    public class OverallTime : TimeRange
    {
        public OverallTime()
        {
            Years = new();
        }

        private ObservableCollection<Year> _years;

        public ObservableCollection<Year> Years
        {
            get => _years;

            private set => SetProperty(ref _years, value);
        }

        private DocumentReference _yearsDocumentReference;

        private FirestoreChangeListener _yearsDocumentChangeListener;

        private Microsoft.UI.Dispatching.DispatcherQueue _dispatcherQueue;

        public Microsoft.UI.Dispatching.DispatcherQueue DispatcherQueue
        {
            get => _dispatcherQueue;

            set => _dispatcherQueue = value;
        }

        public DocumentReference YearsDocumentReference
        {
            get => _yearsDocumentReference;

            set
            {
                _yearsDocumentReference = value;

                _setYearsChangeListener();
            }
        }

        private async Task _removeListener() => await _yearsDocumentChangeListener.StopAsync();

        private void _setYearsChangeListener()
        {
            _yearsDocumentChangeListener = YearsDocumentReference.Listen
            (
                snapshot =>
                {
                    if (DispatcherQueue == null)
                        return;

                    if (snapshot.Exists)
                    {
                        DispatcherQueue.TryEnqueue
                        (
                            Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, async () =>
                            {
                                var yearCollections = await snapshot.Reference.ListCollectionsAsync().ToListAsync();

                                await App.MainViewModel.FirestoreViewModel.AddNewYearsToOverall(yearCollections);
                            }
                        );
                    }
                }
            );
        }

    }
}
