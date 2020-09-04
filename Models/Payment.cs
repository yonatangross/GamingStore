using GamingStore.Contracts;

namespace GamingStore.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ItemsCost { get; set; }
        public int ShippingCost { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public bool Paid { get; set; }
    }
}