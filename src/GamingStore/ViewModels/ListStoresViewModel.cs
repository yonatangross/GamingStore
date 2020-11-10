using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GamingStore.Contracts;
using GamingStore.Models;
using Microsoft.AspNetCore.Http;

namespace GamingStore.ViewModels
{
    public class ListStoresViewModel : ViewModelBase
    {
        public IEnumerable<Store> Stores { get; set; }
    }
}
