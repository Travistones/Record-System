using Google.Cloud.Firestore;
using Google.Type;
using Record_System.BackEnd.Constants;
using Record_System.BackEnd.Database;
using Record_System.BackEnd.ObjectPropertyChanged;
using Record_System.BackEnd.User;
using Record_System.BackEnd.ViewModels.Main;
using Record_System.BackEnd.ViewModels.Orders;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Record_System.BackEnd.Data
{
    public class Order : BindableBase
    {
        private string _id;
        private string _account;
        private double _volume = double.NaN;
        private double _price = double.NaN;
        private double _discount = double.NaN;
        private double _netReceived = double.NaN;
        private string _details;
        private System.DateTime _time;
        private DateTimeOffset? _closingTime;
        private bool _isComplete;
        private bool _canAdd;
        private Grade _grade = App.MainViewModel.SettingsViewModel.ApplicationSettings.GradeB;
        private DocumentReference _documentReference;
        private double _gradeAPricePerVolume;
        private double _gradeBPricePerVolume;
        private double _gradeCPricePerVolume;
        private bool _isUpdatingIsComplete;
        private System.DateTimeOffset? _completedTime;

        public List<Grade> Grades { get => App.MainViewModel.SettingsViewModel.ApplicationSettings.Grades; }

        public Order() { }

        public Order(Dictionary<string, object> orderDictionary, string id)
        {
            Id = id;
            _account = (string)orderDictionary[nameof(Account)];
            _volume = (double)orderDictionary[nameof(Volume)];
            _price = (double)orderDictionary[nameof(Price)];
            _discount = (double)orderDictionary[nameof(Discount)];
            _netReceived = (double)orderDictionary[nameof(NetReceived)];
            _details = (string)orderDictionary[nameof(Details)];
            _time = System.DateTime.ParseExact((string)orderDictionary[nameof(Time)], "MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
            _closingTime = System.DateTimeOffset.ParseExact((string)orderDictionary[nameof(ClosingTime)], "MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
            _isComplete = (bool)orderDictionary[nameof(IsComplete)];
            _gradeAPricePerVolume = (double)orderDictionary[nameof(GradeAPricePerVolume)];
            _gradeBPricePerVolume = (double)orderDictionary[nameof(GradeBPricePerVolume)];
            _gradeCPricePerVolume = (double)orderDictionary[nameof(GradeCPricePerVolume)];
            _grade = App.MainViewModel.SettingsViewModel.ApplicationSettings.GetGradeFromString((string)orderDictionary[nameof(Grade)]);
            _completedTime = ((orderDictionary[nameof(CompletedTime)] == null) || ((string)orderDictionary[nameof(CompletedTime)] == string.Empty)) ? null : DateTimeOffset.ParseExact((string)orderDictionary[nameof(CompletedTime)], "MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
        }

        public DocumentReference DocumentReference
        {
            get => _documentReference;

            set => _documentReference = value;
        }

        public bool IsUpdatingIsComplete
        {
            get => _isUpdatingIsComplete;

            set => SetProperty(ref _isUpdatingIsComplete, value);
        }

        public double GradeAPricePerVolume
        {
            get => _gradeAPricePerVolume;

            set => _gradeAPricePerVolume = value;
        }

        public double GradeBPricePerVolume
        {
            get => _gradeBPricePerVolume;

            set => _gradeBPricePerVolume = value;
        }

        public double GradeCPricePerVolume
        {
            get => _gradeCPricePerVolume;

            set => _gradeCPricePerVolume = value;
        }

        public System.DateTimeOffset? CompletedTime
        {
            get => _completedTime;

            set => SetProperty(ref _completedTime, value);
        }

        public bool IsComplete
        {
            get => _isComplete;

            set
            {
                SetProperty(ref _isComplete, value);
                RaisePropertyChanged(nameof(IsCompleteString));
            }
        }

        public string IsCompleteString
        {
            get => IsComplete ? "\uE3AE" : "\uE59F";
        }

        public Grade Grade
        {
            get => _grade;

            set
            {
                if (value == null)
                    return;

                SetProperty(ref _grade, value);

                autoUpdatePrice();
            }
        }

        public string Id
        {
            get => _id;

            private set => _id = value;
        }

        public string Account
        {
            get => _account;

            set
            {
                SetProperty(ref _account, value);

                UpdateCanAdd();
            }
        }

        public double Volume
        {
            get => _volume;

            set
            {
                canAutoUpdateVolume = false;

                SetProperty(ref _volume, value);

                if (App.MainViewModel.SettingsViewModel.ApplicationSettings.CanUseCurrentPricePerVolume)
                    autoUpdatePrice();

                canAutoUpdateVolume = true;

                UpdateCanAdd();
            }
        }

        public double Price
        {
            get => _price;

            set
            {
                canAutoUpdatePrice = false;

                SetProperty(ref _price, value);

                if(App.MainViewModel.SettingsViewModel.ApplicationSettings.CanUseCurrentPricePerVolume)
                    autoUpdateVolume();

                autoUpdateNetReceived();

                if (!double.IsNaN(NetReceived))
                    autoUpdateDiscount();

                canAutoUpdatePrice = true;

                UpdateCanAdd();
            }
        }

        public double Discount
        {
            get => _discount;

            set
            {
                if(value > Price)
                {
                    RaisePropertyChanged(nameof(Discount));
                    return;
                }

                canAutoUpdateDiscount = false;

                SetProperty(ref _discount, value);

                if (!double.IsNaN(Price))
                    autoUpdateNetReceived();

                canAutoUpdateDiscount = true;
            }
        }

        public double NetReceived
        {
            get => _netReceived;

            set
            {
                if(value > Price)
                {
                    RaisePropertyChanged(nameof(NetReceived));
                    return;
                }

                canAutoUpdateNetReceived = false;

                SetProperty(ref _netReceived, value);

                if (!double.IsNaN(Price))
                    autoUpdateDiscount();

                canAutoUpdateNetReceived = true;

                UpdateCanAdd();
            }
        }

        public string Details
        {
            get => _details;

            set
            {
                SetProperty(ref _details, value);
            }
        }

        public System.DateTime Time
        {
            get => _time;

            set
            {
                SetProperty(ref _time, value);
            }
        }

        public DateTimeOffset? ClosingTime
        {
            get => _closingTime;

            set
            {
                if (value is null)
                {
                    RaisePropertyChanged(nameof(ClosingTime));
                    return;
                }    

                if (((DateTimeOffset)value).Date < Time)
                {
                    RaisePropertyChanged(nameof(ClosingTime));
                    return;
                }

                SetProperty(ref _closingTime, value);

                UpdateCanAdd();
            }
        }

        public bool CanAdd
        {
            get => _canAdd;

            set
            {
                SetProperty(ref _canAdd, value);
            }
        }

        public DateTimeOffset CurrentDateTime
        {
            get => new DateTimeOffset(Time);
        }

        private bool canAutoUpdateVolume = true;
        private bool canAutoUpdatePrice = true;
        private bool canAutoUpdateDiscount = true;
        private bool canAutoUpdateNetReceived = true;

        private void autoUpdateVolume()
        {
            if (DocumentReference != null)
                return;

            if (canAutoUpdateVolume)
            {
                canAutoUpdateVolume = false;

                var result = double.IsNaN(Price) ? double.NaN : Price / Grade.Price;

                Volume = result;

                canAutoUpdateVolume = true;
            }
        }

        private void autoUpdatePrice()
        {
            if (DocumentReference != null)
                return;

            if (canAutoUpdatePrice)
            {
                canAutoUpdatePrice = false;

                var result = double.IsNaN(Volume) ? double.NaN : Volume * Grade.Price;

                Price = result;

                canAutoUpdatePrice = true;
            }
        }

        private void autoUpdateDiscount()
        {
            if (DocumentReference != null)
                return;

            if (canAutoUpdateDiscount)
            {
                canAutoUpdateDiscount = false;

                var result = double.IsNaN(Price) ? double.NaN : (double.IsNaN(NetReceived) ? double.NaN : Price - NetReceived);

                Discount = result;

                canAutoUpdateDiscount = true;
            }
        }

        private void autoUpdateNetReceived()
        {
            if (DocumentReference != null)
                return;

            if (canAutoUpdateNetReceived)
            {
                canAutoUpdateNetReceived = false;

                var result = double.IsNaN(Discount) ? Price - 0 : (double.IsNaN(Price) ? double.NaN : Price - Discount);

                NetReceived = result;

                canAutoUpdateNetReceived = true;
            }
        }

        public void UpdateCanAdd()
        {
            CanAdd =
                !string.IsNullOrEmpty(Account) &&
                (ClosingTime != null) &&
                !double.IsNaN(Volume) &&
                !double.IsNaN(Price) &&
                !double.IsNaN(NetReceived);
        }

        public Task UpdateAccount(string newAccount)
        {
            if (newAccount == this.Account)
                return null;

            return DocumentReference.UpdateAsync(nameof(Account), newAccount);
        }

        public Task UpdateGrade(Grade newGrade)
        {
            if (newGrade == this.Grade)
                return null;

            if (newGrade == null)
                return null;

            List<Task> updatingTasks = new();

            updatingTasks.Add
            (
                DocumentReference.UpdateAsync
                (
                    new Dictionary<string, object>
                    {
                        { nameof(Grade), newGrade.Quality.ToString() },
                        {
                            nameof(Price),
                            (
                                newGrade.Quality == Qualities.A ?
                                (GradeAPricePerVolume * Volume) :
                                (newGrade.Quality == Qualities.B ?
                                (GradeBPricePerVolume * Volume) :
                                (GradeCPricePerVolume * Volume))
                            )
                        },                        
                        {
                            nameof(NetReceived),
                            (
                                (
                                    newGrade.Quality == Qualities.A ?
                                    (GradeAPricePerVolume * Volume) :
                                    (newGrade.Quality == Qualities.B ?
                                    (GradeBPricePerVolume * Volume) :
                                    (GradeCPricePerVolume * Volume))
                                )
                                - this.Discount
                            )
                        }
                    }
                )
            );

            UpdateNetReceivedTotals(((
                                    newGrade.Quality == Qualities.A ?
                                    (GradeAPricePerVolume * Volume) :
                                    (newGrade.Quality == Qualities.B ?
                                    (GradeBPricePerVolume * Volume) :
                                    (GradeCPricePerVolume * Volume))
                                )
                                - this.Discount), updatingTasks);

            return MainViewModel.UpdateTasks(updatingTasks);
        }

        public Task UpdateVolume(double newVolume)
        {
            if (newVolume == this.Volume)
                return null;

            if (double.IsNaN(newVolume))
                return null;

            if (newVolume <= 0)
                return null;

            List<Task> updatingTasks = new();

            UpdateVolumeTotals(newVolume, updatingTasks);

            updatingTasks.Add
            (
                DocumentReference.UpdateAsync
                (
                    new Dictionary<string, object>
                    {
                        { nameof(Volume), newVolume },
                        {
                            nameof(Price),
                            (
                                Grade.Quality == Qualities.A ?
                                (GradeAPricePerVolume * newVolume) :
                                (Grade.Quality == Qualities.B ?
                                (GradeBPricePerVolume * newVolume) :
                                (GradeCPricePerVolume * newVolume))
                            )
                        },
                        {
                            nameof(NetReceived),
                            ((
                                Grade.Quality == Qualities.A ?
                                (GradeAPricePerVolume * newVolume) :
                                (Grade.Quality == Qualities.B ?
                                (GradeBPricePerVolume * newVolume) :
                                (GradeCPricePerVolume * newVolume))
                            ) - this.Discount)
                        }
                    }
                )
            );

            UpdateNetReceivedTotals(((
                                    Grade.Quality == Qualities.A ?
                                    (GradeAPricePerVolume * newVolume) :
                                    (Grade.Quality == Qualities.B ?
                                    (GradeBPricePerVolume * newVolume) :
                                    (GradeCPricePerVolume * newVolume))
                                )
                                - this.Discount), updatingTasks);
            return MainViewModel.UpdateTasks(updatingTasks);
        }

        private void UpdateVolumeTotals(double newVolume, List<Task> updatingTasks)
        {
            var totalsDocumentReferences = App.MainViewModel.FirestoreViewModel.GetTotalsDocumentReferences(this.Time);

            foreach (var totalsDocumentReference in totalsDocumentReferences)
            {
                Dictionary<string, object> updateDictionary = new()
                {
                    { nameof(TimeRange.TotalVolumeInOrders), FieldValue.Increment((newVolume - this.Volume)) },
                    { $"{ApplicationConstants.TOTAL_VOLUME_OF_PEBBLES_KEY}{Grade.Quality}{nameof(OrdersViewModel.Orders)}", FieldValue.Increment((newVolume - this.Volume)) },
                };

                if (this.IsComplete)
                {
                    updateDictionary.Add(nameof(TimeRange.TotalVolumeInIncompleteOrders), FieldValue.Increment((newVolume - this.Volume)));
                    updateDictionary.Add($"{ApplicationConstants.TOTAL_VOLUME_OF_PEBBLES_KEY}{Grade.Quality}{ApplicationConstants.IN_INCOMPLETE_ORDERS_KEY}", FieldValue.Increment((newVolume - this.Volume)));
                }
                else
                {
                    updateDictionary.Add(nameof(TimeRange.TotalVolumeInCompleteOrders), FieldValue.Increment((newVolume - this.Volume)));
                    updateDictionary.Add($"{ApplicationConstants.TOTAL_VOLUME_OF_PEBBLES_KEY}{Grade.Quality}{ApplicationConstants.IN_COMPLETE_ORDERS_KEY}", FieldValue.Increment((newVolume - this.Volume)));
                }

                updatingTasks.Add(totalsDocumentReference.UpdateAsync(updateDictionary));
            }
        }

        public Task UpdateDiscount(double newDiscount)
        {
            if (newDiscount == this.Discount)
                return null;

            if (newDiscount < 0)
                return null;

            List<Task> updatingTasks = new();

            UpdateNetReceivedTotals((Price - newDiscount), updatingTasks);

            updatingTasks.Add(DocumentReference.UpdateAsync(new Dictionary<string, object> { { nameof(Discount), newDiscount }, { nameof(NetReceived), (Price - newDiscount) } }));

            return MainViewModel.UpdateTasks(updatingTasks);
        }

        private void UpdateNetReceivedTotals(double newNetReceived, List<Task> updatingTasks)
        {
            var totalsDocumentReferences = App.MainViewModel.FirestoreViewModel.GetTotalsDocumentReferences(this.Time);

            foreach (var totalsDocumentReference in totalsDocumentReferences)
            {
                Dictionary<string, object> updateDictionary = new()
                {
                    { nameof(TimeRange.TotalAmountInOrders), FieldValue.Increment((newNetReceived - this.NetReceived)) },
                    { $"{ApplicationConstants.TOTAL_AMOUNT_OF_PEBBLES_KEY}{Grade.Quality}{nameof(OrdersViewModel.Orders)}", FieldValue.Increment((newNetReceived - this.NetReceived)) },
                };

                if (this.IsComplete)
                {
                    updateDictionary.Add(nameof(TimeRange.TotalAmountInIncompleteOrders), FieldValue.Increment((newNetReceived - this.NetReceived)));
                    updateDictionary.Add($"{ApplicationConstants.TOTAL_AMOUNT_OF_PEBBLES_KEY}{Grade.Quality}{ApplicationConstants.IN_INCOMPLETE_ORDERS_KEY}", FieldValue.Increment((newNetReceived - this.NetReceived)));
                }
                else
                {
                    updateDictionary.Add(nameof(TimeRange.TotalVolumeInCompleteOrders), FieldValue.Increment((newNetReceived - this.NetReceived)));
                    updateDictionary.Add($"{ApplicationConstants.TOTAL_AMOUNT_OF_PEBBLES_KEY}{Grade.Quality}{ApplicationConstants.IN_COMPLETE_ORDERS_KEY}", FieldValue.Increment((newNetReceived - this.NetReceived)));
                }

                updatingTasks.Add(totalsDocumentReference.UpdateAsync(updateDictionary));
            }
        }

        public Task UpdateDetails(string newDetails)
        {
            if (newDetails == this.Details)
                return null;

            return DocumentReference.UpdateAsync(new Dictionary<string, object> { { nameof(Details), newDetails } });
        }

        public Task UpdateClosingDate(DateTimeOffset? newClosingDate)
        {
            if (newClosingDate == this.ClosingTime)
                return null;

            if (newClosingDate == null)
                return null;

            return DocumentReference.UpdateAsync(nameof(ClosingTime), ((DateTimeOffset)newClosingDate).DateTime.ToString("MM/dd/yyyy hh:mm:ss tt"));
        }

        public Task Close()
        {
            var totalsDocumentReferences = App.MainViewModel.FirestoreViewModel.GetTotalsDocumentReferences(this.Time);

            List<Task> updatingTasks = new();

            var completedTime = System.DateTime.Now;

            updatingTasks.Add(DocumentReference.UpdateAsync(nameof(CompletedTime), completedTime.ToString("MM/dd/yyyy hh:mm:ss tt")));

            foreach (var totalsDocumentReference in totalsDocumentReferences)
            {
                updatingTasks.Add
                (
                    totalsDocumentReference.UpdateAsync
                    (
                        new Dictionary<string, object>
                        {
                            { nameof(TimeRange.TotalNumberOfIncompleteOrders), FieldValue.Increment(-1) },
                            { nameof(TimeRange.TotalAmountInIncompleteOrders), FieldValue.Increment(-NetReceived) },
                            { nameof(TimeRange.TotalVolumeInIncompleteOrders), FieldValue.Increment(-Volume) },

                            { $"{ApplicationConstants.TOTAL_NUMBER_OF_PEBBLES_KEY}{Grade.Quality}{ApplicationConstants.IN_INCOMPLETE_ORDERS_KEY}", FieldValue.Increment(-1) },
                            { $"{ApplicationConstants.TOTAL_VOLUME_OF_PEBBLES_KEY}{Grade.Quality}{ApplicationConstants.IN_INCOMPLETE_ORDERS_KEY}", FieldValue.Increment(-Volume) },
                            { $"{ApplicationConstants.TOTAL_AMOUNT_OF_PEBBLES_KEY}{Grade.Quality}{ApplicationConstants.IN_INCOMPLETE_ORDERS_KEY}", FieldValue.Increment(-NetReceived) },

                            { nameof(TimeRange.TotalNumberOfCompleteOrders), FieldValue.Increment(1) },
                            { nameof(TimeRange.TotalAmountInCompleteOrders), FieldValue.Increment(NetReceived) },
                            { nameof(TimeRange.TotalVolumeInCompleteOrders), FieldValue.Increment(Volume) },

                            { $"{ApplicationConstants.TOTAL_NUMBER_OF_PEBBLES_KEY}{Grade.Quality}{ApplicationConstants.IN_COMPLETE_ORDERS_KEY}", FieldValue.Increment(1) },
                            { $"{ApplicationConstants.TOTAL_VOLUME_OF_PEBBLES_KEY}{Grade.Quality}{ApplicationConstants.IN_COMPLETE_ORDERS_KEY}", FieldValue.Increment(Volume) },
                            { $"{ApplicationConstants.TOTAL_AMOUNT_OF_PEBBLES_KEY}{Grade.Quality}{ApplicationConstants.IN_COMPLETE_ORDERS_KEY}", FieldValue.Increment(NetReceived) }
                        }
                    )
                );
            }

            updatingTasks.Add
            (
                App.MainViewModel.FirestoreViewModel.AddRevenueToDatabase
                (
                    new Revenue(this, completedTime)
                )
            );

            updatingTasks.Add(this.DocumentReference.UpdateAsync(nameof(IsComplete), true));

            return MainViewModel.UpdateTasks(updatingTasks);
        }
    }
}
