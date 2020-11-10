using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GamingStore.Contracts;
using GamingStore.Models;
using GamingStore.Models.Relationships;

namespace GamingStore.ViewModels
{
    public class OrderDetailsViewModel : ViewModelBase
    {
        public Order Order { get; set; }
    }
}
