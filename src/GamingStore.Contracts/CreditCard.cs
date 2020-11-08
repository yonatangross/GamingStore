using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamingStore.Contracts
{
    public class CreditCard
    {
        public string OwnerName { get; set; }

        public string Number { get; set; }

        public string ExpirationMonth { get; set; }

        public string ExpirationYear { get; set; }

        public string CVV { get; set; }
    }
}
