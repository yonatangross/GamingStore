using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading;
using GamingStore.Contracts;
using GamingStore.Models.Relationships;

namespace GamingStore.Models
{
    public class Item 
    {
        public static int ItemCounter;

        public Item()
        {
            StoreItems = new List<StoreItem>();
            OrderItems = new List<OrderItem>();
            Id = ItemCounter;
            Interlocked.Increment(ref ItemCounter);
        }

        [Key, DatabaseGenerated((DatabaseGeneratedOption.None))]
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Title { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Manufacturer { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        [Range(1, 99999, ErrorMessage = "Price between 1 to 99999")]
        public double Price { get; set; }

        [DataType(DataType.Text)]
        public string Description { get; set; }
        
        [Required]
        [DataType(DataType.Text)] 
        public string Category { get; set; }
        
        public Dictionary<string, string> PropertiesList { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        public bool Active { get; set; } = true;
        public ICollection<StoreItem> StoreItems { get; set; } // many to many relationship
        
        public ICollection<OrderItem> OrderItems { get; set; } // many to many relationship
    }
}