namespace GamingStore.Models
{
    public class RelatedItem
    {
        public RelatedItem(int customerNumber, int itemId)
        {
            CustomerNumber = customerNumber;
            ItemId = itemId;
        }

        public int CustomerNumber { get; set; }
        public int ItemId { get; set; }

    }
}
