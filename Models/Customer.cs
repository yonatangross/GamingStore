using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GamingStore.Contracts;

namespace GamingStore.Models
{
    public class Customer
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required, DataType(DataType.Text), StringLength(50), RegularExpression(@"^[A-Z]+[a-zA-Z]*$")]
        public string FirstName { get; set; }
        [Required, DataType(DataType.Text), StringLength(50), RegularExpression(@"^[A-Z]+[a-zA-Z]*$")]
        public string LastName { get; set; }
        public Address Address { get; set; }
        [Required, DataType(DataType.EmailAddress),]
        public string Email { get; set; }
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
        public ICollection<Order> OrderHistory { get; set; }
        public ICollection<Item> ShoppingCart { get; set; }

    }
}
