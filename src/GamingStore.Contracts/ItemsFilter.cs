using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GamingStore.Contracts
{
    public class ItemsFilter
    {
        public string Category { get; set; }

        public string Manufacturer { get; set; }

        [Range(1, 99999)]
        public int? PriceMin { get; set; }

        [Range(1, 99999)]
        public int? PriceMax { get; set; }

        public SortByFilter SortBy { get; set; }

        public string Keywords { get; set; }
    }
}