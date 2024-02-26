using Record_System.BackEnd.Data;
using Record_System.BackEnd.ObjectPropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Record_System.BackEnd.ViewModels.Credits
{
    public class CreditsViewModel : BindableBase
    {
        public CreditsViewModel()
        {
            Credits = new();
        }

        private ObservableCollection<Credit> _credits;

        public ObservableCollection<Credit> Credits
        {
            get => _credits;

            set => SetProperty(ref _credits, value);
        }

        private async Task addNewCredit(Credit newCredit)
        {
            if (string.IsNullOrEmpty(newCredit.Details))
                newCredit.Details = "-";

            newCredit.Time = DateTime.Now;

            newCredit.GradeAPricePerVolume = App.MainViewModel.SettingsViewModel.ApplicationSettings.GradeA.Price;

            newCredit.GradeBPricePerVolume = App.MainViewModel.SettingsViewModel.ApplicationSettings.GradeB.Price;

            newCredit.GradeCPricePerVolume = App.MainViewModel.SettingsViewModel.ApplicationSettings.GradeC.Price;

            await App.MainViewModel.FirestoreViewModel.AddCreditToDatabase(newCredit);
        }

        public async Task AddNewCredit(Credit newCredit) => await this.addNewCredit(newCredit);

        private async Task deleteCredits(List<Credit> creditsToDelete)
        {
            if (Credits.Count == 0) return;

            foreach (var credit in creditsToDelete.ToList())
            {
                await App.MainViewModel.FirestoreViewModel.RemoveCredit(credit);
            }
        }

        public async Task DeleteCredits(List<Credit> creditsToDelete) => await deleteCredits(creditsToDelete);

    }
}
