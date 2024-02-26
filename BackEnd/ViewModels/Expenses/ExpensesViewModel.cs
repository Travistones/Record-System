using Record_System.BackEnd.Data;
using Record_System.BackEnd.ObjectPropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Record_System.BackEnd.ViewModels.Expenses
{
    public class ExpensesViewModel : BindableBase
    {
        public ExpensesViewModel()
        {
            Expenses = new();
        }

        private ObservableCollection<Expense> _expenses;

        public ObservableCollection<Expense> Expenses
        {
            get => _expenses;

            private set => _expenses = value;
        }

        private async Task addNewExpense(Expense newExpense, int number = 0)
        {
            if (string.IsNullOrEmpty(newExpense.Account))
                newExpense.Account = "-";

            if (string.IsNullOrEmpty(newExpense.Details))
                newExpense.Details = "-";

            newExpense.Time = DateTime.Now;

            string id = newExpense.Time.ToString("yyyy-MM-dd-HH-mm-ss-" + ((number.ToString().Length < 3) ? ((number.ToString().Length < 2) ? $"00{number}" : $"0{number}") : number.ToString()));

            await App.MainViewModel.FirestoreViewModel.AddExpenseToDatabase(newExpense, id);
        }

        public async Task AddNewExpense(Expense newExpense, int number = 0) => await this.addNewExpense(newExpense, number);

        private async Task deleteExpenses(List<Expense> expensesToDelete)
        {
            if (expensesToDelete.Count == 0) return;

            foreach (var Expense in expensesToDelete.ToList())
            {
                await App.MainViewModel.FirestoreViewModel.RemoveExpense(Expense);
            }
        }

        public async Task DeleteExpenses(List<Expense> expensesToDelete) => await this.deleteExpenses(expensesToDelete);
    }
}
