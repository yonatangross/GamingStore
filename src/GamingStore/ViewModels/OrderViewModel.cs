using System.Collections.Generic;
using GamingStore.Models;

namespace GamingStore.ViewModels
{
    public class OrderViewModel : ViewModelBase
    {
        public List<Order> OrderList { get; set; }
    }
}
