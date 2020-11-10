using System.ComponentModel.DataAnnotations;

namespace GamingStore.Models
{
    public class RelatedItem
    {
        public RelatedItem(int customerNumber, int itemId)
        {
            CustomerNumber = customerNumber;
            ItemId = itemId;
        }
        [DataType(DataType.Custom)]
        public int CustomerNumber { get; set; }
        [DataType(DataType.Custom)]

        public int ItemId { get; set; }

    }
}
