using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GamingStore.Contracts;
using GamingStore.Models;
using Microsoft.AspNetCore.Http;

namespace GamingStore.ViewModels
{
    public class GetItemsViewModel : ViewModelBase
    {
        public Item[] Items { get; set; }
        public IEnumerable<Category>  Categories { get; set; }
        public IEnumerable<string> Manufactures { get; set; }

        public ItemsFilter ItemsFilter { get; set; }

    }
}
