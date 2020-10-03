using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace GamingStore.Contracts
{
    //todo: move to be a model
    public class Address 
    {
        [Required]
        public string Address1 { get; set; }

        public string Address2 { get; set; }

        [Required] 
        public string City { get; set; }

        public string State { get; set; }

        [Required] 
        public string PostalCode { get; set; }

        //todo: remove country 
        [Required]
        public string Country { get; set; }

        public override string ToString()
        {
            // shows values only if they aren't null.
            //todo: fix warning, aviv miranda.
            List<string?> values
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