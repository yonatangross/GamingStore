using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GamingStore.Models
{
    public class Cart
    {
        public static int ItemCounter;

        public Cart()
        {
            Id = ItemCounter;
            Interlocked.Increment(ref ItemCounter);
        }

        [Key, DatabaseGenerated((DatabaseGeneratedOption.None))]
        public int Id { get; set; }
        
        public string CustomerId { get; set; }

        public int ItemId { get; set; }

        public int Quantity { get; set; }
        
        [NotMapped]
        public Item Item { get; set; }
    }
}
