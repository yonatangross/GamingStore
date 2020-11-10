using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GamingStore.Contracts;
using GamingStore.Models;
using GamingStore.Models.Relationships;

namespace GamingStore.ViewModels
{
    public class CreateOrderViewModel : ViewModelBase
    {
        public int OrderId { get; set; }

        [Required] 
        public string CustomerId { get; set; }
        
        public Customer Customer { get; set; }
        
        [Required]
        public int StoreId { get; set; }
        
        public DateTime OrderDate { get; set; }
        
        public OrderState State { get; set; }

        public Address ShippingAddress { get; set; }
        
        public ShippingMethod ShippingMethod { get; set; }

        public CreditCard CreditCard { get; set; }

        public List<Cart> Cart { get; set; }

        [Required]
        public Payment Payment { get; set; }
    }
}