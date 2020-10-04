using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GamingStore.Models
{
    public class Cart
    {
        public Cart(string customerId)
        {
            CustomerId = customerId;
        }

        [Key]
        public string CustomerId { get; set; }

        public Dictionary<int,uint> ShoppingCart { get; set; }
    }
}
