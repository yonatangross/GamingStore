using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GamingStore.Contracts;
using GamingStore.Models;
using Microsoft.AspNetCore.Http;

namespace GamingStore.ViewModels
{
    public class ListItemsViewModel : ViewModelBase
    {
        public IEnumerable<Item> Items { get; set; }
    }
}
