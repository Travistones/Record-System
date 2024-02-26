using Record_System.BackEnd.Data;
using Record_System.BackEnd.Database;
using Record_System.BackEnd.ObjectPropertyChanged;
using Record_System.BackEnd.Settings;
using Record_System.BackEnd.ViewModels.Orders;
using Record_System.BackEnd.ViewModels.Revenues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Record_System.Pages.Content.Orders.ViewModel
{
    public class OrdersPageViewModel : BindableBase
    {
        public OrdersViewModel OrdersViewModel;

        public OverallTime OverallTime;

        public OrdersPageViewModel()
        {
            OrdersViewModel = App.MainViewModel.OrdersViewModel;

            OverallTime = App.MainViewModel.OverallTimeViewModel.OverallTime;
        }

        private Order newOrder;

        private Order selectedOrder;

        public Order SelectedOrder
        {
            get => selectedOrder;

            set => SetProperty(ref selectedOrder, value);
        }

        public Order NewOrder
        {
            get => newOrder;

            set => SetProperty(ref newOrder, value);
        }

        private bool _canDeleteOrders;

        public bool CanDeleteOrders
        {
            get => _canDeleteOrders;

            set => SetProperty(ref _canDeleteOrders, value);
        }

        public DateTime CurrentDateTime
        {
            get => DateTime.Now;
        }

        public async Task AddToOrders()
        {
            await OrdersViewModel.AddNewOrder(NewOrder);
        }

        public void CreateNewOrder()
        {
            NewOrder = new();
        }

        public async Task DeleteOrders(List<Order> ordersToDelete)
        {
            await OrdersViewModel.DeleteOrders(ordersToDelete);
        }
    }
}
