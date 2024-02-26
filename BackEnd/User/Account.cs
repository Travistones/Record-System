using Google.Cloud.Firestore;
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

namespace Record_System.BackEnd.User
{
    public class Account : BindableBase
    {
        public Account()
        {
            //EmployeeAccount = new();
        }

        private string _password;

        private string _userName;

        //private Employee _employeeAccount;

        //private DocumentReference _employeeDocumentReference;

        //private bool _isOwner;

        private FirestoreChangeListener _accountDocumentReferenceChangeListener;

        //private FirestoreChangeListener _employeeDocumentReferenceChangeListener;

        private DocumentReference _accountDocumentReference;

        //public Employee EmployeeAccount
        //{
        //    get => _employeeAccount;

        //    set => _employeeAccount = value;
        //}

        //public bool IsOwner
        //{
        //    get => _isOwner;

        //    set => SetProperty(ref _isOwner, value);
        //}

        public string Password
        {
            get => _password;

            set => _password = value;
        }
        
        public string UserName
        {
            get => _userName;

            set => SetProperty(ref _userName, value);
        }

        //public DocumentReference EmployeeDocumentReference
        //{
        //    get => _employeeDocumentReference;

        //    set
        //    {
        //        _employeeDocumentReference = value;

        //        _setEmployeeDocumentReferenceChangeListener();
        //    }
        //}

        public DocumentReference AccountDocumentReference
        {
            get => _accountDocumentReference;

            set
            {
                _accountDocumentReference = value;

                _setAccountDocumentReferenceChangeListener();
            }
        }

        public async Task StopAccountDocumentReferenceChangeListener() => await _stopAccountDocumentReferenceChangeListener();

        private async Task _stopAccountDocumentReferenceChangeListener() => await _accountDocumentReferenceChangeListener.StopAsync();

        //private async Task _stopEmployeeDocumentReferenceChangeListener() => await _employeeDocumentReferenceChangeListener.StopAsync();

        //private void _setEmployeeDocumentReferenceChangeListener()
        //{
        //    _accountDocumentReferenceChangeListener = AccountDocumentReference.Listen
        //    (
        //        snapshot =>
        //        {
        //            if (App.MainViewModel.OverallTimeViewModel.OverallTime.DispatcherQueue == null)
        //                return;

        //            if (snapshot.Exists)
        //            {
        //                _ = App.MainViewModel.OverallTimeViewModel.OverallTime.DispatcherQueue.TryEnqueue
        //                (
        //                    Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, 
        //                    () =>
        //                    {
        //                        Dictionary<string, object> employeeDictionary = snapshot.ToDictionary();

        //                        //EmployeeAccount.FullName = (string)employeeDictionary[nameof(Employee.FullName)];

        //                        BitmapImage bitmapImage = new BitmapImage();

        //                        if((string)employeeDictionary[(nameof(Employee.ProfilePictureDownloadUrl))] != null)
        //                            bitmapImage.UriSource = new Uri((string)employeeDictionary[(nameof(Employee.ProfilePictureDownloadUrl))]);

        //                        //EmployeeAccount.ProfilePictureDownloadUrl = bitmapImage;
        //                    }
        //                );
        //            }
        //        }
        //    );
        //}

        private void _setAccountDocumentReferenceChangeListener()
        {
            _accountDocumentReferenceChangeListener = AccountDocumentReference.Listen
            (
                snapshot =>
                {
                    if (App.MainViewModel.OverallTimeViewModel.OverallTime.DispatcherQueue == null)
                        return;

                    if (snapshot.Exists)
                    {
                        _ = App.MainViewModel.OverallTimeViewModel.OverallTime.DispatcherQueue.TryEnqueue
                        (
                            Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, 
                            () =>
                            {
                                Dictionary<string, object> account = snapshot.ToDictionary();

                                //IsOwner = (bool)account[nameof(IsOwner)];
                                Password = (string)account[nameof(Password)];
                                UserName = (string)account[nameof(UserName)];

                                //AccountDocumentReference = App.MainViewModel.FirestoreViewModel.GetUserDocumentReference(UserName);
                                //snapshot = await AccountDocumentReference.GetSnapshotAsync();
                            }
                        );
                    }
                }
            );
        }
    }
}