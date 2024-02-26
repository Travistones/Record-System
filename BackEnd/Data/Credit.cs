using Google.Cloud.Firestore;
using Google.Type;
using Microsoft.UI.Xaml.Input;
using Record_System.BackEnd.Constants;
using Record_System.BackEnd.Database;
using Record_System.BackEnd.ObjectPropertyChanged;
using Record_System.BackEnd.ViewModels.Credits;
using Record_System.BackEnd.ViewModels.Main;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Record_System.BackEnd.Data
{
    public class Credit : BindableBase
    {
        private string _id;
        private string _account;
        private double _volume = double.NaN;
        private double _price = double.NaN;
        private double _netPaid = double.NaN;
        private double _netUnpaid = double.NaN;
        private DateTimeOffset? _closingTime;
        private string _details;
        private System.DateTime _time;
        private DateTimeOffset? _completedTime;
        private bool _canAdd;
        private Grade _grade = App.MainViewModel.SettingsViewModel.ApplicationSettings.GradeB;
        private bool _isComplete;
        private DocumentReference _documentReference;
        private double _gradeAPricePerVolume;
        private double _gradeBPricePerVolume;
        private double _gradeCPricePerVolume;
        private bool _isUpdatingIsComplete;

        public Credit(Dictionary<string, object> creditDictionary, string id)
        {
            Id = id;
            _account = (string)creditDictionary[nameof(Account)];
            _volume = (double)creditDictionary[nameof(Volume)];
            _price = (double)creditDictionary[nameof(Price)];
            _netPaid = (double)creditDictionary[nameof(NetPaid)];
            _netUnpaid = (double)creditDictionary[nameof(NetUnpaid)];
            _closingTime = DateTimeOffset.ParseExact((string)creditDictionary[nameof(ClosingTime)], "MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
            _details = (string)creditDictionary[nameof(Details)];
            _time = System.DateTime.ParseExact((string)creditDictionary[nameof(Time)], "MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
            _isComplete = (bool)creditDictionary[nameof(IsComplete)];
            _gradeAPricePerVolume = (double)creditDictionary[nameof(GradeAPricePerVolume)];
            _gradeBPricePerVolume = (double)creditDictionary[nameof(GradeBPricePerVolume)];
            _gradeCPricePerVolume = (double)creditDictionary[nameof(GradeCPricePerVolume)];
            _grade = App.MainViewModel.SettingsViewModel.ApplicationSettings.GetGradeFromString((string)creditDictionary[nameof(Grade)]);
            _completedTime = ((creditDictionary[nameof(CompletedTime)] == null) || ((string)creditDictionary[nameof(CompletedTime)] == string.Empty)) ? null : DateTimeOffset.ParseExact((string)creditDictionary[nameof(CompletedTime)], "MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
        }

        public Credit() { }

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

        public DateTimeOffset? CompletedTime
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

        public List<Grade> Grades { get => App.MainViewModel.SettingsViewModel.ApplicationSettings.Grades; }

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

                updateCanAdd();
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

                updateCanAdd();
            }
        }

        public double Price
        {
            get => _price;

            set
            {
                canAutoUpdatePrice = false;

                SetProperty(ref _price, value);

                if (App.MainViewModel.SettingsViewModel.ApplicationSettings.CanUseCurrentPricePerVolume)
                    autoUpdateVolume();

                autoUpdateNetUnpaid();

                canAutoUpdatePrice = true;

                updateCanAdd();
            }
        }

        public double NetPaid
        {
            get => _netPaid;

            set
            { 
                SetProperty(ref _netPaid, value);

                if (!double.IsNaN(Price))
                    autoUpdateNetUnpaid();
            }
        }

        public double NetUnpaid
        {
            get => _netUnpaid;

            set
            {
                canAutoUpdateNetUnpaid = false;

                SetProperty(ref _netUnpaid, value);

                canAutoUpdateNetUnpaid = true;

                updateCanAdd();
            }
        }

        public DateTimeOffset? ClosingTime
        {
            get => _closingTime;

            set
            {
                SetProperty(ref _closingTime, value);

                updateCanAdd();
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

            set => SetProperty(ref _time, value);
        }

        public bool CanAdd
        {
            get => _canAdd;

            set => SetProperty(ref _canAdd, value);
        }

        public DateTimeOffset CurrentDateTime
        {
            get => new DateTimeOffset(Time);
        }

        private bool canAutoUpdateVolume = true;
        private bool canAutoUpdatePrice = true;
        private bool canAutoUpdateNetUnpaid = true;

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

        private void autoUpdateNetUnpaid()
        {
            if (DocumentReference != null)
                return;

            if (canAutoUpdateNetUnpaid)
            {
                canAutoUpdateNetUnpaid = false;

                NetUnpaid = double.IsNaN(Price) ? double.NaN : (double.IsNaN(NetPaid) ? Price : Price - NetPaid);

                canAutoUpdateNetUnpaid = true;
            }
        }

        private void updateCanAdd()
        {
            CanAdd =
                !string.IsNullOrEmpty(Account) &&
                !double.IsNaN(Volume) &&
                !double.IsNaN(Price) &&
                !double.IsNaN(NetUnpaid) &&
                (ClosingTime != null);
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
                            nameof(NetUnpaid),
                            (
                                (
                                    newGrade.Quality == Qualities.A ?
                                    (GradeAPricePerVolume * Volume) :
                                    (newGrade.Quality == Qualities.B ?
                                    (GradeBPricePerVolume * Volume) :
                                    (GradeCPricePerVolume * Volume))
                                ) - this.NetPaid
                            )
                        }
                    }
                )
            );

            UpdateNetUnpaidTotals((
                                (
                                    newGrade.Quality == Qualities.A ?
                                    (GradeAPricePerVolume * Volume) :
                                    (newGrade.Quality == Qualities.B ?
                                    (GradeBPricePerVolume * Volume) :
                                    (GradeCPricePerVolume * Volume))
                                ) - this.NetPaid
                            ), updatingTasks);

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
                            nameof(NetUnpaid),
                            (
                                (
                                    Grade.Quality == Qualities.A ?
                                    (GradeAPricePerVolume * newVolume) :
                                    (Grade.Quality == Qualities.B ?
                                    (GradeBPricePerVolume * newVolume) :
                                    (GradeCPricePerVolume * newVolume))
                                )
                                - this.NetPaid
                            )
                        }
                    }
                )
            );
            UpdateNetUnpaidTotals((
                                (
                                    Grade.Quality == Qualities.A ?
                                    (GradeAPricePerVolume * newVolume) :
                                    (Grade.Quality == Qualities.B ?
                                    (GradeBPricePerVolume * newVolume) :
                                    (GradeCPricePerVolume * newVolume))
                                ) - this.NetPaid
                            ), updatingTasks);

            return MainViewModel.UpdateTasks(updatingTasks);
        }

        private void UpdateVolumeTotals(double newVolume, List<Task> updatingTasks)
        {
            var totalsDocumentReferences = App.MainViewModel.FirestoreViewModel.GetTotalsDocumentReferences(this.Time);

            foreach (var totalsDocumentReference in totalsDocumentReferences)
            {
                Dictionary<string, object> updateDictionary = new()
                {
                    { nameof(TimeRange.TotalVolumeInCredits), FieldValue.Increment((newVolume - this.Volume)) },
                    { $"{ApplicationConstants.TOTAL_VOLUME_OF_PEBBLES_KEY}{Grade.Quality}{nameof(CreditsViewModel.Credits)}", FieldValue.Increment((newVolume - this.Volume)) },
                };

                if (this.IsComplete)
                {
                    updateDictionary.Add(nameof(TimeRange.TotalVolumeInIncompleteCredits), FieldValue.Increment((newVolume - this.Volume)));
                    updateDictionary.Add($"{ApplicationConstants.TOTAL_VOLUME_OF_PEBBLES_KEY}{Grade.Quality}{ApplicationConstants.IN_INCOMPLETE_CREDITS_KEY}", FieldValue.Increment((newVolume - this.Volume)));
                }
                else
                {
                    updateDictionary.Add(nameof(TimeRange.TotalVolumeInCompleteCredits), FieldValue.Increment((newVolume - this.Volume)));
                    updateDictionary.Add($"{ApplicationConstants.TOTAL_VOLUME_OF_PEBBLES_KEY}{Grade.Quality}{ApplicationConstants.IN_COMPLETE_CREDITS_KEY}", FieldValue.Increment((newVolume - this.Volume)));
                }

                updatingTasks.Add(totalsDocumentReference.UpdateAsync(updateDictionary));
            }
        }

        public Task UpdateNetPaid(double newNetPaid)
        {
            if (newNetPaid > this.Price)
                return null;

            if (newNetPaid == this.NetPaid)
                return null;

            if (newNetPaid < 0)
                return null;

            List<Task> updatingTasks = new();

            updatingTasks.Add
            (
                DocumentReference.UpdateAsync
                (
                    new Dictionary<string, object>
                    {
                        { nameof(NetPaid), newNetPaid },
                        { nameof(NetUnpaid), (Price - newNetPaid) }
                    }
                )
            );

            UpdateNetUnpaidTotals((Price - newNetPaid), updatingTasks);

            return MainViewModel.UpdateTasks(updatingTasks);
        }

        private void UpdateNetUnpaidTotals(double newNetUnpaid, List<Task> updatingTasks)
        {
            var totalsDocumentReferences = App.MainViewModel.FirestoreViewModel.GetTotalsDocumentReferences(this.Time);

            foreach (var totalsDocumentReference in totalsDocumentReferences)
            {
                Dictionary<string, object> updateDictionary = new()
                {
                    { nameof(TimeRange.TotalAmountInCredits), FieldValue.Increment((newNetUnpaid - this.NetUnpaid)) },
                    { $"{ApplicationConstants.TOTAL_AMOUNT_OF_PEBBLES_KEY}{Grade.Quality}{nameof(CreditsViewModel.Credits)}", FieldValue.Increment((newNetUnpaid - this.NetUnpaid)) },
                };

                if (this.IsComplete)
                {
                    updateDictionary.Add(nameof(TimeRange.TotalAmountInIncompleteCredits), FieldValue.Increment((newNetUnpaid - this.NetUnpaid)));
                    updateDictionary.Add($"{ApplicationConstants.TOTAL_AMOUNT_OF_PEBBLES_KEY}{Grade.Quality}{ApplicationConstants.IN_INCOMPLETE_CREDITS_KEY}", FieldValue.Increment((newNetUnpaid - this.NetUnpaid)));
                }
                else
                {
                    updateDictionary.Add(nameof(TimeRange.TotalVolumeInCompleteCredits), FieldValue.Increment((newNetUnpaid - this.NetUnpaid)));
                    updateDictionary.Add($"{ApplicationConstants.TOTAL_AMOUNT_OF_PEBBLES_KEY}{Grade.Quality}{ApplicationConstants.IN_COMPLETE_CREDITS_KEY}", FieldValue.Increment((newNetUnpaid - this.NetUnpaid)));
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

            bool isUnpaid = this.NetUnpaid > 0;

            foreach (var totalsDocumentReference in totalsDocumentReferences)
            {
                updatingTasks.Add
                (
                    totalsDocumentReference.UpdateAsync
                    (
                        new Dictionary<string, object>
                        {
                            { nameof(TimeRange.TotalNumberOfIncompleteCredits), FieldValue.Increment(-1) },
                            { nameof(TimeRange.TotalAmountInIncompleteCredits), FieldValue.Increment(isUnpaid ? -NetUnpaid : -NetPaid) },
                            { nameof(TimeRange.TotalVolumeInIncompleteCredits), FieldValue.Increment(-Volume) },

                            { $"{ApplicationConstants.TOTAL_NUMBER_OF_PEBBLES_KEY}{Grade.Quality}{ApplicationConstants.IN_INCOMPLETE_CREDITS_KEY}", FieldValue.Increment(-1) },
                            { $"{ApplicationConstants.TOTAL_VOLUME_OF_PEBBLES_KEY}{Grade.Quality}{ApplicationConstants.IN_INCOMPLETE_CREDITS_KEY}", FieldValue.Increment(-Volume) },
                            { $"{ApplicationConstants.TOTAL_AMOUNT_OF_PEBBLES_KEY}{Grade.Quality}{ApplicationConstants.IN_INCOMPLETE_CREDITS_KEY}", FieldValue.Increment(isUnpaid ? -NetUnpaid : -NetPaid) },

                            { nameof(TimeRange.TotalNumberOfCompleteCredits), FieldValue.Increment(1) },
                            { nameof(TimeRange.TotalAmountInCompleteCredits), FieldValue.Increment(isUnpaid ? NetUnpaid : NetPaid) },
                            { nameof(TimeRange.TotalVolumeInCompleteCredits), FieldValue.Increment(Volume) },

                            { $"{ApplicationConstants.TOTAL_NUMBER_OF_PEBBLES_KEY}{Grade.Quality}{ApplicationConstants.IN_COMPLETE_CREDITS_KEY}", FieldValue.Increment(1) },
                            { $"{ApplicationConstants.TOTAL_VOLUME_OF_PEBBLES_KEY}{Grade.Quality}{ApplicationConstants.IN_COMPLETE_CREDITS_KEY}", FieldValue.Increment(Volume) },
                            { $"{ApplicationConstants.TOTAL_AMOUNT_OF_PEBBLES_KEY}{Grade.Quality}{ApplicationConstants.IN_COMPLETE_CREDITS_KEY}", FieldValue.Increment(isUnpaid ? NetUnpaid : NetPaid) }
                        }
                    )
                );
            }

            if ((!double.IsNaN(this.NetPaid) && this.NetPaid > 0))
            {
                updatingTasks.Add
                (
                    App.MainViewModel.FirestoreViewModel.AddRevenueToDatabase
                    (
                        new Revenue(this, completedTime)
                    )
                );
            }

            if (isUnpaid)
            {
                updatingTasks.Add
                (
                    App.MainViewModel.FirestoreViewModel.AddExpenseToDatabase
                    (
                        new Expense(this, completedTime),
                        completedTime.ToString("yyyy-MM-dd-HH-mm-ss")
                    )
                );
            }

            updatingTasks.Add(this.DocumentReference.UpdateAsync(nameof(IsComplete), true));

            return MainViewModel.UpdateTasks(updatingTasks);
        }
    }
}
