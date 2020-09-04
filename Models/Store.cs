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
        //TODO: add geolocation.
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public  OpeningHours[] OpeningHours { get; set; }
        public Dictionary<Item,uint> Stock { get; set; } // determines how many items there are in the store. example: {{fridge, 5},{mouse,6}}

        public ICollection<Order> Orders { get; set; }

    }
}