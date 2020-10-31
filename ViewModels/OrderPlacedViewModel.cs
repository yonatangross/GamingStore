using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GamingStore.Contracts;
using GamingStore.Models;

namespace GamingStore.ViewModels
{
    public class OrderPlacedViewModel
    {
        public Customer Customer { get; set; }

        public List<Cart> Cart { get; set; }

        public Address ShippingAddress { get; set; }

    }


}
