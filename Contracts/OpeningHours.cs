using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GamingStore.Contracts
{
    [NotMapped]
    public class OpeningHours
    {
        public DayOfWeek DayOfWeek { get; set; }
        public DateTime OpeningTime { get; set; }
        public DateTime ClosingTime { get; set; }
    }
}