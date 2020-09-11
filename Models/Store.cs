using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Policy;
using System.Threading;
using GamingStore.Contracts;
using GamingStore.Models.Relationships;

namespace GamingStore.Models
{
    public class Store
    {
        public static int StoreCounter = 0;

        public Store()
        {
            Orders = new List<Order>();
            StoreItems = new List<StoreItem>();
            Id = StoreCounter;
            Interlocked.Increment(ref StoreCounter);
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required, DataType(DataType.Text)] public string Name { get; set; }

        public Address Address { get; set; }

        //TODO: add geolocation.
        [DataType(DataType.PhoneNumber)] public string PhoneNumber { get; set; }
        [DataType(DataType.EmailAddress)] public string Email { get; set; }

        public OpeningHours[] OpeningHours { get; set; }

        //public Dictionary<Item,uint> Stock { get; set; } // determines how many items there are in the store. example: {{fridge, 5},{mouse,6}}
        public ICollection<Order> Orders { get; set; }
        public ICollection<StoreItem> StoreItems { get; set; } // many to many relationship
    }
}