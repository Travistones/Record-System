using Record_System.BackEnd.Data;
using Record_System.BackEnd.ObjectPropertyChanged;
using Record_System.BackEnd.ViewModels.Account;
using Record_System.BackEnd.ViewModels.Employees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Record_System.Pages.Content.Employees.ViewModel
{
    public class EmployeesPageViewModel : BindableBase
    {
        public EmployeesViewModel EmployeesViewModel;

        private Employee _selectedEmployee;

        private bool _isEdit;

        public AccountViewModel AccountViewModel;

        public EmployeesPageViewModel() 
        {
            EmployeesViewModel = App.MainViewModel.EmployeesViewModel;

            AccountViewModel = App.MainViewModel.AccountViewModel;
        }

        private bool _canDeleteEmployees;
        private Employee newEmployee;

        public bool IsEdit
        {
            get => _isEdit;

            set => SetProperty(ref _isEdit, value);
        }

        public Employee NewEmployee
        {
            get => newEmployee;

            set => SetProperty(ref newEmployee, value);
        }

        public Employee SelectedEmployee
        {
            get => _selectedEmployee;

            set => SetProperty(ref _selectedEmployee, value);
        }

        public bool CanDeleteEmployees
        {
            get => _canDeleteEmployees;

            set => SetProperty(ref _canDeleteEmployees, value);
        }

        public void CreateNewEmployee()
        {
            NewEmployee = new();
        }

        public void EditEmployee(Employee employee)
        {
            NewEmployee = new();

            NewEmployee.ProfilePictureDownloadUrl = employee.ProfilePictureDownloadUrl;
            NewEmployee.FullName = employee.FullName;
            NewEmployee.Location = employee.Location;
            NewEmployee.Email = employee.Email;
            NewEmployee.PhoneNumber = employee.PhoneNumber;
            NewEmployee.SalaryPerMonth = employee.SalaryPerMonth;
            NewEmployee.BirthDate = employee.BirthDate;
            NewEmployee.DateJoined = employee.DateJoined;

            IsEdit = true;
        }

        public async Task AddToEmployees() => await EmployeesViewModel.AddNewEmployee(NewEmployee);

        public async Task DeleteEmployees(List<Employee> employeesToDelete) => await EmployeesViewModel.DeleteEmployees(employeesToDelete);
    }
}
