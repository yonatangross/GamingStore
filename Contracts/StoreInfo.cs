using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GamingStore.Models;

namespace GamingStore.Contracts
{
    public class StoreInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Address Address { get; set; }
        //TODO: add geolocation.
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public OpeningHours[] OpeningHours { get; set; }
        public Dictionary<Item, uint> Stock { get; set; } // determines how many items there are in the store. example: {{fridge, 5},{mouse,6}}
        public ICollection<Order> Orders { get; set; }
    }
}
