using Record_System.BackEnd.Data;
using Record_System.BackEnd.Database;
using Record_System.BackEnd.ObjectPropertyChanged;
using Record_System.BackEnd.ViewModels.Expenses;
using Record_System.BackEnd.ViewModels.Revenues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Record_System.Pages.Content.Expenses.ViewModel
{
    public class ExpensesPageViewModel : BindableBase
    {
        public ExpensesViewModel ExpensesViewModel;

        public OverallTime OverallTime;

        public ExpensesPageViewModel()
        {
            ExpensesViewModel = App.MainViewModel.ExpensesViewModel;

            OverallTime = App.MainViewModel.OverallTimeViewModel.OverallTime;
        }

        private Expense _newExpense;

        public Expense NewExpense
        {
            get => _newExpense;

            set => SetProperty(ref _newExpense, value);
        }

        private bool _canDeleteExpenses;

        public bool CanDeleteExpenses
        {
            get => _canDeleteExpenses;

            set => SetProperty(ref _canDeleteExpenses, value);
        }

        public void CreateNewExpense()
        {
            NewExpense = new();
        }

        public async Task AddToExpenses()
        {
            await ExpensesViewModel.AddNewExpense(NewExpense);
        }

        public async Task DeleteExpenses(List<Expense> expensesToDelete)
        {
            await ExpensesViewModel.DeleteExpenses(expensesToDelete);
        }
    }
}
