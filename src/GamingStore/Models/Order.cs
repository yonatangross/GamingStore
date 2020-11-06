using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading;
using GamingStore.Contracts;
using GamingStore.Models.Relationships;

namespace GamingStore.Models
{
    public class Order
    {

        public Order()
        {
            OrderItems = new List<OrderItem>();
            Id = Guid.NewGuid().ToString();
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }

        [Required] public string CustomerId { get; set; }
        public Customer Customer { get; set; }
        [Required] public int StoreId { get; set; }
        public Store Store { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderState State { get; set; }

        [NotMapped]
        public CreditCard CreditCard { get; set; }

        public Address ShippingAddress { get; set; }

        public ShippingMethod ShippingMethod { get; set; }

        [Required] public string PaymentId { get; set; }

        [Required] public Payment Payment { get; set; }

        //todo: add coupons maybe
        public ICollection<OrderItem> OrderItems { get; set; } // many to many relationship
    }
}