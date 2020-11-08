using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamingStore.Models
{
    public class ItemsBundle
    {
        public ItemsBundle(int firstItemId, int secondItemId)
        {
            FirstItemId = firstItemId;
            SecondItemId = secondItemId;
        }

        public int FirstItemId { get; set; }
        public int SecondItemId { get; set; }
    }
}
