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
    public class CombinedOrder
    {
        [Key, DatabaseGenerated((DatabaseGeneratedOption.None))]
        public string OrderId { get; set; }
        public Order Order { get; set; }

        public string ItemId { get; set; }
        public Item Item { get; set; }


    }
}