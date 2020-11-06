using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamingStore.Contracts
{
    public class ItemsFilter
    {
        public string Category { get; set; }

        public string Manufacture { get; set; }

        public string PriceRange { get; set; }
    }
}
