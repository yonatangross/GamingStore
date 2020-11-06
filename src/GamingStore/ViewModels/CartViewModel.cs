using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GamingStore.Models;

namespace GamingStore.ViewModels
{
    public class CartViewModel
    {
        public List<Cart> Carts { get; set; }

        public Payment Payment { get; set; }
    }
}
