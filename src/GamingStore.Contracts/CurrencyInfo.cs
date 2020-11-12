using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GamingStore.Contracts
{
    public class CurrencyInfo
    {
        public string Currency { get; set; }
        
        public string Symbol { get; set; }

        [DisplayFormat(DataFormatString = "{0:n}", ApplyFormatInEditMode = true)]
        public double Value { get; set; }

        [DisplayFormat(DataFormatString = "{0:n}", ApplyFormatInEditMode = true)]
        public double Total { get; set; }
    }
}
