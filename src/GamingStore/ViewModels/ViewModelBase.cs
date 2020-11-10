using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GamingStore.Models;

namespace GamingStore.ViewModels
{
    public abstract class ViewModelBase
    {
        public int? ItemsInCart { get; set; } = null;


    }
}
