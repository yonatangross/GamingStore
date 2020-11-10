using System.Collections.Generic;
using GamingStore.Models;

namespace GamingStore.ViewModels.Administration
{
    public class ListStoresViewModel : ViewModelBase
    {
        public IEnumerable<Store> Stores { get; set; }
    }
}
