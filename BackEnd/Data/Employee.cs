using Google.Cloud.Firestore;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Record_System.BackEnd.ObjectPropertyChanged;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.AppBroadcasting;
using Windows.Web.AtomPub;

namespace Record_System.BackEnd.Data
{
    public class Employee : BindableBase
    {
        private string _fullName;
        private double _salaryPerMonth = double.NaN;
        private DateTimeOffset? _dateJoined;
        private DateTimeOffset? _birthDate;
        private string _email;
        private string _location;
        private string _phoneNumber;
        private ImageSource _profilePictureDownloadUrl = null;
        private DocumentReference _documentReference;
        //private DocumentReference _userDocumentReference;
        private bool _canAdd;
        //private bool _canModifyEmployees;
        //private bool _canModifyPrices;
        //private bool _canModifyCompany;

        public void AssignEmployeeFromDictionary(Dictionary<string, object> employeeDictionary)
        {
            FullName = (string)employeeDictionary[nameof(FullName)];
            SalaryPerMonth = (double)employeeDictionary[nameof(SalaryPerMonth)];
            DateJoined = DateTimeOffset.ParseExact((string)employeeDictionary[nameof(DateJoined)], "MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
            BirthDate = DateTimeOffset.ParseExact((string)employeeDictionary[nameof(BirthDate)], "MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
            //BirthDate = DateTime.Parse((string)employeeDictionary[nameof(BirthDate)]);
            Email = (string)employeeDictionary[nameof(Email)];
            Location = (string)employeeDictionary[nameof(Location)];
            PhoneNumber = (string)employeeDictionary[nameof(PhoneNumber)];

            if((string)employeeDictionary[nameof(ProfilePictureDownloadUrl)] != null)
                ProfilePictureDownloadUrl = new BitmapImage(new Uri((string)employeeDictionary[nameof(ProfilePictureDownloadUrl)]));

            //UserDocumentReference = (DocumentReference)employeeDictionary[nameof(UserDocumentReference)];
            //CanModifyCompany = (bool)employeeDictionary[nameof(CanModifyCompany)];
            //CanModifyEmployees = (bool)employeeDictionary[nameof(CanModifyEmployees)];
            //CanModifyPrices = (bool)employeeDictionary[nameof(CanModifyPrices)];
        }

        //public bool CanModifyEmployees
        //{
        //    get => _canModifyEmployees;

        //    set => SetProperty(ref _canModifyEmployees, value);
        //}

        //public bool CanModifyCompany
        //{
        //    get => _canModifyCompany;

        //    set => SetProperty(ref _canModifyCompany, value);
        //}

        //public bool CanModifyPrices
        //{
        //    get => _canModifyPrices;

        //    set => SetProperty(ref _canModifyPrices, value);
        //}

        public bool CanAdd
        {
            get => _canAdd;

            set => SetProperty(ref _canAdd, value);
        }

        public DocumentReference DocumentReference
        {
            get => _documentReference;

            set => _documentReference = value;
        }

        //public DocumentReference UserDocumentReference
        //{
        //    get => _userDocumentReference;

        //    set
        //    {
        //        _userDocumentReference = value;
        //        RaisePropertyChanged(nameof(IsUser));
        //    }
        //}

        //public bool IsUser
        //{
        //    get => UserDocumentReference != null;
        //}

        public string FullName
        {
            get => _fullName;

            set
            {
                SetProperty(ref _fullName, value);

                updateCanAdd();
            }
        }

        public double SalaryPerMonth
        {
            get => _salaryPerMonth;

            set
            {
                SetProperty(ref _salaryPerMonth, value);

                updateCanAdd();
            }
        }

        public DateTimeOffset? DateJoined
        {
            get => _dateJoined;

            set
            {
                SetProperty(ref _dateJoined, value);

                updateCanAdd();
            }
        }

        public DateTimeOffset? BirthDate
        {
            get => _birthDate;

            set 
            {
                SetProperty(ref _birthDate, value);

                updateCanAdd();
            }
        }

        public string Email
        {
            get => _email;

            set
            {
                SetProperty(ref _email, value);

                updateCanAdd();
            }
        }

        public string Location
        {
            get => _location;

            set
            {
                SetProperty(ref _location, value);

                updateCanAdd();
            }
        }

        public string PhoneNumber
        {
            get => _phoneNumber;

            set
            {
                SetProperty(ref _phoneNumber, value);

                updateCanAdd();
            }
        }

        public ImageSource ProfilePictureDownloadUrl
        {
            get => _profilePictureDownloadUrl;

            set
            {
                SetProperty(ref _profilePictureDownloadUrl, value);

                RaisePropertyChanged(nameof(HasProfilePicture));

                RaisePropertyChanged(nameof(HasNoProfilePicture));
            }
        }

        public bool HasProfilePicture
        {
            get => ProfilePictureDownloadUrl != null;
        }

        public bool HasNoProfilePicture
        {
            get => ProfilePictureDownloadUrl == null;
        }

        private void updateCanAdd()
        {
            CanAdd =
                !double.IsNaN(SalaryPerMonth) &&
                !string.IsNullOrEmpty(FullName) &&
                (DateJoined != null) &&
                !string.IsNullOrEmpty(Email) &&
                !string.IsNullOrEmpty(Location) &&
                !string.IsNullOrEmpty(PhoneNumber) &&
                (BirthDate != null);
        }

        public async Task<Task> UpdateEmployee(Employee employee)
        {
            string profilePictureDownloadUrl = null;

            if (this.ProfilePictureDownloadUrl != employee.ProfilePictureDownloadUrl)
            {
                profilePictureDownloadUrl = await App.MainViewModel.FirebaseStorageViewModel.AddImage(((BitmapImage)employee.ProfilePictureDownloadUrl).UriSource.AbsolutePath, employee.Email);
            }

            Dictionary<string, object> updateFields = new();

            if (profilePictureDownloadUrl != null)
                updateFields.Add(nameof(ProfilePictureDownloadUrl), profilePictureDownloadUrl);

            if (this.FullName != employee.FullName)
                updateFields.Add(nameof(FullName), employee.FullName);

            if (this.Location != employee.Location)
                updateFields.Add(nameof(Location), employee.Location);

            if (this.Email != employee.Email)
                updateFields.Add(nameof(Email), employee.Email);

            if (this.PhoneNumber != employee.PhoneNumber)
                updateFields.Add(nameof(PhoneNumber), employee.PhoneNumber);

            if (this.SalaryPerMonth != employee.SalaryPerMonth)
                updateFields.Add(nameof(SalaryPerMonth), employee.SalaryPerMonth);

            if (this.BirthDate != employee.BirthDate)
                updateFields.Add(nameof(BirthDate), employee.BirthDate?.DateTime.ToString("MM/dd/yyyy hh:mm:ss tt"));

            if (this.DateJoined != employee.DateJoined)
                updateFields.Add(nameof(DateJoined), employee.DateJoined?.DateTime.ToString("MM/dd/yyyy hh:mm:ss tt"));

            if (updateFields.Count > 0)
                return this.DocumentReference.UpdateAsync(updateFields);
            else
                return null;
        }

        //public Task UpdateUserPermissions(bool canModifyCompany, bool canModifyEmployees, bool canModifyPrices)
        //{
        //    var updateDictionary = new Dictionary<string, object>();

        //    if(this.CanModifyCompany != canModifyCompany)
        //        updateDictionary.Add(nameof(CanModifyCompany), canModifyCompany);

        //    if (this.CanModifyEmployees != canModifyEmployees)
        //        updateDictionary.Add(nameof(CanModifyEmployees), CanModifyEmployees);

        //    if (this.CanModifyPrices != canModifyPrices)
        //        updateDictionary.Add(nameof(CanModifyPrices), canModifyPrices);

        //    if (updateDictionary.Count > 0)
        //        return UserDocumentReference.UpdateAsync(updateDictionary);
        //    else
        //        return null;
        //}

        //public Task ResetPasswordAndUserName()
        //{
        //    return UserDocumentReference.UpdateAsync
        //    (
        //        new Dictionary<string, object>
        //        {
        //            { nameof(User.Account.UserName), Email },
        //            { nameof(User.Account.Password), Email }
        //        }
        //    );
        //}
    }
}
