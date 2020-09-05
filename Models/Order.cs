using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using GamingStore.Contracts;
using GamingStore.Contracts.Converters;
using GamingStore.Data;
using Newtonsoft.Json;

namespace GamingStore.Models
{
    public class Order
    {
        public int Id { get; set; }
        //todo: change saving in db to <id,uint> instead of <Item,uint>
        [JsonProperty(ItemConverterType = typeof(DictionaryJsonConverter))]
        public Dictionary<Item, uint> Items { get; set; }
        public Customer Customer { get; set; }
        public Store Store { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderState State { get; set; }
        public Payment Payment { get; set; }
        //todo: add coupons maybe

        
    }
}
