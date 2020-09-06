using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamingStore.Models
{
    public class StoreItem
    {
        public int StoreId { get; set; }
        public int ItemId{ get; set; }

        public Store Store { get; set; }
        public Item Item { get; set; }
        public uint ItemsCount { get; set; }
    }
}
