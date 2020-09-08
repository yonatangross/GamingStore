using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GamingStore.Contracts
{
    public class OpeningHours
    {
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan OpeningTime { get; set; }
        public TimeSpan ClosingTime { get; set; }
    }
}