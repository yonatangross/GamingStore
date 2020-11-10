using System.Collections.Generic;
using GamingStore.Models;

namespace GamingStore.ViewModels.Administration
{
    public class ListItemsViewModel : ViewModelBase
    {
        public IEnumerable<Item> Items { get; set; }
    }
}
