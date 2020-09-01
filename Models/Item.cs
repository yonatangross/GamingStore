using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GamingStore.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Manufacturer { get; set; }
        public int Price { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        [NotMapped]

        public Dictionary<string, string> PropertiesList { get; set; }


    }
}
