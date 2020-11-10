using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GamingStore.Contracts;
using GamingStore.Models;
using Microsoft.AspNetCore.Http;

namespace GamingStore.ViewModels
{
    public class StoresCitiesViewModel : ViewModelBase
    {
        public IEnumerable<Store> Stores { get; set; }

        public string[] CitiesWithStores { get; set; }
        public IEnumerable<Store> OpenStores{ get; set; }

        public string Name { get; set; }
        public string City { get; set; }
        public bool IsOpen { get; set; } 
    }
}
