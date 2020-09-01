using System.Collections.Generic;
using GamingStore.Contracts;

namespace GamingStore.Models
{
    public class Store
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Address Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public  OpeningHours[] OpeningHours { get; set; }
        public Dictionary<Item,ushort> ItemsList { get; set; } // determines how many items there are in the store. example: {{fridge, 5},{mouse,6}}
    }
}