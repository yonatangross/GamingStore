using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GamingStore.Contracts;
using GamingStore.Models;
using Microsoft.AspNetCore.Http;

namespace GamingStore.ViewModels
{
    public class StoresCitiesViewModel
    {
        public IEnumerable<Store> Stores { get; set; }

        public IEnumerable<string> CitiesWithStores { get; set; }
        public IEnumerable<Store> OpenStores{ get; set; }

    }
}
