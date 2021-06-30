using GamingStore.Contracts;
using GamingStore.Models;

namespace GamingStore.ViewModels.Orders
{
    public class OrderPlacedViewModel : ViewModelBase
    {
        public Customer Customer { get; set; }

        public int ItemsCount { get; set; }

        public Address ShippingAddress { get; set; }

        public string OrderId { get; set; }
    }
}
