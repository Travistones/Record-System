using Google.Cloud.Firestore;
using OxyPlot;
using Record_System.BackEnd.Data;
using Record_System.BackEnd.ViewModels.Main;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Record_System.BackEnd.Database
{
    public class Day : TimeRange
    {
        public Day()
        {
            Revenues = new();
            Expenses = new();
            Orders = new();
            Credits = new();
        }

        private ObservableCollection<Revenue> _revenues;
        private ObservableCollection<Expense> _expenses;
        private ObservableCollection<Order> _orders;
        private ObservableCollection<Credit> _credits;

        public ObservableCollection<Revenue> Revenues
        {
            get => _revenues;

            private set => SetProperty(ref _revenues, value);
        }
        
        public ObservableCollection<Expense> Expenses
        {
            get => _expenses;

            private set => SetProperty(ref _expenses, value);
        }

        public ObservableCollection<Order> Orders
        {
            get => _orders;

            private set => SetProperty(ref _orders, value);
        }

        public ObservableCollection<Credit> Credits
        {
            get => _credits;

            private set => SetProperty(ref _credits, value);
        }

        private int _dayNumber;

        public int DayNumber
        {
            get => _dayNumber;

            set => SetProperty(ref _dayNumber, value);
        }

        private CollectionReference _creditsCollectionReference;

        private CollectionReference _expensesCollectionReference;

        private CollectionReference _ordersCollectionReference;

        private CollectionReference _revenuesCollectionReference;

        public CollectionReference CreditsCollectionReference
        {
            get => _creditsCollectionReference;

            set
            {
                _creditsCollectionReference = value;

                _setCreditsChangeListener();
            }
        }

        public CollectionReference ExpensesCollectionReference
        {
            get => _expensesCollectionReference;

            set
            {
                _expensesCollectionReference = value;

                _setExpensesChangeListener();
            }
        }

        public CollectionReference OrdersCollectionReference
        {
            get => _ordersCollectionReference;

            set
            {
                _ordersCollectionReference = value;

                _setOrdersChangeListener();
            }
        }

        public CollectionReference RevenuesCollectionReference
        {
            get => _revenuesCollectionReference;

            set
            {
                _revenuesCollectionReference = value;

                _setRevenuesChangeListener();
            }
        }

        private FirestoreChangeListener _revenuesChangeListener;

        private FirestoreChangeListener _creditsChangeListener;

        private FirestoreChangeListener _ordersChangeListener;

        private FirestoreChangeListener _expensesChangeListener;

        private void _setRevenuesChangeListener()
        {
            _revenuesChangeListener = RevenuesCollectionReference.Listen
            (
                snapshot =>
                {
                    if (App.MainViewModel.OverallTimeViewModel.OverallTime.DispatcherQueue == null)
                        return;

                    App.MainViewModel.OverallTimeViewModel.OverallTime.DispatcherQueue.TryEnqueue
                    (
                        Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, async () =>
                        {
                            foreach (DocumentChange change in snapshot.Changes)
                            {
                                if (change.ChangeType == DocumentChange.Type.Added)
                                {
                                    var isRevenuePresent = (from revenue in Revenues
                                                          where change.Document.Id == revenue.Id
                                                          select revenue).ToList().Count > 0;

                                    if (isRevenuePresent)
                                        break;

                                    Revenue newRevenue = await App.MainViewModel.FirestoreViewModel.GetRevenueFromDocumentReference(change.Document.Reference);

                                    newRevenue.DocumentReference = change.Document.Reference;

                                    this.Revenues.Insert(0, newRevenue);

                                    App.MainViewModel.RevenuesViewModel.Revenues.Insert(0, newRevenue);
                                }
                                else if (change.ChangeType == DocumentChange.Type.Modified)
                                {
                                    var modifiedRevenue = (from revenue in Revenues
                                                           where change.Document.Id == revenue.Id
                                                           select revenue).ToList()[0];

                                    if (modifiedRevenue == null)
                                        break;

                                    var changeDictionary = change.Document.ToDictionary();

                                    modifiedRevenue.Details = (string)changeDictionary[nameof(Revenue.Details)];
                                    modifiedRevenue.Account = (string)changeDictionary[nameof(Revenue.Account)];
                                    modifiedRevenue.NetReceived = (double)changeDictionary[nameof(Revenue.NetReceived)];

                                    if (!modifiedRevenue.IsSale)
                                        break;

                                    modifiedRevenue.Grade = App.MainViewModel.SettingsViewModel.ApplicationSettings.GetGradeFromString((string)changeDictionary[nameof(Grade)]);
                                    modifiedRevenue.Volume = (double)changeDictionary[nameof(Revenue.Volume)];
                                    modifiedRevenue.Price = (double)changeDictionary[nameof(Revenue.Price)];
                                    modifiedRevenue.Discount = (double)changeDictionary[nameof(Revenue.Discount)];
                                }
                                else if (change.ChangeType == DocumentChange.Type.Removed)
                                {
                                    var removedRevenue = (from revenue in Revenues
                                                           where change.Document.Id == revenue.Id
                                                           select revenue).ToList()[0];

                                    if (removedRevenue == null)
                                        break;

                                    Revenues.Remove(removedRevenue);

                                    App.MainViewModel.RevenuesViewModel.Revenues.Remove(removedRevenue);
                                }
                            }
                        }
                    );
                }
            );
        }
        
        private void _setExpensesChangeListener()
        {
            _expensesChangeListener = ExpensesCollectionReference.Listen
            (
                snapshot =>
                {
                    if (App.MainViewModel.OverallTimeViewModel.OverallTime.DispatcherQueue == null)
                        return;

                    App.MainViewModel.OverallTimeViewModel.OverallTime.DispatcherQueue.TryEnqueue
                    (
                        Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, async () =>
                        {
                            foreach (DocumentChange change in snapshot.Changes)
                            {
                                if (change.ChangeType == DocumentChange.Type.Added)
                                {
                                    var isExpensePresent = (from expense in Expenses
                                                            where change.Document.Id == expense.Id
                                                            select expense).ToList().Count > 0;

                                    if (isExpensePresent)
                                        break;

                                    Expense newExpense = await App.MainViewModel.FirestoreViewModel.GetExpenseFromDocumentReference(change.Document.Reference);

                                    newExpense.DocumentReference = change.Document.Reference;

                                    this.Expenses.Insert(0, newExpense);

                                    App.MainViewModel.ExpensesViewModel.Expenses.Insert(0, newExpense);
                                }
                                else if (change.ChangeType == DocumentChange.Type.Modified)
                                {
                                    var modifiedExpense = (from expense in Expenses
                                                           where change.Document.Id == expense.Id
                                                           select expense).ToList()[0];

                                    if (modifiedExpense == null)
                                        break;

                                    var changeDictionary = change.Document.ToDictionary();

                                    modifiedExpense.Account = (string)changeDictionary[nameof(Expense.Account)];
                                    modifiedExpense.ProductOrService = (string)changeDictionary[nameof(Expense.ProductOrService)];
                                    modifiedExpense.Quantity = (double)changeDictionary[nameof(Expense.Quantity)];
                                    modifiedExpense.PricePerEach = (double)changeDictionary[nameof(Expense.PricePerEach)];
                                    modifiedExpense.TotalPrice = (double)changeDictionary[nameof(Expense.TotalPrice)];
                                    modifiedExpense.NetPaid = (double)changeDictionary[nameof(Expense.NetPaid)];
                                    modifiedExpense.NetUnpaid = (double)changeDictionary[nameof(Expense.NetUnpaid)];
                                    modifiedExpense.Details = (string)changeDictionary[nameof(Expense.Details)];
                                }
                                else if (change.ChangeType == DocumentChange.Type.Removed)
                                {
                                    var removedExpense = (from expense in Expenses
                                                          where change.Document.Id == expense.Id
                                                          select expense).ToList()[0];

                                    if (removedExpense == null)
                                        break;

                                    Expenses.Remove(removedExpense);

                                    App.MainViewModel.ExpensesViewModel.Expenses.Remove(removedExpense);
                                }
                            }
                        }
                    );
                }
            );
        }
        
        private void _setOrdersChangeListener()
        {
            _ordersChangeListener = OrdersCollectionReference.Listen
            (
                snapshot =>
                {
                    if (App.MainViewModel.OverallTimeViewModel.OverallTime.DispatcherQueue == null)
                        return;

                    App.MainViewModel.OverallTimeViewModel.OverallTime.DispatcherQueue.TryEnqueue
                    (
                        Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, async () =>
                        {
                            foreach (DocumentChange change in snapshot.Changes)
                            {
                                if (change.ChangeType == DocumentChange.Type.Added)
                                {
                                    var isOrderPresent = (from order in Orders
                                                            where change.Document.Id == order.Id
                                                            select order).ToList().Count > 0;

                                    if (isOrderPresent)
                                        break;

                                    Order newOrder = await App.MainViewModel.FirestoreViewModel.GetOrderFromDocumentReference(change.Document.Reference);

                                    newOrder.DocumentReference = change.Document.Reference;

                                    this.Orders.Insert(0, newOrder);

                                    App.MainViewModel.OrdersViewModel.Orders.Insert(0, newOrder);
                                }
                                else if (change.ChangeType == DocumentChange.Type.Modified)
                                {
                                    var modifiedOrder = (from order in Orders
                                                           where change.Document.Id == order.Id
                                                           select order).ToList()[0];

                                    if (modifiedOrder == null)
                                        break;

                                    var changeDictionary = change.Document.ToDictionary();

                                    modifiedOrder.Account = (string)changeDictionary[nameof(Order.Account)];
                                    modifiedOrder.Grade = App.MainViewModel.SettingsViewModel.ApplicationSettings.GetGradeFromString((string)changeDictionary[nameof(Grade)]);
                                    modifiedOrder.Volume = (double)changeDictionary[nameof(Order.Volume)];
                                    modifiedOrder.Price = (double)changeDictionary[nameof(Order.Price)];
                                    modifiedOrder.Discount = (double)changeDictionary[nameof(Order.Discount)];
                                    modifiedOrder.NetReceived = (double)changeDictionary[nameof(Order.NetReceived)];
                                    modifiedOrder.Details = (string)changeDictionary[nameof(Order.Details)];
                                    modifiedOrder.IsComplete = (bool)changeDictionary[nameof(Order.IsComplete)];
                                    modifiedOrder.ClosingTime = DateTimeOffset.ParseExact((string)changeDictionary[nameof(Order.ClosingTime)], "MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
                                    modifiedOrder.CompletedTime = ((changeDictionary[nameof(Order.CompletedTime)] != null) && ((string)changeDictionary[nameof(Order.CompletedTime)] != string.Empty)) ? DateTimeOffset.ParseExact((string)changeDictionary[nameof(Order.CompletedTime)], "MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture) : null;
                                }
                                else if (change.ChangeType == DocumentChange.Type.Removed)
                                {
                                    var removedOrder = (from order in Orders
                                                          where change.Document.Id == order.Id
                                                          select order).ToList()[0];

                                    if (removedOrder == null)
                                        break;

                                    Orders.Remove(removedOrder);

                                    App.MainViewModel.OrdersViewModel.Orders.Remove(removedOrder);
                                }
                            }
                        }
                    );
                }
            );
        }
        
        private void _setCreditsChangeListener()
        {
            _creditsChangeListener = CreditsCollectionReference.Listen
            (
                snapshot =>
                {
                    if (App.MainViewModel.OverallTimeViewModel.OverallTime.DispatcherQueue == null)
                        return;

                    App.MainViewModel.OverallTimeViewModel.OverallTime.DispatcherQueue.TryEnqueue
                    (
                        Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, async () =>
                        {
                            foreach (DocumentChange change in snapshot.Changes)
                            {
                                if (change.ChangeType == DocumentChange.Type.Added)
                                {
                                    var isCreditPresent = (from credit in Credits
                                                            where change.Document.Id == credit.Id
                                                            select credit).ToList().Count > 0;

                                    if (isCreditPresent)
                                        break;

                                    Credit newCredit = await App.MainViewModel.FirestoreViewModel.GetCreditFromDocumentReference(change.Document.Reference);

                                    newCredit.DocumentReference = change.Document.Reference;

                                    this.Credits.Insert(0, newCredit);

                                    App.MainViewModel.CreditsViewModel.Credits.Insert(0, newCredit);
                                } 
                                else if (change.ChangeType == DocumentChange.Type.Modified)
                                {
                                    var modifiedCredit = (from credit in Credits
                                                         where change.Document.Id == credit.Id
                                                         select credit).ToList()[0];

                                    if (modifiedCredit == null)
                                        break;

                                    var changeDictionary = change.Document.ToDictionary();

                                    modifiedCredit.Account = (string)changeDictionary[nameof(Credit.Account)];
                                    modifiedCredit.Grade = App.MainViewModel.SettingsViewModel.ApplicationSettings.GetGradeFromString((string)changeDictionary[nameof(Grade)]);
                                    modifiedCredit.Volume = (double)changeDictionary[nameof(Credit.Volume)];
                                    modifiedCredit.Price = (double)changeDictionary[nameof(Credit.Price)];
                                    modifiedCredit.NetPaid = (double)changeDictionary[nameof(Credit.NetPaid)];
                                    modifiedCredit.NetUnpaid = (double)changeDictionary[nameof(Credit.NetUnpaid)];
                                    modifiedCredit.Details = (string)changeDictionary[nameof(Credit.Details)];
                                    modifiedCredit.ClosingTime = DateTimeOffset.ParseExact((string)changeDictionary[nameof(Credit.ClosingTime)], "MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
                                    modifiedCredit.IsComplete = (bool)changeDictionary[nameof(Credit.IsComplete)];
                                    modifiedCredit.CompletedTime = ((changeDictionary[nameof(Credit.CompletedTime)] != null) && ((string)changeDictionary[nameof(Credit.CompletedTime)] != string.Empty)) ? DateTimeOffset.ParseExact((string)changeDictionary[nameof(Credit.CompletedTime)], "MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture) : null;
                                }
                                else if (change.ChangeType == DocumentChange.Type.Removed)
                                {
                                    var removedCredit = (from credit in Credits
                                                          where change.Document.Id == credit.Id
                                                          select credit).ToList()[0];

                                    if (removedCredit == null)
                                        break;

                                    Credits.Remove(removedCredit);

                                    App.MainViewModel.CreditsViewModel.Credits.Remove(removedCredit);
                                }
                            }
                        }
                    );
                }
            );
        }

    }
}
