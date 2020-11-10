using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GamingStore.Contracts
{
    public class ItemsFilter
    {
        //todo: category shouldn't be category?(with enum)
        public string Category { get; set; }

        [DataType(DataType.Text)] public string Manufacturer { get; set; }

        [Display(Name = "Min Price"), DataType(DataType.Currency)]
        public int? PriceMin { get; set; }

        [Display(Name = "Max Price"), DataType(DataType.Currency)]
        public int? PriceMax { get; set; }

        [Display(Name = "Sort By")] public SortByFilter SortBy { get; set; }
        [DataType(DataType.Text)] public string Keywords { get; set; }
    }
}