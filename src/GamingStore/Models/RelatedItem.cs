namespace GamingStore.Models
{
    public class RelatedItem
    {
        public RelatedItem(int customerIntId, int itemId)
        {
            CustomerIntId = customerIntId;
            ItemId = itemId;
        }

        public int CustomerIntId { get; set; }
        public int ItemId { get; set; }

    }
}
