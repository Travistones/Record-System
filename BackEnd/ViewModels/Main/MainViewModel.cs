using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Record_System.ApplicationWindows.Main.Window;
using Record_System.BackEnd.ViewModels.Account;
using Record_System.BackEnd.ViewModels.Credits;
using Record_System.BackEnd.ViewModels.Employees;
using Record_System.BackEnd.ViewModels.Expenses;
using Record_System.BackEnd.ViewModels.FirebaseStorage;
using Record_System.BackEnd.ViewModels.Firestore;
using Record_System.BackEnd.ViewModels.Orders;
using Record_System.BackEnd.ViewModels.Revenues;
using Record_System.BackEnd.ViewModels.Settings;
using Windows.Storage.Pickers;
using Windows.Storage;
using WinRT.Interop;
using Record_System.BackEnd.ViewModels.OverallTime;
using Record_System.BackEnd.ObjectPropertyChanged;
using Microsoft.UI.Dispatching;
using Record_System.BackEnd.ViewModels.Graphs;

namespace Record_System.BackEnd.ViewModels.Main
{
    public class MainViewModel : BindableBase
    {
        public MainViewModel()
        {
            
        }

        private SettingsViewModel _settingsViewModel;

        private AccountViewModel _accountViewModel;

        private RevenuesViewModel _revenuesViewModel;

        private ExpensesViewModel _expensesViewModel;

        private OrdersViewModel _ordersViewModel;

        private CreditsViewModel _creditsViewModel;

        private EmployeesViewModel _employeesViewModel;

        private FirestoreViewModel _firestoreViewModel;

        private FirebaseStorageViewModel _firebaseStorageViewModel;

        private OverallTimeViewModel _overallTimeViewModel;

        private GraphsViewModel _graphsViewModel;

        private Timer _timer;

        private DispatcherQueue _dispatcherQueue;

        private int _numberOfUploadTasks;

        public DispatcherQueue DispatcherQueue
        {
            get => _dispatcherQueue;

            set => _dispatcherQueue = value;
        }

        public OverallTimeViewModel OverallTimeViewModel
        {
            get => _overallTimeViewModel;

            private set => _overallTimeViewModel = value;
        }

        public FirestoreViewModel FirestoreViewModel
        {
            get => _firestoreViewModel;

            private set => _firestoreViewModel = value;
        }

        public FirebaseStorageViewModel FirebaseStorageViewModel
        {
            get => _firebaseStorageViewModel;

            set => _firebaseStorageViewModel = value;
        }

        public OrdersViewModel OrdersViewModel
        {
            get => _ordersViewModel;

            private set => _ordersViewModel = value;
        }

        public RevenuesViewModel RevenuesViewModel
        {
            get => _revenuesViewModel;

            private set => _revenuesViewModel = value;
        }

        public ExpensesViewModel ExpensesViewModel
        {
            get => _expensesViewModel;

            private set => _expensesViewModel = value;
        }

        public SettingsViewModel SettingsViewModel
        {
            get => _settingsViewModel;

            private set => _settingsViewModel = value;
        }

        public AccountViewModel AccountViewModel
        {
            get => _accountViewModel;

            private set => _accountViewModel = value;
        }

        public CreditsViewModel CreditsViewModel
        {
            get => _creditsViewModel;

            private set => _creditsViewModel = value;
        }

        public EmployeesViewModel EmployeesViewModel
        {
            get => _employeesViewModel;

            private set => _employeesViewModel = value;
        }

        public GraphsViewModel GraphsViewModel
        {
            get => _graphsViewModel;

            private set => _graphsViewModel = value;
        }

        private void initializeViewModels()
        {
            OverallTimeViewModel = new();

            FirebaseStorageViewModel = new();

            FirestoreViewModel = new();

            AccountViewModel = new();

            SettingsViewModel = new();

            RevenuesViewModel = new();

            ExpensesViewModel = new();

            OrdersViewModel = new();

            CreditsViewModel = new();

            EmployeesViewModel = new();

            GraphsViewModel = new();
        }

        public void InitializeViewModels() => initializeViewModels();

        public bool _hasInternetConnection;

        public bool HasInternetConnection
        {
            get => _hasInternetConnection;

            set => SetProperty(ref _hasInternetConnection, value);
        }
        
        private void _initTimer()
        {
            _timer = new();
            _timer.Interval = 1000;
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();
        }

        public void InitTimer() => _initTimer();

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            CheckInternetConnection();
        }

        public void CheckInternetConnection()
        {

            DispatcherQueue.TryEnqueue
            (
                Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal,
                async () =>
                {
                    try
                    {
                        using (var client = new HttpClient())
                        using (_ = await client.GetAsync("https://www.google.com"))
                        {
                            HasInternetConnection = true;
                        }
                    }
                    catch
                    {
                        HasInternetConnection = false;
                    }
                }    
            );
        }

        public int NumberOfUploadTasks
        {
            get => _numberOfUploadTasks;

            set
            {
                DispatcherQueue.TryEnqueue
                (
                    Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal,
                    () =>
                    {
                        if (value < 0)
                            return;

                        SetProperty(ref _numberOfUploadTasks, value);

                        RaisePropertyChanged(nameof(AreTasksPresent));
                    }
                );
            }
        }

        public void IncrementUploadTasks() => NumberOfUploadTasks = (NumberOfUploadTasks + 1);

        public void DecrementUploadTasks()
        {
            if (NumberOfUploadTasks <= 0)
                return;

            NumberOfUploadTasks = (NumberOfUploadTasks - 1);
        }

        public bool AreTasksPresent
        {
            get => NumberOfUploadTasks > 0;
        }

        public async Task<StorageFile> GetLocalImage()
        {
            var openPicker = new Windows.Storage.Pickers.FileOpenPicker();

            var hWnd = WindowNative.GetWindowHandle(MainWindow.MainWindowObject);

            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");

            var file = await openPicker.PickSingleFileAsync();

            return file;
        }

        public static async Task UpdateTasks(List<Task> updateTasks)
        {
            foreach (var updateTask in updateTasks)
            {
                await updateTask;
            }
        }

        public static void RunTaskWithInternet(Task taskToRun)
        {
            _= Task.Run( async () =>
            {
                while (true)
                {
                    if (!App.MainViewModel.HasInternetConnection)
                        continue;
                    try
                    {
                        await taskToRun;
                    }
                    catch
                    {
                        continue;
                    }

                    break;
                }
            }
            );
            
        }

        public static async Task<object> RunTaskWithInternet(Task<object> taskToRun)
        {
            object returnObject = null;

            while (true)
            {
                if (!App.MainViewModel.HasInternetConnection)
                    continue;
                try
                {
                    returnObject = await taskToRun;
                }
                catch
                {
                    continue;
                }

                if (returnObject == null)
                    continue;

                break;
            }

            return returnObject;
        }

    }
}
