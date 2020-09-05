using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GamingStore.Contracts;

namespace GamingStore.Models
{
    public class Payment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public int OrderId { get; set; }
        public int ItemsCost { get; set; }
        public int ShippingCost { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public bool Paid { get; set; }
    }
}