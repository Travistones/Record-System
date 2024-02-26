using Google.Cloud.Firestore;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Record_System.BackEnd.Constants;
using Record_System.BackEnd.Data;
using Record_System.BackEnd.Database;
using Record_System.BackEnd.Login;
using Record_System.BackEnd.ObjectPropertyChanged;
using Record_System.BackEnd.Settings;
using Record_System.BackEnd.ViewModels.Credits;
using Record_System.BackEnd.ViewModels.Employees;
using Record_System.BackEnd.ViewModels.Expenses;
using Record_System.BackEnd.ViewModels.Orders;
using Record_System.BackEnd.ViewModels.Revenues;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Record_System.BackEnd.ViewModels.Firestore
{
    public class FirestoreViewModel : BindableBase
    {
        public FirestoreViewModel()
        {
            init();
        }

        public FirestoreDb db;

        private void SetEnvironmentalVariable()
        {
            string filePath = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(Path.GetRandomFileName())) + ".json";
            File.WriteAllText(filePath, ApplicationConstants.FIRESTORE_KEY);
            File.SetAttributes(filePath, FileAttributes.Hidden);
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", filePath);
            db = FirestoreDb.Create("record-system-717f8");
            File.Delete(filePath);
        }

        private void init()
        {
            SetEnvironmentalVariable();
        }

        private DocumentReference _getTransactionDocumentOnDateTime(DateTime Time)
        {
            return _getDayCollectionOnDateTime(Time)
                .Document(ApplicationConstants.TRANSACTIONS_COLLECTION_ID);
        }

        private DocumentReference _getDayTotalsDocumentOnDateTime(DateTime Time)
        {
            return _getDayCollectionOnDateTime(Time)
                .Document(ApplicationConstants.TOTALS_DOCUMENT_ID);
        }

        private CollectionReference _getDayCollectionOnDateTime(DateTime Time)
        {
            return db
                .Collection(ApplicationConstants.TRANSACTIONS_COLLECTION_ID)
                .Document(nameof(Database.OverallTime.Years))
                .Collection(Time.Year.ToString())
                .Document(nameof(Database.Year.Months))
                .Collection(Time.ToString("MMMM"))
                .Document(nameof(Database.Month.Days))
                .Collection(Time.Day.ToString());
        }

        private List<DocumentReference> _getTotalsDocumentReferences(DateTime Time)
        {
            var overallTotalsReference = db
                .Collection(ApplicationConstants.TRANSACTIONS_COLLECTION_ID)
                .Document(ApplicationConstants.TOTALS_DOCUMENT_ID);

            var thisYearTotalsReference = overallTotalsReference
                .Parent
                .Document(nameof(Database.OverallTime.Years))
                .Collection(Time.Year.ToString())
                .Document(ApplicationConstants.TOTALS_DOCUMENT_ID);

            var thisMonthTotalsReference = thisYearTotalsReference
                .Parent
                .Document(nameof(Database.Year.Months))
                .Collection(Time.ToString("MMMM"))
                .Document(ApplicationConstants.TOTALS_DOCUMENT_ID);

            var thisDayTotalsReference = thisMonthTotalsReference
                .Parent
                .Document(nameof(Database.Month.Days))
                .Collection(Time.Day.ToString())
                .Document(ApplicationConstants.TOTALS_DOCUMENT_ID);

            List<DocumentReference> totalsDocumentReferences = new()
            {
                thisDayTotalsReference,
                thisMonthTotalsReference,
                thisYearTotalsReference,
                overallTotalsReference,
            };

            foreach (var totalsDocumentReference in totalsDocumentReferences)
            {
                totalsDocumentReference.SetAsync(new Dictionary<string, object> { { nameof(Database.TimeRange.LastModified), Time.ToString("MM/dd/yyyy hh:mm:ss tt") } }, SetOptions.MergeAll);

                if (totalsDocumentReference == overallTotalsReference)
                    continue;

                totalsDocumentReference.Parent.Parent.SetAsync(new Dictionary<string, object> { { nameof(Database.TimeRange.LastModified), Time.ToString("MM/dd/yyyy hh:mm:ss tt") } }, SetOptions.MergeAll);
            }

            return totalsDocumentReferences;
        }

        private async Task _updateDay(DateTime Time)
        {
            bool canAddDay = false;

            var years = (from _y in App.MainViewModel.OverallTimeViewModel.OverallTime.Years
                       where _y.YearNumber.ToString() == Time.ToString("yyyy")
                       select _y).ToList();

            canAddDay = years.Count <= 0;

            if (canAddDay)
                goto addDay;

            var months = (from _m in years[0].Months
                          where _m.MonthName == Time.ToString("MMMM")
                          select _m).ToList();

            canAddDay = months.Count <= 0;

            if (canAddDay)
                goto addDay;

            var days = (from _d in months[0].Days
                        where _d.DayNumber == Time.Day
                        select _d).ToList();

            canAddDay = days.Count <= 0;

            if (!canAddDay)
                return;

        addDay:
            await _getDayTotalsDocumentOnDateTime(DateTime.Now).SetAsync(new Dictionary<string, object> { { nameof(TimeRange.Day), Time.ToString("MM/dd/yyyy hh:mm:ss tt") } });
        }

        private async Task _addRevenueToDatabase(Revenue newRevenue)
        {
            await _updateDay(newRevenue.Time);

            var newRevenueDocumentReference =
                _getTransactionDocumentOnDateTime(newRevenue.Time)
                .Collection(nameof(RevenuesViewModel.Revenues))
                .Document(newRevenue.Time.ToString("yyyy-MM-dd-HH-mm-ss"));

            await newRevenueDocumentReference.SetAsync(
                new Dictionary<string, object>
                {
                    { nameof(Revenue.Account), newRevenue.Account },
                    { nameof(Revenue.Time), newRevenue.Time.ToString("MM/dd/yyyy hh:mm:ss tt") },
                    { nameof(Revenue.Volume), newRevenue.Volume },
                    { nameof(Revenue.Details), newRevenue.Details },
                    { nameof(Revenue.Price), newRevenue.Price },
                    { nameof(Revenue.Discount), (double.IsNaN(newRevenue.Discount) ? 0 : newRevenue.Discount) },
                    { nameof(Revenue.NetReceived), newRevenue.NetReceived },
                    { nameof(Revenue.IsSale), newRevenue.IsSale },
                    { nameof(Revenue.Grade), newRevenue.Grade?.Quality.ToString() },
                    { nameof(Revenue.GradeAPricePerVolume), newRevenue.GradeAPricePerVolume },
                    { nameof(Revenue.GradeBPricePerVolume), newRevenue.GradeBPricePerVolume },
                    { nameof(Revenue.GradeCPricePerVolume), newRevenue.GradeCPricePerVolume },
                    { nameof(Revenue.CanEdit), newRevenue.CanEdit }
                }
            );

            await updateRevenueTotals(newRevenue);
        }

        private async Task _addExpenseToDatabase(Expense newExpense, string id)
        {
            await _updateDay(newExpense.Time);

            var newExpenseDocumentReference =
                _getTransactionDocumentOnDateTime(newExpense.Time)
                .Collection(nameof(ExpensesViewModel.Expenses))
                .Document(id);

            await newExpenseDocumentReference.SetAsync
            (
                new Dictionary<string, object>
                {
                    { nameof(Expense.Account), newExpense.Account },
                    { nameof(Expense.ProductOrService), newExpense.ProductOrService },
                    { nameof(Expense.Quantity), newExpense.Quantity },
                    { nameof(Expense.PricePerEach), newExpense.PricePerEach },
                    { nameof(Expense.TotalPrice), newExpense.TotalPrice },
                    { nameof(Expense.NetPaid), newExpense.NetPaid },
                    { nameof(Expense.NetUnpaid), newExpense.NetUnpaid },
                    { nameof(Expense.Details), newExpense.Details },
                    { nameof(Expense.Time), newExpense.Time.ToString("MM/dd/yyyy hh:mm:ss tt") },
                    { nameof(Expense.IsTrucksExpense), newExpense.IsTrucksExpense },
                    { nameof(Expense.CanEdit), newExpense.CanEdit }
                }
            );

            await updateExpenseTotals(newExpense);
        }

        private async Task _addOrderToDatabase(Order newOrder)
        {
            await _updateDay(newOrder.Time);

            var newOrderDocumentReference =
                _getTransactionDocumentOnDateTime(newOrder.Time)
                .Collection(nameof(OrdersViewModel.Orders))
                .Document(newOrder.Time.ToString("yyyy-MM-dd-HH-mm-ss"));

            await newOrderDocumentReference.SetAsync
            (
                new Dictionary<string, object>
                {
                    { nameof(Order.Account), newOrder.Account },
                    { nameof(Order.Volume), newOrder.Volume },
                    { nameof(Order.Price), newOrder.Price },
                    { nameof(Order.Discount), newOrder.Discount },
                    { nameof(Order.NetReceived), newOrder.NetReceived },
                    { nameof(Order.Details), newOrder.Details },
                    { nameof(Order.Time), newOrder.Time.ToString("MM/dd/yyyy hh:mm:ss tt") },
                    { nameof(Order.ClosingTime), newOrder.ClosingTime?.Date.ToString("MM/dd/yyyy hh:mm:ss tt") },
                    { nameof(Order.CompletedTime), newOrder.CompletedTime?.Date.ToString("MM/dd/yyyy hh:mm:ss tt") },
                    { nameof(Order.Grade), newOrder.Grade.Quality.ToString() },
                    { nameof(Order.IsComplete), newOrder.IsComplete },
                    { nameof(Order.GradeAPricePerVolume), newOrder.GradeAPricePerVolume },
                    { nameof(Order.GradeBPricePerVolume), newOrder.GradeBPricePerVolume },
                    { nameof(Order.GradeCPricePerVolume), newOrder.GradeCPricePerVolume }
                }
            );

            await updateOrderTotals(newOrder);
        }

        private async Task _addCreditToDatabase(Credit newCredit)
        {
            await _updateDay(newCredit.Time);

            var newCreditDocumentReference =
                _getTransactionDocumentOnDateTime(newCredit.Time)
                .Collection(nameof(CreditsViewModel.Credits))
                .Document(newCredit.Time.ToString("yyyy-MM-dd-HH-mm-ss"));

            await newCreditDocumentReference.SetAsync
            (
                new Dictionary<string, object>
                {
                    { nameof(Credit.Account), newCredit.Account },
                    { nameof(Credit.Volume), newCredit.Volume },
                    { nameof(Credit.Price), newCredit.Price },
                    { nameof(Credit.NetPaid), newCredit.NetPaid },
                    { nameof(Credit.NetUnpaid), newCredit.NetUnpaid },
                    { nameof(Credit.ClosingTime), newCredit.ClosingTime?.DateTime.ToString("MM/dd/yyyy hh:mm:ss tt") },
                    { nameof(Credit.Details), newCredit.Details },
                    { nameof(Credit.Time), newCredit.Time.ToString("MM/dd/yyyy hh:mm:ss tt") },
                    { nameof(Credit.Grade), newCredit.Grade.Quality.ToString() },
                    { nameof(Credit.IsComplete), newCredit.IsComplete },
                    { nameof(Credit.CompletedTime), newCredit.CompletedTime?.DateTime.ToString("MM/dd/yyyy hh:mm:ss tt") },
                    { nameof(Credit.GradeAPricePerVolume), newCredit.GradeAPricePerVolume },
                    { nameof(Credit.GradeBPricePerVolume), newCredit.GradeBPricePerVolume },
                    { nameof(Credit.GradeCPricePerVolume), newCredit.GradeCPricePerVolume }
                }
            );

            await updateCreditTotals(newCredit);
        }

        private async Task _addEmployeeToDatabase(Employee newEmployee)
        {
            string profilePictureDownloadUrl = null;

            if (newEmployee.HasProfilePicture)
            {
                profilePictureDownloadUrl = await App.MainViewModel.FirebaseStorageViewModel.AddImage(((BitmapImage)newEmployee.ProfilePictureDownloadUrl).UriSource.AbsolutePath, newEmployee.Email);
            }

            var newEmployeeDocumentReference = db
                .Collection(nameof(EmployeesViewModel.Employees))
                .Document();

            await newEmployeeDocumentReference.SetAsync
            (
                new Dictionary<string, object>
                {
                    { nameof(Employee.FullName), newEmployee.FullName },
                    { nameof(Employee.SalaryPerMonth), newEmployee.SalaryPerMonth },
                    { nameof(Employee.DateJoined), newEmployee.DateJoined?.DateTime.ToString("MM/dd/yyyy hh:mm:ss tt") },
                    { nameof(Employee.BirthDate), newEmployee.BirthDate?.DateTime.ToString("MM/dd/yyyy hh:mm:ss tt") },
                    { nameof(Employee.Email), newEmployee.Email },
                    { nameof(Employee.PhoneNumber), newEmployee.PhoneNumber },
                    { nameof(Employee.Location), newEmployee.Location },
                    { nameof(Employee.ProfilePictureDownloadUrl), profilePictureDownloadUrl },
                    { ApplicationConstants.IS_ADMIN_KEY, false },
                    { ApplicationConstants.IS_OWNER_KEY, false }
                }
            );

            if (profilePictureDownloadUrl == null)
                return;

            newEmployee.ProfilePictureDownloadUrl = new BitmapImage(new Uri(profilePictureDownloadUrl));
        }

        public DocumentReference GetCompanySettingsDocumentReference() => db.Collection(ApplicationConstants.SETTINGS_COLLECTION_ID).Document(ApplicationConstants.COMPANY_KEY);

        public DocumentReference GetPricingSettingsDocumentReference() => db.Collection(ApplicationConstants.SETTINGS_COLLECTION_ID).Document(ApplicationConstants.PRICING_KEY);

        //private async Task _updateCompanySettings(ImageSource companyLogoImageSource, string companyName)
        //{
        //    string companyLogoDownloadUrl = string.Empty;

        //    if (companyLogoImageSource != null)
        //    {
        //        companyLogoDownloadUrl = await App.MainViewModel.FirebaseStorageViewModel.AddImage(((BitmapImage)companyLogoImageSource).UriSource.AbsolutePath, "Company Logo");
        //    }

        //    var SettingsDocumentReference = GetCompanySettingsDocumentReference();

        //    //await SettingsDocumentReference.SetAsync
        //    //(
        //    //    new Dictionary<string, object>
        //    //    {
        //    //        { nameof(ApplicationSettings.CompanyLogoDownloadUrl), companyLogoDownloadUrl },
        //    //        { nameof(ApplicationSettings.CompanyName), companyName }
        //    //    }
        //    //);
        //}

        //private DocumentReference _getUserDocumentReference(string userName) => db.Collection(ApplicationConstants.USERS_COLLECTION_ID).Document(userName);

        //private async Task<bool> _isUserNamePresent(string UserName)
        //{
        //    if (UserName != App.MainViewModel.AccountViewModel.Account.UserName)
        //    {
        //        var userSnapshot = await (_getUserDocumentReference(UserName)).GetSnapshotAsync();

        //        return (userSnapshot).Exists;
        //    }

        //    return false;
        //}

        private async Task _updateUserAccount(/*ImageSource profilePictureSource, string fullName, */ string userName, string password)
        {
            await App.MainViewModel.AccountViewModel.Account.AccountDocumentReference.UpdateAsync(new Dictionary<string, object> { { nameof(User.Account.UserName), userName }, { nameof(User.Account.Password), password } });

            //var userDocumentReference = _getUserDocumentReference(App.MainViewModel.AccountViewModel.Account.UserName);

            //var userDocumentSnapshot = await userDocumentReference.GetSnapshotAsync();

            //await userDocumentReference.UpdateAsync(nameof(User.Account.Password), password);

            //if (App.MainViewModel.AccountViewModel.Account.IsOwner)
            //{
            //    var userEmployeeAccountDocumentReference = userDocumentSnapshot.GetValue<DocumentReference>(nameof(User.Account.EmployeeAccount));

            //    string profilePictureDownloadUrl = string.Empty;

            //    if (profilePictureSource != null)
            //    {
            //        profilePictureDownloadUrl = await App.MainViewModel.FirebaseStorageViewModel.AddImage(((BitmapImage)profilePictureSource).UriSource.AbsolutePath, userName + DateTime.Now.ToString("dd-MM-yyyy-HH-mm"));
            //    }

            //    await userEmployeeAccountDocumentReference.UpdateAsync
            //    (
            //        new Dictionary<string, object>
            //        {
            //            { nameof(Employee.ProfilePictureDownloadUrl), profilePictureDownloadUrl },
            //            { nameof(Employee.FullName), fullName }
            //        }
            //    );
            //}

            //if(userName != App.MainViewModel.AccountViewModel.Account.UserName)
            //{
            //    var newUserDocumentReference = db.Collection(ApplicationConstants.USERS_COLLECTION_ID).Document(userName);

            //    await newUserDocumentReference.SetAsync
            //    (
            //        new Dictionary<string, object>
            //        {
            //            { nameof(User.Account.EmployeeAccount), userDocumentSnapshot.GetValue<DocumentReference>(nameof(User.Account.EmployeeAccount)) },
            //            { nameof(User.Account.IsOwner), userDocumentSnapshot.GetValue<bool>(nameof(User.Account.IsOwner)) }
            //        }
            //    );

            //    await userDocumentReference.DeleteAsync();

            //    return;
            //}
        }

        private async Task _updatePricingSettings(bool arePricesSet, double gradeAPrice, double gradeBPrice, double gradeCPrice)
        {
            var pricingSettingsDocumentReference = GetPricingSettingsDocumentReference();

            await pricingSettingsDocumentReference.SetAsync
            (
                new Dictionary<string, object>
                {
                    { nameof(ApplicationSettings.ArePricesSet), arePricesSet },
                    { nameof(ApplicationSettings.GradeA), gradeAPrice },
                    { nameof(ApplicationSettings.GradeB), gradeBPrice },
                    { nameof(ApplicationSettings.GradeC), gradeCPrice }
                }
            );
        }

        private async Task updateRevenueTotals(Revenue revenue, bool isAdding = true)
        {
            var totalsDocumentReferences = _getTotalsDocumentReferences(revenue.Time);

            foreach(var totalsDocumentReference in totalsDocumentReferences)
            {
                Dictionary<string, object> fieldsDictionary = new()
                {
                    { ApplicationConstants.TOTAL_REVENUES_KEY, FieldValue.Increment(isAdding ? revenue.NetReceived : -revenue.NetReceived) },
                    { nameof(TimeRange.NetProfit), FieldValue.Increment(isAdding ? revenue.NetReceived : -revenue.NetReceived) }
                };

                if (revenue.IsSale)
                {
                    fieldsDictionary.Add(nameof(Database.TimeRange.TotalSalesRevenues), FieldValue.Increment(isAdding ? revenue.NetReceived : -revenue.NetReceived));
                    fieldsDictionary.Add(nameof(Database.TimeRange.TotalVolumeOfPebbles), FieldValue.Increment(isAdding ? revenue.Volume : -revenue.Volume));
                    fieldsDictionary.Add(nameof(Database.TimeRange.TotalAmountEarnedFromPebbles), FieldValue.Increment(isAdding ? revenue.NetReceived : -revenue.NetReceived));
                    fieldsDictionary.Add($"{ApplicationConstants.TOTAL_VOLUME_OF_GRADE}{revenue.Grade.Quality}{ApplicationConstants.PEBBLES_KEY}", FieldValue.Increment(isAdding ? revenue.Volume : -revenue.Volume));
                    fieldsDictionary.Add($"{ApplicationConstants.TOTAL_AMOUNT_EARNED_FROM_GRADE}{revenue.Grade.Quality}{ApplicationConstants.PEBBLES_KEY}", FieldValue.Increment(isAdding ? revenue.NetReceived : -revenue.NetReceived));
                }
                else
                {
                    fieldsDictionary.Add(nameof(Database.TimeRange.TotalOtherRevenues), FieldValue.Increment(isAdding ? revenue.NetReceived : -revenue.NetReceived));
                }

                await totalsDocumentReference.UpdateAsync(fieldsDictionary);
            }
        }

        private async Task updateExpenseTotals(Expense expense, bool isAdding = true)
        {
            var totalsDocumentReferences = _getTotalsDocumentReferences(expense.Time);

            foreach (var totalsDocumentReference in totalsDocumentReferences)
            {
                Dictionary<string, object> fieldsDictionary = new()
                {
                    { nameof(TimeRange.TotalExpenses), FieldValue.Increment(isAdding ? expense.NetPaid : -expense.NetPaid) },
                    { nameof(TimeRange.NetProfit), FieldValue.Increment(isAdding ? -expense.NetPaid : expense.NetPaid) }
                };

                if(expense.IsTrucksExpense)
                {
                    fieldsDictionary.Add(nameof(TimeRange.TotalAmountUsedInTrucks), FieldValue.Increment(isAdding ? expense.NetPaid : -expense.NetPaid));
                    fieldsDictionary.Add(nameof(TimeRange.TotalTrucksIn), FieldValue.Increment(isAdding ? expense.Quantity : -expense.Quantity));
                }
                else
                {
                    fieldsDictionary.Add(nameof(TimeRange.TotalAmountInOtherExpenses), FieldValue.Increment(isAdding ? expense.NetPaid : -expense.NetPaid));
                }

                await totalsDocumentReference.UpdateAsync(fieldsDictionary);
            }
        }

        private async Task updateOrderTotals(Order order, bool isAdding = true)
        {
            var totalsDocumentReferences = _getTotalsDocumentReferences(order.Time);

            foreach (var totalsDocumentReference in totalsDocumentReferences)
            {
                await totalsDocumentReference.UpdateAsync
                (
                    new Dictionary<string, object>
                    {
                        { nameof(TimeRange.TotalNumberOfOrders), FieldValue.Increment(isAdding ? 1 : -1) },
                        { nameof(TimeRange.TotalAmountInOrders), FieldValue.Increment(isAdding ? order.NetReceived : -order.NetReceived) },
                        { nameof(TimeRange.TotalVolumeInOrders), FieldValue.Increment(isAdding ? order.Volume : -order.Volume) },

                        { nameof(TimeRange.TotalNumberOfIncompleteOrders), FieldValue.Increment(isAdding ? 1 : -1) },
                        { nameof(TimeRange.TotalAmountInIncompleteOrders), FieldValue.Increment(isAdding ? order.NetReceived : -order.NetReceived) },
                        { nameof(TimeRange.TotalVolumeInIncompleteOrders), FieldValue.Increment(isAdding ? order.Volume : -order.Volume) },


                        { $"{ApplicationConstants.TOTAL_NUMBER_OF_PEBBLES_KEY}{order.Grade.Quality}{nameof(OrdersViewModel.Orders)}", FieldValue.Increment(isAdding ? 1 : -1) },
                        { $"{ApplicationConstants.TOTAL_VOLUME_OF_PEBBLES_KEY}{order.Grade.Quality}{nameof(OrdersViewModel.Orders)}", FieldValue.Increment(isAdding ? order.Volume : -order.Volume) },
                        { $"{ApplicationConstants.TOTAL_AMOUNT_OF_PEBBLES_KEY}{order.Grade.Quality}{nameof(OrdersViewModel.Orders)}", FieldValue.Increment(isAdding ? order.NetReceived : -order.NetReceived) },

                        { $"{ApplicationConstants.TOTAL_NUMBER_OF_PEBBLES_KEY}{order.Grade.Quality}{ApplicationConstants.IN_INCOMPLETE_ORDERS_KEY}", FieldValue.Increment(isAdding ? 1 : -1) },
                        { $"{ApplicationConstants.TOTAL_VOLUME_OF_PEBBLES_KEY}{order.Grade.Quality}{ApplicationConstants.IN_INCOMPLETE_ORDERS_KEY}", FieldValue.Increment(isAdding ? order.Volume : -order.Volume) },
                        { $"{ApplicationConstants.TOTAL_AMOUNT_OF_PEBBLES_KEY}{order.Grade.Quality}{ApplicationConstants.IN_INCOMPLETE_ORDERS_KEY}", FieldValue.Increment(isAdding ? order.NetReceived : -order.NetReceived) }
                    }
                );
            }

        }

        private async Task updateCreditTotals(Credit credit, bool isAdding = true)
        {
            var totalsDocumentReferences = _getTotalsDocumentReferences(credit.Time);

            foreach (var totalsDocumentReference in totalsDocumentReferences)
            {
                await totalsDocumentReference.UpdateAsync
                (
                    new Dictionary<string, object>
                    {
                        { nameof(TimeRange.TotalNumberOfCredits), FieldValue.Increment(isAdding ? 1 : -1) },
                        { nameof(TimeRange.TotalAmountInCredits), FieldValue.Increment(isAdding ? credit.NetUnpaid : -credit.NetUnpaid) },
                        { nameof(TimeRange.TotalVolumeInCredits), FieldValue.Increment(isAdding ? credit.Volume : -credit.Volume) },

                        { nameof(TimeRange.TotalNumberOfIncompleteCredits), FieldValue.Increment(isAdding ? 1 : -1) },
                        { nameof(TimeRange.TotalAmountInIncompleteCredits), FieldValue.Increment(isAdding ? credit.NetUnpaid : -credit.NetUnpaid) },
                        { nameof(TimeRange.TotalVolumeInIncompleteCredits), FieldValue.Increment(isAdding ? credit.Volume : -credit.Volume) },

                        { $"{ApplicationConstants.TOTAL_NUMBER_OF_PEBBLES_KEY}{credit.Grade.Quality}{nameof(CreditsViewModel.Credits)}", FieldValue.Increment(isAdding ? 1 : -1) },
                        { $"{ApplicationConstants.TOTAL_VOLUME_OF_PEBBLES_KEY}{credit.Grade.Quality}{nameof(CreditsViewModel.Credits)}", FieldValue.Increment(isAdding ? credit.Volume : -credit.Volume) },
                        { $"{ApplicationConstants.TOTAL_AMOUNT_OF_PEBBLES_KEY}{credit.Grade.Quality}{nameof(CreditsViewModel.Credits)}", FieldValue.Increment(isAdding ? credit.NetUnpaid : -credit.NetUnpaid) },

                        { $"{ApplicationConstants.TOTAL_NUMBER_OF_PEBBLES_KEY}{credit.Grade.Quality}{ApplicationConstants.IN_INCOMPLETE_CREDITS_KEY}", FieldValue.Increment(isAdding ? 1 : -1) },
                        { $"{ApplicationConstants.TOTAL_VOLUME_OF_PEBBLES_KEY}{credit.Grade.Quality}{ApplicationConstants.IN_INCOMPLETE_CREDITS_KEY}", FieldValue.Increment(isAdding ? credit.Volume : -credit.Volume) },
                        { $"{ApplicationConstants.TOTAL_AMOUNT_OF_PEBBLES_KEY}{credit.Grade.Quality}{ApplicationConstants.IN_INCOMPLETE_CREDITS_KEY}", FieldValue.Increment(isAdding ? credit.NetUnpaid : -credit.NetUnpaid) }
                    }
                );
            }
        }

        private async Task _retrieveData()
        {
            await _retrieveEmployees();
            await _retrieveTransactions();
        }

        private async Task _retrieveEmployees()
        {
            var employeesCollectionReference = db.Collection(nameof(EmployeesViewModel.Employees));

            var employeesCollectionSnapshot = await employeesCollectionReference.GetSnapshotAsync();

            var documentSnapshots = employeesCollectionSnapshot.Documents.ToList();

            foreach(var documentSnapshot in documentSnapshots)
            {
                //if (documentSnapshot.Id == App.MainViewModel.AccountViewModel.Account.EmployeeDocumentReference.Id)
                //    continue;

                var newEmployee = new Employee();

                newEmployee.DocumentReference = documentSnapshot.Reference;

                newEmployee.AssignEmployeeFromDictionary(documentSnapshot.ToDictionary());

                App.MainViewModel.EmployeesViewModel.TotalSalary = (App.MainViewModel.EmployeesViewModel.TotalSalary + newEmployee.SalaryPerMonth);

                App.MainViewModel.EmployeesViewModel.Employees.Add(newEmployee);
            }

            App.MainViewModel.EmployeesViewModel.EmployeesCollectionReference = employeesCollectionReference;
        }

        private async Task _removeEmployee(Employee employee)
        {
            //if (employee.IsUser)
            //    await employee.UserDocumentReference.DeleteAsync();

            await employee.DocumentReference.DeleteAsync();
        }

        //private async Task _removeUser(Employee employee)
        //{
        //    if (!employee.IsUser)
        //        return;

        //    await employee.UserDocumentReference.DeleteAsync();

        //    await employee.DocumentReference.UpdateAsync
        //    (
        //        new Dictionary<string, object>
        //        {
        //            { nameof(Employee.UserDocumentReference), null },
        //            { nameof(Employee.CanModifyEmployees), false },
        //            { nameof(Employee.CanModifyPrices), false },
        //            { nameof(Employee.CanModifyCompany), false }
        //        }
        //    );
        //}

        private async Task _retrieveTransactions()
        {
            var OverallTransactionCollectionReference = db.Collection(ApplicationConstants.TRANSACTIONS_COLLECTION_ID);

            var OverallTransactionCollectionSnapshot = await OverallTransactionCollectionReference.GetSnapshotAsync();

            var OverallTransactionTotalsDocumentReference = OverallTransactionCollectionReference.Document(ApplicationConstants.TOTALS_DOCUMENT_ID);

            var YearsDocumentReference = OverallTransactionCollectionReference.Document(nameof(Database.OverallTime.Years));

            if (OverallTransactionCollectionSnapshot.Documents.Count < 1)
                goto updateListeners;

            var OverallTransactionTotalDocumentSnapshot = await OverallTransactionTotalsDocumentReference.GetSnapshotAsync();

            Dictionary<string, object> OverallTransactionsTotals = OverallTransactionTotalDocumentSnapshot.ToDictionary();

            _setTotals(OverallTransactionsTotals, App.MainViewModel.OverallTimeViewModel.OverallTime);

            var YearCollections = await YearsDocumentReference.ListCollectionsAsync().ToListAsync();

            await _addNewYearsToOverall(YearCollections);

        updateListeners:

            App.MainViewModel.OverallTimeViewModel.OverallTime.YearsDocumentReference = YearsDocumentReference;

            App.MainViewModel.OverallTimeViewModel.OverallTime.TotalsDocumentReference = OverallTransactionTotalsDocumentReference;

            App.MainViewModel.OverallTimeViewModel.OverallTime.DispatcherQueue = App.MainViewModel.DispatcherQueue;
        }

        private async Task _addNewYearsToOverall(List<CollectionReference> YearCollections)
        {
            foreach (var yearCollection in YearCollections)
            {
                var isYearAlreadyPresent = (from yearInOverall in App.MainViewModel.OverallTimeViewModel.OverallTime.Years
                                             where yearInOverall.YearNumber == int.Parse(yearCollection.Id)
                                             select yearInOverall).Count() > 0;

                if (isYearAlreadyPresent)
                    continue;

                var year = new Year();

                year.YearNumber = int.Parse(yearCollection.Id);

                var yearTotalsDocumentSnapshot = await yearCollection.Document(ApplicationConstants.TOTALS_DOCUMENT_ID).GetSnapshotAsync();

                Dictionary<string, object> yearTotalsDictionary = yearTotalsDocumentSnapshot.ToDictionary();

                _setTotals(yearTotalsDictionary, year);

                var monthsDocumentReference = yearCollection.Document(nameof(Year.Months));

                var monthCollections = await monthsDocumentReference.ListCollectionsAsync().ToListAsync();

                await _addNewMonthsToYear(year, monthCollections);

                App.MainViewModel.OverallTimeViewModel.OverallTime.Years.Add(year);

                year. TotalsDocumentReference = yearTotalsDocumentSnapshot.Reference;

                year.MonthsDocumentReference = monthsDocumentReference;
            }
        }

        private async Task _addNewMonthsToYear(Year year, List<CollectionReference> monthCollections)
        {
            foreach (var monthCollection in monthCollections)
            {
                var isMonthAlreadyPresent = (from monthInYear in year.Months
                                           where monthInYear.MonthName == monthCollection.Id
                                           select monthInYear).Count() > 0;

                if (isMonthAlreadyPresent)
                    continue;

                var month = new Month();

                month.MonthName = monthCollection.Id;

                var monthTotalsDocumentSnapshot = await monthCollection.Document(ApplicationConstants.TOTALS_DOCUMENT_ID).GetSnapshotAsync();

                Dictionary<string, object> monthTotalsDictionary = monthTotalsDocumentSnapshot.ToDictionary();

                _setTotals(monthTotalsDictionary, month);

                var daysDocumentReference = monthCollection.Document(nameof(Month.Days));

                var dayCollections = await daysDocumentReference.ListCollectionsAsync().ToListAsync();

                await _addNewDaysToMonth(month, dayCollections);

                year.Months.Add(month);

                month.DaysDocumentReference = daysDocumentReference;

                month.TotalsDocumentReference = monthTotalsDocumentSnapshot.Reference;
            }
        }

        private async Task _addNewDaysToMonth(Month month, List<CollectionReference> dayCollections)
        {
            foreach (var dayCollection in dayCollections)
            {
                var isDayAlreadyPresent = (from dayInMonth in month.Days
                                           where dayInMonth.DayNumber == int.Parse(dayCollection.Id)
                                           select dayInMonth).Count() > 0;

                if (isDayAlreadyPresent)
                    continue;

                var day = new Day();

                day.DayNumber = int.Parse(dayCollection.Id);

                var dayTotalsDocumentSnapshot = await dayCollection.Document(ApplicationConstants.TOTALS_DOCUMENT_ID).GetSnapshotAsync();

                Dictionary<string, object> dayTotalsDictionary = dayTotalsDocumentSnapshot.ToDictionary();

                _setTotals(dayTotalsDictionary, day);

                var dayTransactionsDocumentReference = dayCollection.Document(ApplicationConstants.TRANSACTIONS_COLLECTION_ID);

                var expensesCollection = dayTransactionsDocumentReference.Collection(nameof(ExpensesViewModel.Expenses));
                var revenuesCollection = dayTransactionsDocumentReference.Collection(nameof(RevenuesViewModel.Revenues));
                var ordersCollection = dayTransactionsDocumentReference.Collection(nameof(OrdersViewModel.Orders));
                var creditsCollection = dayTransactionsDocumentReference.Collection(nameof(CreditsViewModel.Credits));

                await _retrieveRevenues(day, revenuesCollection);
                await _retrieveExpenses(day, expensesCollection);
                await _retrieveCredits(day, creditsCollection);
                await _retrieveOrders(day, ordersCollection);

                month.Days.Add(day);

                day.TotalsDocumentReference = dayTotalsDocumentSnapshot.Reference;
            }
        }

        private void _setTotals(Dictionary<string, object> totalsDictionary, TimeRange timeRange)
        {
            timeRange.LastModified = DateTime.ParseExact((string)totalsDictionary[nameof(TimeRange.LastModified)], "MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);

            if (timeRange is Day)
                timeRange.Day = DateTime.ParseExact((string)totalsDictionary[nameof(TimeRange.Day)], "MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);

            totalsDictionary.Remove(nameof(TimeRange.Day));

            totalsDictionary.Remove(nameof(TimeRange.LastModified));

            foreach(string key in totalsDictionary.Keys.ToList())
            {
                timeRange.GetType().GetProperty(key).SetValue(timeRange, totalsDictionary[key]);
            }
        }

        private async Task _retrieveExpenses(Day day, CollectionReference expensesCollection)
        {
            var expenseDocumentReferences = await expensesCollection.ListDocumentsAsync().ToListAsync();

            foreach (var expenseDocumentReference in expenseDocumentReferences)
            {
                Expense newExpense = await _getExpenseFromDocumentReference(expenseDocumentReference);

                newExpense.DocumentReference = expenseDocumentReference;

                day.Expenses.Add(newExpense);

                App.MainViewModel.ExpensesViewModel.Expenses.Add(newExpense);
            }

            day.ExpensesCollectionReference = expensesCollection;

        }

        private static async Task<Expense> _getExpenseFromDocumentReference(DocumentReference expenseDocumentReference)
        {
            var expenseDocumentSnapshot = await expenseDocumentReference.GetSnapshotAsync();

            Dictionary<string, object> expenseDictionary = expenseDocumentSnapshot.ToDictionary();

            var newExpense = new Expense(expenseDictionary, expenseDocumentReference.Id);
            return newExpense;
        }

        private async Task _retrieveRevenues(Day day, CollectionReference revenueCollection)
        {
            var revenueDocumentReferences = await revenueCollection.ListDocumentsAsync().ToListAsync();

            foreach (var revenueDocumentReference in revenueDocumentReferences)
            {
                Revenue newRevenue = await _getRevenueFromDocumentReference(revenueDocumentReference);

                newRevenue.DocumentReference = revenueDocumentReference;

                day.Revenues.Add(newRevenue);

                App.MainViewModel.RevenuesViewModel.Revenues.Add(newRevenue);
            }

            day.RevenuesCollectionReference = revenueCollection;

        }

        private static async Task<Revenue> _getRevenueFromDocumentReference(DocumentReference revenueDocumentReference)
        {

            var revenueDocumentSnapshot = await revenueDocumentReference.GetSnapshotAsync();

            Dictionary<string, object> revenueDictionary = revenueDocumentSnapshot.ToDictionary();

            var newRevenue = new Revenue(revenueDictionary, revenueDocumentReference.Id);
            return newRevenue;
        }

        private async Task _retrieveOrders(Day day, CollectionReference orderCollection)
        {
            var orderDocumentReferences = await orderCollection.ListDocumentsAsync().ToListAsync();

            foreach (var orderDocumentReference in orderDocumentReferences)
            {
                Order newOrder = await _getOrderFromDocumentReference(orderDocumentReference);

                newOrder.DocumentReference = orderDocumentReference;

                day.Orders.Add(newOrder);

                App.MainViewModel.OrdersViewModel.Orders.Add(newOrder);
            }

            day.OrdersCollectionReference = orderCollection;
        }

        private static async Task<Order> _getOrderFromDocumentReference(DocumentReference orderDocumentReference)
        {
            var orderDocumentSnapshot = await orderDocumentReference.GetSnapshotAsync();

            Dictionary<string, object> orderDictionary = orderDocumentSnapshot.ToDictionary();

            var newOrder = new Order(orderDictionary, orderDocumentReference.Id);
            return newOrder;
        }

        private async Task _retrieveCredits(Day day, CollectionReference creditCollection)
        {
            var creditDocumentReferences = await creditCollection.ListDocumentsAsync().ToListAsync();

            foreach (var creditDocumentReference in creditDocumentReferences)
            {
                Credit newCredit = await _getCreditFromDocumentReference(creditDocumentReference);

                newCredit.DocumentReference = creditDocumentReference;

                day.Credits.Add(newCredit);

                App.MainViewModel.CreditsViewModel.Credits.Add(newCredit);
            }

            day.CreditsCollectionReference = creditCollection;
        }

        private async Task<Credit> _getCreditFromDocumentReference(DocumentReference creditDocumentReference)
        {
            var creditDocumentSnapshot = await creditDocumentReference.GetSnapshotAsync();

            Dictionary<string, object> creditDictionary = creditDocumentSnapshot.ToDictionary();

            var newCredit = new Credit(creditDictionary, creditDocumentReference.Id);
            return newCredit;
        }

        private async Task _removeRevenue(Revenue revenue)
        {
            var revenueDocumentReference =
                _getTransactionDocumentOnDateTime(revenue.Time)
                .Collection(nameof(RevenuesViewModel.Revenues))
                .Document(revenue.Id);

            var revenueDocumentSnapshot = await revenueDocumentReference.GetSnapshotAsync();

            if (!revenueDocumentSnapshot.Exists)
                return;

            await updateRevenueTotals(revenue, false);

            await revenueDocumentSnapshot.Reference.DeleteAsync();
        }

        private async Task _removeExpense(Expense expense)
        {
            var expenseDocumentSnapshot = await expense.DocumentReference.GetSnapshotAsync();

            if (!expenseDocumentSnapshot.Exists)
                return;

            await updateExpenseTotals(expense, false);

            var result = await expenseDocumentSnapshot.Reference.DeleteAsync();
        }

        private async Task _removeCredit(Credit credit)
        {
            var creditDocumentReference =
                _getTransactionDocumentOnDateTime(credit.Time)
                .Collection(nameof(CreditsViewModel.Credits))
                .Document(credit.Id);

            var creditDocumentSnapshot = await creditDocumentReference.GetSnapshotAsync();

            if (!creditDocumentSnapshot.Exists)
                return;

            await updateCreditTotals(credit, false);

            await creditDocumentSnapshot.Reference.DeleteAsync();
        }

        private async Task _removeOrder(Order order)
        {
            var orderDocumentReference =
                _getTransactionDocumentOnDateTime(order.Time)
                .Collection(nameof(OrdersViewModel.Orders))
                .Document(order.Id);

            var orderDocumentSnapshot = await orderDocumentReference.GetSnapshotAsync();

            if (!orderDocumentSnapshot.Exists)
                return;

            await updateOrderTotals(order, false);

            await orderDocumentSnapshot.Reference.DeleteAsync();
        }

        //private async Task _addSystemUser(Employee employee, bool canModifyPrices, bool canModifyEmployees, bool canModifyCompany)
        //{
        //    var newUserDocumentReference = db
        //        .Collection(ApplicationConstants.USERS_COLLECTION_ID)
        //        .Document(employee.Email);

        //    await newUserDocumentReference.SetAsync
        //    (
        //        new Dictionary<string, object>
        //        {
        //            { nameof(User.Account.UserName), employee.Email },
        //            { nameof(User.Account.EmployeeDocumentReference), employee.DocumentReference },
        //            { nameof(User.Account.Password), employee.Email },
        //            { nameof(User.Account.IsOwner), false },
        //        }
        //    );

        //    await employee.DocumentReference.UpdateAsync
        //    (
        //        new Dictionary<string, object>
        //        {
        //            { nameof(Employee.CanModifyCompany), canModifyCompany },
        //            { nameof(Employee.CanModifyEmployees), canModifyEmployees },
        //            { nameof(Employee.CanModifyPrices), canModifyPrices },
        //            { nameof(Employee.IsUser), true }
        //        }
        //    );
        //}

        private async Task<LoginResults> _login(string _userName, string _password)
        {
            try
            {
                var userDocument = db.Collection(ApplicationConstants.USERS_COLLECTION_ID).Document("SystemUser");

                var userDocumentSnapshot = await userDocument.GetSnapshotAsync();

                if (_userName != userDocumentSnapshot.GetValue<string>(nameof(User.Account.UserName)))
                    return LoginResults.IncorrectPasswordOrUserName;

                if (_password != userDocumentSnapshot.GetValue<string>(nameof(User.Account.Password)))
                    return LoginResults.IncorrectPasswordOrUserName;

                //Dictionary<string, object> accountDictionary = userDocumentSnapshot.ToDictionary();

                App.MainViewModel.AccountViewModel.Account.Password = _password;
                App.MainViewModel.AccountViewModel.Account.UserName = _userName;
                //App.MainViewModel.AccountViewModel.Account.IsOwner = (bool)accountDictionary[nameof(User.Account.IsOwner)];
                App.MainViewModel.AccountViewModel.Account.AccountDocumentReference = userDocument;

                //DocumentReference employeeDocumentReference = null;

                //employeeDocumentReference = (DocumentReference)accountDictionary[nameof(User.Account.EmployeeDocumentReference)];

                //var employeeDocumentSnapshot = await employeeDocumentReference.GetSnapshotAsync();

                //App.MainViewModel.AccountViewModel.Account.EmployeeAccount.AssignEmployeeFromDictionary(employeeDocumentSnapshot.ToDictionary());
                //App.MainViewModel.AccountViewModel.Account.EmployeeDocumentReference = employeeDocumentReference;
                //App.MainViewModel.AccountViewModel.Account.EmployeeAccount.DocumentReference = employeeDocumentReference;

                return LoginResults.Success;
            }
            catch
            {
                return LoginResults.LostConnection;
            }
        }

        public async Task<LoginResults> Login(string userName, string password) => await _login(userName, password);

        //public async Task AddSystemUser(Employee employee, bool canModifyPrice, bool canModifyEmployees, bool canModifyCompany) => await _addSystemUser(employee, canModifyPrice, canModifyEmployees, canModifyCompany);

        public async Task RemoveRevenue(Revenue revenue) => await _removeRevenue(revenue);

        public async Task RemoveExpense(Expense expense) => await _removeExpense(expense);

        public async Task RemoveOrder(Order order) => await _removeOrder(order);

        public async Task RemoveCredit(Credit credit) => await _removeCredit(credit);

        public async Task RemoveEmployee(Employee employee) => await _removeEmployee(employee);

        //public async Task RemoveUser(Employee employee) => await _removeUser(employee);

        public async Task AddRevenueToDatabase(Revenue newRevenue) => await _addRevenueToDatabase(newRevenue);

        public async Task AddExpenseToDatabase(Expense newExpense, string id) => await _addExpenseToDatabase(newExpense, id);

        public async Task AddOrderToDatabase(Order newOrder) => await _addOrderToDatabase(newOrder);

        public async Task AddCreditToDatabase(Credit newCredit) => await _addCreditToDatabase(newCredit);

        public async Task AddEmployeeToDatabase(Employee newEmployee) => await _addEmployeeToDatabase(newEmployee);

        //public async Task UpdateCompanySettings(ImageSource companyLogoImageSource, string companyName) => await _updateCompanySettings(companyLogoImageSource, companyName);

        //public async Task<bool> IsUserNamePresent(string userName) => await _isUserNamePresent(userName);

        public async Task UpdateUserAccount(/*ImageSource profilePictureSource, string fullName, */ string userName, string password) => await _updateUserAccount(/*profilePictureSource, fullName,*/ userName, password);

        //public DocumentReference GetUserDocumentReference(string userName) => _getUserDocumentReference(userName);

        public async Task UpdatePricingSettings(bool arePricesSet, double gradeAPrice, double gradeBPrice, double gradeCPrice) => await _updatePricingSettings(arePricesSet, gradeAPrice, gradeBPrice, gradeCPrice);

        public async Task RetrieveData() => await _retrieveData();

        public void SetTotals(Dictionary<string, object> dictionary, TimeRange timeRange) => _setTotals(dictionary, timeRange);

        public async Task AddNewYearsToOverall(List<CollectionReference> yearCollections) => await _addNewYearsToOverall(yearCollections);

        public async Task AddNewMonthsToYear(Year year, List<CollectionReference> monthCollections) => await _addNewMonthsToYear(year, monthCollections);

        public async Task AddNewDaysToMonth(Month month, List<CollectionReference> dayCollections) => await _addNewDaysToMonth(month, dayCollections);

        public async Task<Credit> GetCreditFromDocumentReference(DocumentReference creditDocumentReference) => await _getCreditFromDocumentReference(creditDocumentReference);

        public async Task<Order> GetOrderFromDocumentReference(DocumentReference orderDocumentReference) => await _getOrderFromDocumentReference(orderDocumentReference);

        public async Task<Revenue> GetRevenueFromDocumentReference(DocumentReference revenueDocumentReference) => await _getRevenueFromDocumentReference(revenueDocumentReference);

        public async Task<Expense> GetExpenseFromDocumentReference(DocumentReference expenseDocumentReference) => await _getExpenseFromDocumentReference(expenseDocumentReference);

        public DocumentReference GetTransactionDocumentOnDateTime(DateTime Time) => _getTransactionDocumentOnDateTime(Time);

        public List<DocumentReference> GetTotalsDocumentReferences(DateTime Time) => _getTotalsDocumentReferences(Time);
    }
}
