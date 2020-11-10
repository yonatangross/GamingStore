using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GamingStore.Models;

namespace GamingStore.ViewModels
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
