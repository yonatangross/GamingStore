using System.Collections.Generic;
using GamingStore.Models;

namespace GamingStore.ViewModels
{
    public class OrderIndexViewModel : ViewModelBase
    {
        public List<Order> OrderList { get; set; }
    }
}
