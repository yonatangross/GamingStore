using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GamingStore.Contracts;
using Microsoft.AspNetCore.Identity;

namespace GamingStore.Models
{
    public sealed class Customer : IdentityUser
    {
        public Customer()
        {
            OrderHistory = new List<Order>();
        }

        public int CustomerNumber { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(50)]
        [RegularExpression(@"[a-zA-Z]{2,}$")]
        public string FirstName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(50)]
        [RegularExpression(@"[a-zA-Z]{2,}$")]
        public string LastName { get; set; }

        public Address Address { get; set; }

        public ICollection<Order> OrderHistory { get; set; }
    }
}