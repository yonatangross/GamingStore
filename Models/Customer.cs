using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading;
using GamingStore.Contracts;
using Microsoft.AspNetCore.Identity;
using Org.BouncyCastle.Crypto.Tls;

namespace GamingStore.Models
{
    public sealed class Customer :IdentityUser
    {
        public Customer()
        {
            OrderHistory = new List<Order>();
        }
        
        [Required, DataType(DataType.Text), StringLength(50), RegularExpression(@"[a-zA-Z]{2,}$")]
        public string FirstName { get; set; }

        [Required, DataType(DataType.Text), StringLength(50), RegularExpression(@"[a-zA-Z]{2,}$")]
        public string LastName { get; set; }

        public Address Address { get; set; }

        public ICollection<Order> OrderHistory { get; set; }
    }
}