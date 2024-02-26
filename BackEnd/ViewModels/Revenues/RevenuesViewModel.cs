using Record_System.BackEnd.Data;
using Record_System.BackEnd.ObjectPropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Record_System.BackEnd.ViewModels.Revenues
{
    public class RevenuesViewModel : BindableBase
    {
        public RevenuesViewModel()
        {
            Revenues = new();
        }

        private ObservableCollection<Revenue> _revenues;

        public ObservableCollection<Revenue> Revenues
        {
            get => _revenues;

            private set => _revenues = value;
        }

        private async Task addNewRevenue(Revenue newRevenue)
        {
            if (string.IsNullOrEmpty(newRevenue.Account))
                newRevenue.Account = "-";

            if (string.IsNullOrEmpty(newRevenue.Details))
                newRevenue.Details = "-";

            newRevenue.Time = DateTime.Now;

            newRevenue.GradeAPricePerVolume = App.MainViewModel.SettingsViewModel.ApplicationSettings.GradeA.Price;

            newRevenue.GradeBPricePerVolume = App.MainViewModel.SettingsViewModel.ApplicationSettings.GradeB.Price;

            newRevenue.GradeCPricePerVolume = App.MainViewModel.SettingsViewModel.ApplicationSettings.GradeC.Price;

            await App.MainViewModel.FirestoreViewModel.AddRevenueToDatabase(newRevenue);
        }

        public async Task AddNewRevenue(Revenue newRevenue) => await this.addNewRevenue(newRevenue);

        private async Task deleteRevenues(List<Revenue> revenues)
        {
            if (revenues.Count == 0) return;

            foreach(var revenue in revenues.ToList())
            {
                await App.MainViewModel.FirestoreViewModel.RemoveRevenue(revenue);
            }
        }

        public async Task DeleteRevenues(List<Revenue> revenues) => await this.deleteRevenues(revenues);
    }
}
