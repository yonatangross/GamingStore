using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GamingStore.Models.Relationships;

namespace GamingStore.Models
{
    public class Item
    {
        [Key,DatabaseGenerated((DatabaseGeneratedOption.Identity))]
        public int Id { get; set; }
        [DataType(DataType.Text)]
        public string Title { get; set; }
        [DataType(DataType.Text)]
        public string Manufacturer { get; set; }
        public int Price { get; set; }
        [DataType(DataType.Text)]
        public string Category { get; set; }
        public Dictionary<string, string> PropertiesList { get; set; }
        public ICollection<StoreItem> StoreItems { get; set; } // many to many relationship
        public ICollection<OrderItem> OrderItems { get; set; } // many to many relationship

    }
}
