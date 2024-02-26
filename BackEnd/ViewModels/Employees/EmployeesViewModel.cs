using Google.Cloud.Firestore;
using Record_System.BackEnd.Data;
using Record_System.BackEnd.ObjectPropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Record_System.BackEnd.ViewModels.Employees
{
    public class EmployeesViewModel : BindableBase
    {
        public EmployeesViewModel() 
        {
            Employees = new();
        }

        private ObservableCollection<Employee> _employees;
        private CollectionReference _employeesCollectionReference;
        private FirestoreChangeListener _employeeCollectionListener;
        private double _totalSalary;

        public double TotalSalary
        {
            get => _totalSalary;

            set => SetProperty(ref _totalSalary, value);
        }

        public ObservableCollection<Employee> Employees
        {
            get => _employees;

            private set => _employees = value;
        }

        private async Task addNewEmployee(Employee newEmployee)
        {
            await App.MainViewModel.FirestoreViewModel.AddEmployeeToDatabase(newEmployee);
        }

        public async Task AddNewEmployee(Employee newEmployee) => await this.addNewEmployee(newEmployee);

        private async Task deleteEmployee(List<Employee> employeesToDelete)
        {
            if (employeesToDelete.Count == 0) return;

            foreach (var employee in employeesToDelete.ToList())
            {
                await App.MainViewModel.FirestoreViewModel.RemoveEmployee(employee);
            }
        }

        public async Task DeleteEmployees(List<Employee> employeesToDelete) => await deleteEmployee(employeesToDelete);

        public CollectionReference EmployeesCollectionReference
        {
            get => _employeesCollectionReference;

            set
            {
                _employeesCollectionReference = value;

                _setEmployeeCollectionListener();
            }
        }

        public async Task PayAll()
        {
            for (int i = 0; i < Employees.Count; i++)
            {
                await PayEmployee(Employees[i], i);
            }
        }

        public async Task PayEmployee(Employee employeeToPay, int number = 0)
        {
            await App.MainViewModel.ExpensesViewModel.AddNewExpense
            (
                new Expense()
                {
                    IsTrucksExpense = false,
                    Account = employeeToPay.FullName,
                    ProductOrService = "Salary Per Month",
                    Quantity = 1,
                    PricePerEach = employeeToPay.SalaryPerMonth,
                    TotalPrice = employeeToPay.SalaryPerMonth,
                    NetPaid = employeeToPay.SalaryPerMonth,
                    NetUnpaid = 0,
                    Details = "Salary Per Month",
                    Time = DateTime.Now
                },
                number
            );
        }

        public bool IsEmailPresent(string emailToCheck)
        {
            return (from employee in Employees
                    where employee.Email == emailToCheck
                    select employee).ToList().Count > 0;
        }

        private void _setEmployeeCollectionListener()
        {
            _employeeCollectionListener = EmployeesCollectionReference.Listen
            (
                snapshot =>
                {
                    if (App.MainViewModel.OverallTimeViewModel.OverallTime.DispatcherQueue == null)
                        return;

                    App.MainViewModel.OverallTimeViewModel.OverallTime.DispatcherQueue.TryEnqueue
                    (
                        Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () =>
                        {
                            foreach (DocumentChange change in snapshot.Changes)
                            {
                                if (change.ChangeType == DocumentChange.Type.Added)
                                {
                                    var newEmployee = new Employee();

                                    newEmployee.DocumentReference = change.Document.Reference;

                                    newEmployee.AssignEmployeeFromDictionary(change.Document.ToDictionary());

                                    TotalSalary = (TotalSalary + newEmployee.SalaryPerMonth);

                                    Employees.Add(newEmployee);
                                }
                                else if (change.ChangeType == DocumentChange.Type.Modified)
                                {
                                    var modifiedEmployees = (from employee in Employees
                                                            where change.Document.Reference.Id == employee.DocumentReference.Id
                                                            select employee).ToList();

                                    if (modifiedEmployees.Count < 1)
                                        break;

                                    var employeeDictionary = change.Document.ToDictionary();

                                    TotalSalary = (TotalSalary + (((double)employeeDictionary[nameof(Employee.SalaryPerMonth)]) - modifiedEmployees[0].SalaryPerMonth));

                                    modifiedEmployees[0].AssignEmployeeFromDictionary(employeeDictionary);
                                }
                                else if (change.ChangeType == DocumentChange.Type.Removed)
                                {
                                    var removedEmployees = (from employee in Employees
                                                          where change.Document.Reference.Id == employee.DocumentReference.Id
                                                          select employee).ToList();

                                    if (removedEmployees.Count < 1)
                                        break;

                                    var removedEmployee = removedEmployees[0];

                                    TotalSalary = (TotalSalary - removedEmployee.SalaryPerMonth);

                                    Employees.Remove(removedEmployee);
                                }
                            }
                        }
                    );
                }
            );
        }

    }
}
