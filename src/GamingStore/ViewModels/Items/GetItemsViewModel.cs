using System.Collections.Generic;
using GamingStore.Contracts;
using GamingStore.Models;

namespace GamingStore.ViewModels.Items
{
    public class GetItemsViewModel : ViewModelBase
    {
        public Item[] Items { get; set; }
        public IEnumerable<Category>  Categories { get; set; }
        public IEnumerable<string> Manufactures { get; set; }

        public ItemsFilter ItemsFilter { get; set; }

    }
}
