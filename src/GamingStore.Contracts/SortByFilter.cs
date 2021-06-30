using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GamingStore.Contracts
{
    public enum SortByFilter
    {
        [Display(Name = "Newest Arrivals")]
        NewestArrivals,

        [Display(Name = "Price: Low to High")]
        PriceLowToHigh,

        [Display(Name = "Price: High to Low")]
        PriceHighToLow
    }
}
