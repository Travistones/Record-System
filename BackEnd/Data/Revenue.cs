using CommunityToolkit.WinUI.UI.Controls.TextToolbarSymbols;
using Google.Cloud.Firestore;
using Record_System.BackEnd.Constants;
using Record_System.BackEnd.Database;
using Record_System.BackEnd.ObjectPropertyChanged;
using Record_System.BackEnd.ViewModels.Main;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Windows.Storage;

namespace Record_System.BackEnd.Data
{
    public class Revenue : BindableBase
    {
        private DateTime _time;
        private string _id;
        private string _account;
        private double _volume = double.NaN;
        private string _details;
        private double _price = double.NaN;
        private double _discount = double.NaN;
        private double _netReceived = double.NaN;
        private bool _canAdd;
        private bool _isSale = true;
        private Grade _grade = App.MainViewModel.SettingsViewModel.ApplicationSettings.GradeB;
        private DocumentReference _documentReference;
        private double _gradeAPricePerVolume;
        private double _gradeBPricePerVolume;
        private double _gradeCPricePerVolume;
        private bool _canEdit = false;

        public Revenue() { }

        public Revenue(Dictionary<string, object> revenueDictionary, string id)
        {
            Id = id;
            _time = DateTime.ParseExact((string)revenueDictionary[nameof(Time)], "MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
            _account = (string)revenueDictionary[nameof(Account)];
            _volume = (double)revenueDictionary[nameof(Volume)];
            _details = (string)revenueDictionary[nameof(Details)];
            _price = (double)revenueDictionary[nameof(Price)];
            _discount = (double)revenueDictionary[nameof(Discount)];
            _netReceived = (double)revenueDictionary[nameof(NetReceived)];
            _isSale = (bool)revenueDictionary[nameof(IsSale)];
            _gradeAPricePerVolume = (double)revenueDictionary[nameof(GradeAPricePerVolume)];
            _gradeBPricePerVolume = (double)revenueDictionary[nameof(GradeBPricePerVolume)];
            _gradeCPricePerVolume = (double)revenueDictionary[nameof(GradeCPricePerVolume)];
            _grade = App.MainViewModel.SettingsViewModel.ApplicationSettings.GetGradeFromString((string)revenueDictionary[nameof(Grade)]);
            _canEdit = (bool)revenueDictionary[nameof(CanEdit)];
        }
        
        public Revenue(Credit credit, DateTime completedTime)
        {
            bool isUnpaid = credit.NetUnpaid > 0;

            _time = completedTime;
            _account = credit.Account;
            _volume = isUnpaid ? double.NaN : credit.Volume;
            _details = credit.Details;
            _price = isUnpaid ? double.NaN : credit.Price   ;
            _discount = 0;
            _netReceived = credit.NetPaid;
            _isSale = !isUnpaid;
            _gradeAPricePerVolume = isUnpaid ? double.NaN : credit.GradeAPricePerVolume;
            _gradeBPricePerVolume = isUnpaid ? double.NaN : credit.GradeBPricePerVolume;
            _gradeCPricePerVolume = isUnpaid ? double.NaN : credit.GradeCPricePerVolume;
            _grade = isUnpaid ? null : credit.Grade;
            _canEdit = false;
        }

        public Revenue(Order order, DateTime completedDateTime)
        {
            _time = completedDateTime;
            _account = order.Account;
            _volume = order.Volume;
            _details = order.Details;
            _price = order.Price;
            _discount = order.Discount;
            _netReceived = order.NetReceived;
            _isSale = true;
            _gradeAPricePerVolume = order.GradeAPricePerVolume;
            _gradeBPricePerVolume = order.GradeBPricePerVolume;
            _gradeCPricePerVolume = order.GradeCPricePerVolume;
            _grade = order.Grade;
            _canEdit = false;
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

        public DocumentReference DocumentReference
        {
            get => _documentReference;

            set => _documentReference = value;
        }

        public string Id
        {
            get => _id;

            private set => _id = value;
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

        public bool IsSale
        {
            get => _isSale;

            set
            {
                if (value != _isSale)
                    this.Clear();

                SetProperty(ref _isSale, value);
            }
        }

        public bool CanEdit
        {
            get => _canEdit;

            set => SetProperty(ref _canEdit, value);
        }

        public DateTime Time
        {
            get => _time;

            set
            {
                SetProperty(ref _time, value);
            }
        }

        public string Account
        {
            get => _account;

            set
            {
                SetProperty(ref _account, value);
            }
        }

        public double Volume
        {
            get => _volume;

            set
            {
                SetProperty(ref _volume, value);
                autoUpdatePrice();
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

        public double Price
        {
            get => _price;

            set
            {
                if (value < 0) return;
                SetProperty(ref _price, value);
                autoUpdateVolume();
                autoUpdateNetReceived();
                autoUpdateDiscount();
            }
        }

        public double Discount
        {
            get => _discount;

            set
            {
                if (value < 0)
                {
                    RaisePropertyChanged(nameof(Discount));
                    return;
                }

                SetProperty(ref _discount, value);
                autoUpdateNetReceived();
            }
        }

        public double NetReceived
        {
            get => _netReceived;

            set
            {
                if (DocumentReference == null)
                {
                    if (value < 0)
                    {
                        RaisePropertyChanged(nameof(NetReceived));
                        return;
                    }

                    if (IsSale && (value > (double.IsNaN(Price) ? 0.0 : Price)))
                    {
                        Price = value;
                        return;
                    }
                }

                SetProperty(ref _netReceived, value);

                if (IsSale)
                {
                    autoUpdateDiscount();
                }

                updateCanAdd();
            }
        }

        public bool CanAdd
        {
            get => _canAdd;

            private set => SetProperty(ref _canAdd, value);
        }

        private void updateCanAdd()
        {
            CanAdd = IsSale ? !double.IsNaN(Volume) && !double.IsNaN(Price) && !double.IsNaN(NetReceived) : !double.IsNaN(NetReceived);
        }

        private bool isNetReceivedAutoUpdatePossible = true;
        private bool isDiscountAutoUpdatePossible = true;
        private bool isPriceAutoUpdatePossible = true;
        private bool isVolumeAutoUpdatePossible = true;

        private void autoUpdateNetReceived()
        {
            if (DocumentReference != null)
                return;

            if (isNetReceivedAutoUpdatePossible)
            {
                isNetReceivedAutoUpdatePossible = false;

                var result = Price - (!double.IsNaN(Discount) ? Discount : 0.0);

                NetReceived = (result < 0) ? double.NaN : result;

                isNetReceivedAutoUpdatePossible = true;
            }
        }

        private void autoUpdateDiscount()
        {
            if (DocumentReference != null)
                return;

            if (!isDiscountAutoUpdatePossible)
                return;

            isDiscountAutoUpdatePossible = false;

            if (!double.IsNaN(NetReceived))
            {
                var result = Price - NetReceived;

                Discount = (result < 0 || result == 0) ? double.NaN : result;
            }

            else Discount = double.NaN;

            isDiscountAutoUpdatePossible = true;
        }

        private void autoUpdatePrice()
        {
            if (DocumentReference != null)
                return;

            if (isPriceAutoUpdatePossible && App.MainViewModel.SettingsViewModel.ApplicationSettings.CanUseCurrentPricePerVolume)
            {

                isPriceAutoUpdatePossible = false;
                isVolumeAutoUpdatePossible = false;

                if (!double.IsNaN(Volume))
                {
                    var result = Volume * Grade.Price;

                    Price = (result < 0 || result == 0) ? double.NaN : result;

                }
                else Price = double.NaN;

                isPriceAutoUpdatePossible = true;
                isVolumeAutoUpdatePossible = true;
            }
        }

        private void autoUpdateVolume()
        {
            if (DocumentReference != null)
                return;

            if (isVolumeAutoUpdatePossible && App.MainViewModel.SettingsViewModel.ApplicationSettings.CanUseCurrentPricePerVolume)
            {

                isVolumeAutoUpdatePossible = false;
                isPriceAutoUpdatePossible = false;

                var result = Price / Grade.Price;

                Volume = (result < 0 || result == 0) ? double.NaN : result;

                isVolumeAutoUpdatePossible = true;
                isPriceAutoUpdatePossible = true;
            }
        }

        private void clear()
        {
            Account = string.Empty;
            Volume = double.NaN;
            Details = string.Empty;
            Price = double.NaN;
            Discount = double.NaN;
            NetReceived = double.NaN;
            CanAdd = false;
        }

        public void Clear() => this.clear();

        public Task UpdateAccount(string newAccount)
        {
            if (newAccount == this.Account)
                return null;

            return DocumentReference.UpdateAsync(nameof(Account), newAccount);
        }

        private void shiftGrade(Grade newGrade, List<Task> updatingTasks)
        {
            var totalsDocumentReferences = App.MainViewModel.FirestoreViewModel.GetTotalsDocumentReferences(this.Time);

            foreach (var totalsDocumentReference in totalsDocumentReferences)
            {
                updatingTasks.Add
                (
                    totalsDocumentReference.UpdateAsync
                    (
                        new Dictionary<string, object> 
                        {
                            { $"{ApplicationConstants.TOTAL_VOLUME_OF_GRADE}{Grade.Quality}{ApplicationConstants.PEBBLES_KEY}", FieldValue.Increment(-this.Volume)},
                            { $"{ApplicationConstants.TOTAL_AMOUNT_EARNED_FROM_GRADE}{Grade.Quality}{ApplicationConstants.PEBBLES_KEY}", FieldValue.Increment(-this.NetReceived)},
                            { $"{ApplicationConstants.TOTAL_VOLUME_OF_GRADE}{newGrade.Quality}{ApplicationConstants.PEBBLES_KEY}", FieldValue.Increment(this.Volume)},
                            { $"{ApplicationConstants.TOTAL_AMOUNT_EARNED_FROM_GRADE}{newGrade.Quality}{ApplicationConstants.PEBBLES_KEY}", FieldValue.Increment
                                    (
                                        (
                                                (newGrade.Quality == Qualities.A ?
                                                (GradeAPricePerVolume * Volume) :
                                                (newGrade.Quality == Qualities.B ?
                                                (GradeBPricePerVolume * Volume) :
                                                (GradeCPricePerVolume * Volume)
                                            )
                                        ) - this.Discount
                                    )
                                )
                            }
                        }
                    )
                );
                
            }
        }

        public Task UpdateGrade(Grade newGrade)
        {
            if (newGrade == this.Grade)
                return null;

            if (newGrade == null)
                return null;

            List<Task> updatingTasks = new();

            shiftGrade(newGrade, updatingTasks);

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
                            (
                                Grade.Quality == Qualities.A ?
                                (GradeAPricePerVolume * newVolume) :
                                (Grade.Quality == Qualities.B ?
                                (GradeBPricePerVolume * newVolume) :
                                (GradeCPricePerVolume * newVolume))
                            ) - this.Discount
                        }
                    }
                )
            );

            return MainViewModel.UpdateTasks(updatingTasks);
        }

        private void UpdateVolumeTotals(double newVolume, List<Task> updatingTasks)
        {
            if (this.IsSale)
            {
                var totalsDocumentReferences = App.MainViewModel.FirestoreViewModel.GetTotalsDocumentReferences(this.Time);

                foreach (var totalsDocumentReference in totalsDocumentReferences)
                {
                    updatingTasks.Add
                    (
                        totalsDocumentReference.UpdateAsync
                        (
                            new Dictionary<string, object>
                            {
                                { nameof(TimeRange.TotalVolumeOfPebbles), FieldValue.Increment((newVolume - this.Volume)) },
                                { $"{ApplicationConstants.TOTAL_VOLUME_OF_GRADE}{this.Grade.Quality}{ApplicationConstants.PEBBLES_KEY}", FieldValue.Increment((newVolume - this.Volume)) }
                            }
                        )
                    );
                }
            }
        }

        public Task UpdateDiscount(double newDiscount)
        {
            if (newDiscount == this.Discount)
                return null;

            if (newDiscount < 0)
                return null;

            if (newDiscount > this.Price)
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
                Dictionary<string, object> updateFields = new()
                {
                    { nameof(TimeRange.TotalRevenues), FieldValue.Increment((newNetReceived - this.NetReceived))},
                    { nameof(TimeRange.NetProfit), FieldValue.Increment((newNetReceived - this.NetReceived))}
                };

                if (this.IsSale)
                {
                    updateFields.Add(nameof(TimeRange.TotalSalesRevenues), FieldValue.Increment((newNetReceived - this.NetReceived)));
                    updateFields.Add(nameof(TimeRange.TotalAmountEarnedFromPebbles), FieldValue.Increment((newNetReceived - this.NetReceived)));
                }
                else
                {
                    updateFields.Add(nameof(TimeRange.TotalOtherRevenues), FieldValue.Increment((newNetReceived - this.NetReceived)));
                }

                updatingTasks.Add(totalsDocumentReference.UpdateAsync(updateFields));

            }
        }

        public Task UpdateDetails(string newDetails)
        {
            if (newDetails == this.Details)
                return null;

            return DocumentReference.UpdateAsync(nameof(Details), newDetails);
        }
    }
}
