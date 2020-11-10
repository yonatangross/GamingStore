using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GamingStore.Models;

namespace GamingStore.ViewModels
{
    public class ItemViewModel : ViewModelBase
    {
        public Item Item { get; set; }
    }
}
