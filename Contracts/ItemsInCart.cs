using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GamingStore.Models;

namespace GamingStore.Contracts
{
    public class ItemsInCart
    {
        public int ItemId { get; set; }

        public string Title { get; set; }

        public double Price { get; set; }

        public uint Quantity { get; set; }
    }
}
