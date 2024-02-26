using Record_System.BackEnd.Data;
using Record_System.BackEnd.Database;
using Record_System.BackEnd.ObjectPropertyChanged;
using Record_System.BackEnd.ViewModels.Credits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Record_System.Pages.Content.Credits.ViewModel
{
    public class CreditsPageViewModel : BindableBase
    {
        public CreditsViewModel CreditsViewModel;

        public OverallTime OverallTime;

        public CreditsPageViewModel()
        {
            CreditsViewModel = App.MainViewModel.CreditsViewModel;

            OverallTime = App.MainViewModel.OverallTimeViewModel.OverallTime;
        }

        private Credit _newCredit;

        private Credit selectedCredit;

        public Credit SelectedCredit
        {
            get => selectedCredit;

            set => SetProperty(ref selectedCredit, value);
        }

        public Credit NewCredit
        {
            get => _newCredit;

            set => SetProperty(ref _newCredit, value);
        }

        private bool _canDeleteCredits;

        public bool CanDeleteCredits
        {
            get => _canDeleteCredits;

            set => SetProperty(ref _canDeleteCredits, value);
        }

        public void CreateNewCredit()
        {
            NewCredit = new();
        }

        public async Task DeleteCredits(List<Credit> creditsToDelete) => await CreditsViewModel.DeleteCredits(creditsToDelete);

        public async Task AddToCredits()
        {
            await CreditsViewModel.AddNewCredit(NewCredit);
        }
    }
}
