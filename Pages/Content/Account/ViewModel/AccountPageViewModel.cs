using Record_System.BackEnd.ObjectPropertyChanged;
using Record_System.BackEnd.ViewModels.Account;
using Record_System.BackEnd.ViewModels.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Record_System.Pages.Content.Account.ViewModel
{
    public class AccountPageViewModel : BindableBase
    {
        public SettingsViewModel SettingsViewModel;

        public AccountViewModel AccountViewModel;

        public AccountPageViewModel()
        {
            SettingsViewModel = App.MainViewModel.SettingsViewModel;

            AccountViewModel = App.MainViewModel.AccountViewModel;
        }

        private byte _numberOfUpdatingEvents = 0;

        public byte NumberOfUpdatingEvents
        {
            get => _numberOfUpdatingEvents;

            set
            {
                SetProperty(ref _numberOfUpdatingEvents, value);

                RaisePropertyChanged(nameof(IsUpdatingSettings));
            }
        }

        public bool IsUpdatingSettings
        {
            get => NumberOfUpdatingEvents > 0;
        }
    }
}
