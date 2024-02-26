using Record_System.BackEnd.ObjectPropertyChanged;
using Record_System.BackEnd.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Record_System.BackEnd.ViewModels.Settings
{
    public class SettingsViewModel : BindableBase
    {
        public SettingsViewModel()
        {
            ApplicationSettings = new();
        }

        private ApplicationSettings _applicationSettings;

        public ApplicationSettings ApplicationSettings
        {
            get => _applicationSettings;

            private set
            {
                SetProperty(ref _applicationSettings, value);
            }
        }
    }
}
