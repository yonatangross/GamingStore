using System.Collections.Generic;
using GamingStore.Models;

namespace GamingStore.ViewModels.Orders
{
    public class EditOrdersViewModel : ViewModelBase
    {
        public List<Customer> Customers { get; set; }

        public Order Order { get; set; }
    }
}
