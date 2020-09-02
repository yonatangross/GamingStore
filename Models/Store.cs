using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using GamingStore.Contracts;

namespace GamingStore.Models
{
    public class Store
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Name { get; set; }
        public Address Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        [NotMapped]
        public  OpeningHours[] OpeningHours { get; set; }
        [NotMapped]
        public Dictionary<Item,ushort> ItemsList { get; set; } // determines how many items there are in the store. example: {{fridge, 5},{mouse,6}}
    }
}