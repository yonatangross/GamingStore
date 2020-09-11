namespace GamingStore.Models.Relationships
{
    public class OrderItem
    {
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public int ItemId { get; set; }
        public Item Item { get; set; }
        public uint ItemsCount { get; set; }
    }
}