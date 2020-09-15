using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading;
using GamingStore.Contracts;
using Microsoft.AspNetCore.Identity;

namespace GamingStore.Models
{
    public class Customer :IdentityUser
    {
        public Customer()
        {
            OrderHistory = new List<Order>();
            ShoppingCart = new List<Item>();
        }
        [Required, DataType(DataType.Text), StringLength(50), RegularExpression(@"^[A-Z]+[a-zA-Z]*$")]
        public string FirstName { get; set; }

        [Required, DataType(DataType.Text), StringLength(50), RegularExpression(@"^[A-Z]+[a-zA-Z]*$")]
        public string LastName { get; set; }

        public Address Address { get; set; }

        public ICollection<Order> OrderHistory { get; set; }
        public ICollection<Item> ShoppingCart { get; set; }
    }
}