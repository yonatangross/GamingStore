using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GamingStore.Models;

namespace GamingStore.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        public List<Item> Items { get; set; }
    }
}
