using System.Collections.Generic;
using System.Linq;

namespace GamingStore.Contracts
{
    public class Address
    {
        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string PostalCode { get; set; }

        public string Country { get; set; }

        public override string ToString()
        { 
            // shows values only if they aren't null.
            var values
                = typeof(Address).GetProperties()
                    .Select(prop => prop.GetValue(this, null))
                    .Where(val => val != null)
                    .Select(val => val.ToString())
                    .Where(str => !string.IsNullOrEmpty(str))
                    .ToList();

            return string.Join(",", values);
        }
    }
}