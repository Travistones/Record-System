using Record_System.BackEnd.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Record_System.BackEnd.ViewModels.Orders
{
    public class OrdersViewModel
    {
        public OrdersViewModel()
        {
            Orders = new();
            //{
            //    new Order()
            //    {
            //        Account = "Some Account",
            //        IsComplete = false,
            //        Volume = 25,
            //        Grade = App.MainViewModel.SettingsViewModel.ApplicationSettings.GradeA,
            //        Price = 25000,
            //        Discount = 0,
            //        NetReceived = 25000,
            //        Details = "",
            //        ClosingTime = DateTimeOffset.Now,
            //        Time = DateTime.Now
            //    }
            //};
        }

        private ObservableCollection<Order> _orders;

        public ObservableCollection<Order> Orders
        {
            get => _orders;

            private set => _orders = value;
        }

        private async Task addNewOrder(Order newOrder)
        {
            if (string.IsNullOrEmpty(newOrder.Details))
                newOrder.Details = "-";

            newOrder.Time = DateTime.Now;

            newOrder.GradeAPricePerVolume = App.MainViewModel.SettingsViewModel.ApplicationSettings.GradeA.Price;

            newOrder.GradeBPricePerVolume = App.MainViewModel.SettingsViewModel.ApplicationSettings.GradeB.Price;

            newOrder.GradeCPricePerVolume = App.MainViewModel.SettingsViewModel.ApplicationSettings.GradeC.Price;

            await App.MainViewModel.FirestoreViewModel.AddOrderToDatabase(newOrder);
        }

        public async Task AddNewOrder(Order newOrder) => await this.addNewOrder(newOrder);

        private async Task deleteOrders(List<Order> ordersToDelete)
        {
            if (Orders.Count == 0) return;

            foreach (var order in ordersToDelete.ToList())
            {
                await App.MainViewModel.FirestoreViewModel.RemoveOrder(order);
            }
        }

        public async Task DeleteOrders(List<Order> ordersToDelete) => await deleteOrders(ordersToDelete);
    }
}
