using System;
using System.Collections.Generic;
using System.Text;

namespace GamingStore.Contracts.ML
{
    public class Request
    {
        public List<ItemsCustomers> ItemCustomersList { get; set; }

        public  List<int> AllItemsIds { get; set; }

        public int CustomerNumber { get; set; }
    }
}
