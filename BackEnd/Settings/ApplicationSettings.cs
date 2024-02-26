using Google.Cloud.Firestore;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Record_System.BackEnd.Constants;
using Record_System.BackEnd.Data;
using Record_System.BackEnd.ObjectPropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Core;

namespace Record_System.BackEnd.Settings
{
    public class ApplicationSettings : BindableBase
    {
        private ApplicationDataContainer _settingsApplicationDataContainer;

        public ApplicationSettings() 
        {
            initApplicationSettings();

            _settingsApplicationDataContainer = ApplicationData.Current.LocalSettings.Containers[ApplicationConstants.SETTINGS_APPLICATION_DATA_CONTAINER_KEY];

            GradeA = new() { Quality = Qualities.A };

            GradeB = new() { Quality = Qualities.B };

            GradeC = new() { Quality = Qualities.C };

            Grades = new()
            {
                GradeA,
                GradeB,
                GradeC
            };
        }

        private void initApplicationSettings()
        {
            if (ApplicationData.Current.LocalSettings.Containers.Keys.Contains(ApplicationConstants.SETTINGS_APPLICATION_DATA_CONTAINER_KEY))
                return;

            var settingsContainer = ApplicationData.Current.LocalSettings.CreateContainer(ApplicationConstants.SETTINGS_APPLICATION_DATA_CONTAINER_KEY, ApplicationDataCreateDisposition.Always);

            settingsContainer.Values.Add(nameof(CanUseCurrentPricePerVolume), false);
        }

        public bool CanUseCurrentPricePerVolume
        {
            get => (bool)_settingsApplicationDataContainer.Values[nameof(CanUseCurrentPricePerVolume)] && ArePricesSet;

            set
            {
                _settingsApplicationDataContainer.Values[nameof(CanUseCurrentPricePerVolume)] = value;
                RaisePropertyChanged(nameof(CanUseCurrentPricePerVolume));
            }
        }


        private void _setDatabaseListener()
        {
            //_setCompanySettingsDocumentReferenceListener();
            _setPricingSettingsDocumentReferenceListener();
        }

        public void SetDatabaseListener() => _setDatabaseListener();

        private void _setPricingSettingsDocumentReferenceListener()
        {
            var PricingSettingsDocumentReference = App.MainViewModel.FirestoreViewModel.GetPricingSettingsDocumentReference();

            _pricingSettingsChangeListener = PricingSettingsDocumentReference.Listen
            (
                snapshot =>
                {
                    if (snapshot.Exists)
                    {
                        _ = App.MainViewModel.DispatcherQueue.TryEnqueue
                        (
                            Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () =>
                            {
                                Dictionary<string, object> pricingSettings = snapshot.ToDictionary();

                                ArePricesSet = (bool)pricingSettings[nameof(ArePricesSet)];
                                GradeA.Price = (double)pricingSettings[nameof(GradeA)];
                                GradeC.Price = (double)pricingSettings[nameof(GradeC)];
                                GradeB.Price = (double)pricingSettings[nameof(GradeB)];
                            }
                        );
                    }
                }
            );
        }

        //private void _setCompanySettingsDocumentReferenceListener()
        //{
        //    var CompanySettingsDocumentReference = App.MainViewModel.FirestoreViewModel.GetCompanySettingsDocumentReference();

        //    _companySettingsChangeListener = CompanySettingsDocumentReference.Listen
        //    (
        //        snapshot =>
        //        {
        //            if (snapshot.Exists)
        //            {
        //                _ = App.MainViewModel.DispatcherQueue.TryEnqueue
        //                (
        //                    Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () =>
        //                    {
        //                        Dictionary<string, object> companySettings = snapshot.ToDictionary();
        //                        var bitmapImage = new BitmapImage();

        //                        this.CompanyName = (string)companySettings[nameof(CompanyName)];

        //                        if (string.IsNullOrEmpty((string)companySettings[nameof(CompanyLogoDownloadUrl)]))
        //                            CompanyLogoDownloadUrl = null;
        //                        else
        //                        {
        //                            bitmapImage.UriSource = new Uri(((string)companySettings[nameof(CompanyLogoDownloadUrl)]));
        //                            CompanyLogoDownloadUrl = bitmapImage;
        //                        }
        //                    }
        //                );
        //            } 
        //        }
        //    );
        //}

        //private async Task _stopCompanySettingsChangeListener()
        //{
        //    await _companySettingsChangeListener.StopAsync();
        //}

        private async Task _stopPricingSettingsChangeListener()
        {
            await _pricingSettingsChangeListener.StopAsync();
        }


        //private ImageSource _companyLogoDownloadUrl;

        //private string _companyName;

        private bool _arePricesSet;

        private Grade _gradeA;

        private Grade _gradeB;

        private Grade _gradeC;

        private List<Grade> _grades;

        //private FirestoreChangeListener _companySettingsChangeListener;

        private FirestoreChangeListener _pricingSettingsChangeListener;


        public List<Grade> Grades
        {
            get => _grades;

            private set => SetProperty(ref _grades, value);
        }

        //public ImageSource CompanyLogoDownloadUrl
        //{
        //    get => _companyLogoDownloadUrl;

        //    set
        //    {
        //        SetProperty(ref _companyLogoDownloadUrl, value);

        //        RaisePropertyChanged(nameof(HasCompanyLogo));

        //        RaisePropertyChanged(nameof(HasNoCompanyLogo));
        //    }
        //}

        //public bool HasCompanyLogo
        //{
        //    get => CompanyLogoDownloadUrl != null;
        //}

        //public bool HasNoCompanyLogo
        //{
        //    get => CompanyLogoDownloadUrl == null;
        //}

        //public string CompanyName
        //{
        //    get => _companyName;

        //    set => SetProperty(ref _companyName, value);
        //}

        public Grade GradeA
        {
            get => _gradeA;

            private set => _gradeA = value;
        }

        public Grade GradeB
        {
            get => _gradeB;

            private set => _gradeB = value;
        }

        public Grade GradeC
        {
            get => _gradeC;

            private set => _gradeC = value;
        }

        public bool ArePricesSet
        {
            get => _arePricesSet;

            set => SetProperty(ref _arePricesSet, value);
        }

        public Grade GetGradeFromString(string gradeString)
        {
            foreach(var grade in Grades)
            {
                if (grade.Quality.ToString() == gradeString)
                    return grade;
            }

            return null;
        }
    }
}
