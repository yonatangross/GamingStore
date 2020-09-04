using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GamingStore.Contracts;

namespace GamingStore.Models
{
    public class Customer
    {
        public int Id { get; set; }
        [StringLength(50), RegularExpression(@"^[A-Z]+[a-zA-Z]*$")]
        public string FirstName { get; set; }
        [StringLength(50), RegularExpression(@"^[A-Z]+[a-zA-Z]*$")]
        public string LastName { get; set; }
        public Address Address { get; set; }
        [Required]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public ICollection<Order> OrderHistory { get; set; }
        public ICollection<Item> ShoppingCart { get; set; }

    }
}
