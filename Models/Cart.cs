using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

        public Dictionary<int, uint> ShoppingCart { get; set; } = new Dictionary<int, uint>();

        [NotMapped]
        public Dictionary<Item, uint> ItemsInCart { get; set; }
    }
}
