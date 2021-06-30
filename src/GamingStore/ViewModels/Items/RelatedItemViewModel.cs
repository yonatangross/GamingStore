using System.Collections.Generic;
using GamingStore.Models;

namespace GamingStore.ViewModels.Items
{
    public class RelatedItemViewModel : ViewModelBase
    {
        public RelatedItemViewModel()
        {

        }
        public int RelatedItemId { get; set; }
        
        public List<RelatedItem> RelatedItem { get; set; }

        public int ItemId { get; set; }
        public List<Item> Item { get; set; }
    }
}
