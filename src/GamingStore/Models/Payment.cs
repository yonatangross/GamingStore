using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GamingStore.Contracts;

namespace GamingStore.Models
{
    public class Payment
    {
        public Payment()
        {
            Id = Guid.NewGuid().ToString();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        public string OrderForeignKey { get; set; }

        [Required]
        [Display(Name = "Items Cost")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public double ItemsCost { get; set; }

        [Display(Name = "Shipping Cost")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public int ShippingCost { get; set; } = 0;

        [Required]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public double Total { get; set; }
        
        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        [Display(Name = "Refund Amount")]
        public double RefundAmount { get; set; }

        public string Notes { get; set; }

        public bool Paid { get; set; } = true;
    }
}