using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GamingStore.Models;

namespace GamingStore.ViewModels
{
    public class EditOrdersViewModel
    {
        public List<Customer> Customers { get; set; }

        public Order Order { get; set; }
    }
}
