using Google.Cloud.Firestore;
using Record_System.BackEnd.Database;
using Record_System.BackEnd.ObjectPropertyChanged;
using Record_System.BackEnd.ViewModels.Expenses;
using Record_System.BackEnd.ViewModels.Main;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography.Core;

namespace Record_System.BackEnd.Data
{
    public class Expense : BindableBase
    {
        private bool _canAdd = false;
        private string _id;
        private string _account;
        private string _productOrService = "Trucks";
        private double _quantity = double.NaN;
        private double _pricePerEach = double.NaN;
        private double _totalPrice = double.NaN;
        private double _netPaid = double.NaN;
        private double _netUnpaid = double.NaN;
        private string _details;
        private DateTime _time;
        private bool _isTrucksExpense = true;
        private DocumentReference _documentReference;
        private bool _canEdit = true;

        public Expense(Dictionary<string, object> expenseDictionary, string id)
        {
            Id = id;
            _account = (string)expenseDictionary[nameof(Account)];
            _productOrService = (string)expenseDictionary[nameof(ProductOrService)];
            _quantity = (double)expenseDictionary[nameof(Quantity)];
            _pricePerEach = (double)expenseDictionary[nameof(PricePerEach)];
            _totalPrice = (double)expenseDictionary[nameof(TotalPrice)];
            _netPaid = (double)expenseDictionary[nameof(NetPaid)];
            _netUnpaid = (double)expenseDictionary[nameof(NetUnpaid)];
            _details = (string)expenseDictionary[nameof(Details)];
            _time = DateTime.ParseExact((string)expenseDictionary[nameof(Time)], "MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
            _isTrucksExpense = (bool)expenseDictionary[nameof(IsTrucksExpense)];
            _canEdit = (bool)expenseDictionary[nameof(CanEdit)];
        }

        public Expense(Credit credit, DateTime completedTime)
        {
            _account = credit.Account;
            _productOrService = "Closed Incomplete Credit";
            _isTrucksExpense = false;
            _quantity = 1;
            _pricePerEach = credit.NetUnpaid;
            _totalPrice = credit.NetUnpaid;
            _netPaid = credit.NetUnpaid;
            _netUnpaid = double.NaN;
            _details = "From UnPaid Credit";
            _time = completedTime;
            _canEdit = false;
        }

        public Expense() { }

        public DocumentReference DocumentReference
        {
            get => _documentReference;

            set => _documentReference = value;
        }

        public bool CanAdd
        {
            get => _canAdd;

            set => SetProperty(ref _canAdd, value);
        }

        public string Id
        {
            get => _id;

            private set => _id = value;
        }

        public string Account
        {
            get => _account;

            set => SetProperty(ref _account, value);
        }

        public bool CanEdit
        {
            get => _canEdit;

            set => SetProperty(ref _canEdit, value);
        }

        public string ProductOrService
        {
            get => _productOrService;

            set
            {
                SetProperty(ref _productOrService, value);

                UpdateCanAdd();
            }
        }

        public double TotalPrice
        {
            get => _totalPrice;

            set
            {
                canAutoUpdateTotalPrice = false;

                SetProperty(ref _totalPrice, value);

                if (!double.IsNaN(PricePerEach))
                    AutoUpdateQuantity();

                AutoUpdateNetUnpaid();

                if(!double.IsNaN(Quantity))
                    AutoUpdatePricePerEach();

                canAutoUpdateTotalPrice = true;

                UpdateCanAdd();
            }
        }

        public double Quantity
        {
            get => _quantity;

            set
            {
                canAutoUpdateQuantity = false;

                SetProperty(ref _quantity, value);
                
                if(!double.IsNaN(PricePerEach))
                    AutoUpdateTotalPrice();

                if(!double.IsNaN(TotalPrice))
                    AutoUpdatePricePerEach();

                canAutoUpdateQuantity = true;

                UpdateCanAdd();
            }
        }

        public double PricePerEach
        {
            get => _pricePerEach;

            set
            {
                canAutoUpdatePricePerEach = false;

                SetProperty(ref _pricePerEach, value);

                if (!double.IsNaN(Quantity))
                    AutoUpdateTotalPrice();

                if (!double.IsNaN(TotalPrice))
                    AutoUpdateQuantity();

                canAutoUpdatePricePerEach = true;

                UpdateCanAdd();
            }
        }

        public double NetPaid
        {
            get => _netPaid;

            set
            {
                var validValue = ((!double.IsNaN(TotalPrice)) && (value > TotalPrice)) ? double.NaN : value; 

                SetProperty(ref _netPaid, validValue);

                AutoUpdateNetUnpaid();
            }
        }

        public double NetUnpaid
        {
            get => _netUnpaid;

            set => SetProperty(ref _netUnpaid, value);
        }

        public string Details
        {
            get => _details;

            set { SetProperty(ref _details, value); }
        }

        public DateTime Time
        {
            get => _time;

            set => SetProperty(ref _time, value);
        }

        public bool IsTrucksExpense
        {
            get => _isTrucksExpense;

            set
            {
                SetProperty(ref _isTrucksExpense, value);

                ProductOrService = value ? "Trucks" : string.Empty;
            }
        }

        private bool canAutoUpdateTotalPrice = true;
        private bool canAutoUpdateQuantity = true;
        private bool canAutoUpdatePricePerEach = true;
        private bool canAutoUpdateNetUnpaid = true;

        private void AutoUpdateTotalPrice()
        {
            if (DocumentReference != null)
                return;

            if (canAutoUpdateTotalPrice) 
            {
                canAutoUpdateTotalPrice = false;

                TotalPrice = double.IsNaN(Quantity) ? double.NaN : (double.IsNaN(PricePerEach) ? double.NaN : PricePerEach * Quantity);

                canAutoUpdateTotalPrice = true;
            }
        }

        private void AutoUpdateNetUnpaid()
        {
            if (DocumentReference != null)
                return;

            if (canAutoUpdateNetUnpaid)
            {
                canAutoUpdateNetUnpaid = false;

                var result = double.IsNaN(TotalPrice) ? double.NaN : (double.IsNaN(NetPaid) ? TotalPrice - 0 : TotalPrice - NetPaid);

                NetUnpaid = result < 0 ? double.NaN : result;

                canAutoUpdateNetUnpaid = true;
            }
        }

        private void AutoUpdateQuantity()
        {
            if (DocumentReference != null)
                return;

            if (canAutoUpdateQuantity)
            {
                canAutoUpdateQuantity = false;

                var result = double.IsNaN(TotalPrice) ? double.NaN : (double.IsNaN(PricePerEach) ? double.NaN : TotalPrice / PricePerEach);

                Quantity = result;

                canAutoUpdateQuantity = true;
            }
        }

        private void AutoUpdatePricePerEach()
        {
            if (DocumentReference != null)
                return;

            if (canAutoUpdatePricePerEach)
            {
                canAutoUpdatePricePerEach = false;

                var result = double.IsNaN(TotalPrice) ? double.NaN : (double.IsNaN(Quantity) ? double.NaN : TotalPrice / Quantity);

                PricePerEach = result;

                canAutoUpdatePricePerEach = true;
            }
        }

        public void UpdateCanAdd()
        {
            if (DocumentReference != null)
                return;

            CanAdd = !double.IsNaN(Quantity) && !double.IsNaN(PricePerEach) && !string.IsNullOrEmpty(ProductOrService) && !double.IsNaN(TotalPrice); 
        }

        public Task UpdateAccount(string newAccount)
        {
            if (newAccount == this.Account) 
                return null; 

            return DocumentReference.UpdateAsync(nameof(Account), newAccount);
        }

        public Task UpdateProductOrService(string newProductOrService = "Trucks")
        {
            if (newProductOrService == this.ProductOrService)
                return null;

            if (this.ProductOrService == "Trucks")
                return null;

            if (string.IsNullOrEmpty(newProductOrService))
                return null;

            return DocumentReference.UpdateAsync(nameof(ProductOrService), newProductOrService);
        }

        public Task UpdateQuantity(double newQuantity)
        {
            if (newQuantity == this.Quantity)
                return null;

            if (double.IsNaN(newQuantity) || newQuantity <= 0)
                return null;

            List<Task> updateTasks = new();

            var totalsDocumentReferences = App.MainViewModel.FirestoreViewModel.GetTotalsDocumentReferences(this.Time);

            foreach (var totalsDocumentReference in totalsDocumentReferences)
            {
                if (this.IsTrucksExpense)
                    updateTasks.Add(totalsDocumentReference.UpdateAsync(nameof(TimeRange.TotalTrucksIn), FieldValue.Increment((newQuantity - this.Quantity))));
            }

            updateTasks.Add
            (
                DocumentReference.UpdateAsync
                (
                    new Dictionary<string, object>
                    {
                        { nameof(Quantity), newQuantity },
                        { nameof(TotalPrice), newQuantity * PricePerEach },
                        { nameof(NetUnpaid), (newQuantity * PricePerEach) - NetPaid }
                    }
                )
            );

            return MainViewModel.UpdateTasks(updateTasks);
        }

        public Task UpdatePricePerEach(double newPricePerEach)
        {
            if (newPricePerEach == this.PricePerEach)
                return null;

            if (double.IsNaN(newPricePerEach) || newPricePerEach <= 0)
                return null;

            return DocumentReference.UpdateAsync
            (
                new Dictionary<string, object>
                {
                    { nameof(PricePerEach), newPricePerEach },
                    { nameof(TotalPrice), Quantity * newPricePerEach },
                    { nameof(NetUnpaid), (Quantity * newPricePerEach) - NetPaid }
                }
            );
        }

        public Task UpdateNetPaid(double newNetPaid)
        {
            if (newNetPaid > this.TotalPrice)
                return null;

            if (newNetPaid == this.NetPaid)
                return null;

            if (double.IsNaN(newNetPaid) || newNetPaid < 0)
                return null;

            var totalsDocumentReferences = App.MainViewModel.FirestoreViewModel.GetTotalsDocumentReferences(this.Time);

            List<Task> updateTasks = new();

            foreach (var totalsDocumentReference in totalsDocumentReferences)
            {
                Dictionary<string, object> updateDictionary = new()
                {
                    { nameof(TimeRange.TotalExpenses), FieldValue.Increment(newNetPaid - this.NetPaid) },
                    { nameof(TimeRange.NetProfit), FieldValue.Increment(-(newNetPaid - this.NetPaid)) } 
                };

                if (this.IsTrucksExpense)
                {
                    updateDictionary.Add(nameof(TimeRange.TotalAmountUsedInTrucks), FieldValue.Increment(newNetPaid - this.NetPaid));
                }
                else
                {
                    updateDictionary.Add(nameof(TimeRange.TotalAmountInOtherExpenses), FieldValue.Increment(newNetPaid - this.NetPaid));
                }

                totalsDocumentReference.UpdateAsync(updateDictionary);
            }

            updateTasks.Add
            (
                DocumentReference.UpdateAsync
                (
                    new Dictionary<string, object>
                    {
                        { nameof(NetPaid), newNetPaid },
                        { nameof(NetUnpaid), TotalPrice - newNetPaid }
                    }
                )
            );

            return MainViewModel.UpdateTasks(updateTasks);
        }

        public Task UpdateDetails(string newDetails)
        {
            if (newDetails == this.Details)
                return null;

            if (newDetails == null)
                newDetails = string.Empty;

            return DocumentReference.UpdateAsync(nameof(Details), newDetails);
        }
    }
}
