using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GamingStore.Contracts
{
    public class BarChartFormat
    {
        [DataType(DataType.Date)]
        public string Date { get; set; }

        [DataType(DataType.Currency)]
        public double Value { get; set; }
    }
}
