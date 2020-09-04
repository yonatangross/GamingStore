using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using GamingStore.Contracts;

namespace GamingStore.Models
{
    public class Order
    {
        public int Id { get; set; }
        [NotMapped]//todo: map to db.
        public Dictionary<Item, uint> Items { get; set; }

        public Customer Customer { get; set; }
        public DateTime OrderDate { get; set; }
        public Store Store { get; set; }
        public OrderState State { get; set; }

        public Payment Payment { get; set; }
        //todo: add coupons maybe
    }
}
