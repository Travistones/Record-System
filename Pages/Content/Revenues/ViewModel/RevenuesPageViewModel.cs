using Record_System.BackEnd.Data;
using Record_System.BackEnd.Database;
using Record_System.BackEnd.ObjectPropertyChanged;
using Record_System.BackEnd.Settings;
using Record_System.BackEnd.ViewModels.Revenues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Record_System.Pages.Content.Revenues.ViewModel
{
    public class RevenuesPageViewModel : BindableBase
    {
        private Revenue _newRevenue;

        public RevenuesPageViewModel()
        {
            RevenuesViewModel = App.MainViewModel.RevenuesViewModel;

            OverallTime = App.MainViewModel.OverallTimeViewModel.OverallTime;
        }

        public RevenuesViewModel RevenuesViewModel;

        public OverallTime OverallTime;

        public Revenue NewRevenue
        {
            get => _newRevenue;

            private set
            {
                SetProperty(ref _newRevenue, value);
            }
        }

        public void CreateNewRevenue()
        {
            NewRevenue = new();
        }

        public async Task AddToRevenues()
        {
            await RevenuesViewModel.AddNewRevenue(NewRevenue);
        }

        private bool _canDeleteRevenues = false;

        public bool CanDeleteRevenues
        {
            get => _canDeleteRevenues;

            set => SetProperty(ref _canDeleteRevenues, value);
        }

        public async Task DeleteRevenues(List<Revenue> revenuesToDelete)
        {
            await RevenuesViewModel.DeleteRevenues(revenuesToDelete);
        }
    }
}
