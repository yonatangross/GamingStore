using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GamingStore.Contracts;
using GamingStore.Models;
using GamingStore.Models.Relationships;

namespace GamingStore.ViewModels
{
    public class OrderDetailsViewModel : ViewModelBase
    {
        public DateTime OrderDate { get; set; }

        public string OrderId { get; set; }

        public string OrderState { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }

        public Address ShippingAddress { get; set; }

        public Payment Payment { get; set; }
    }
}
