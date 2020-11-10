#nullable enable
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace GamingStore.Contracts
{
    public class Address
    {
        [Display(Name = "Full Name"), DataType(DataType.Text)]
        public string? FullName { get; set; }

        [Required, DataType(DataType.Text),Display(Name = "Address")]
        public string? Address1 { get; set; }

        [Required, DataType(DataType.Text)] public string? City { get; set; }

        [DataType(DataType.PostalCode), Required, Display(Name = "Postal Code")]
        public string? PostalCode { get; set; }

        [DataType(DataType.Text)] public string? Country { get; set; } = "Israel";

        public override string ToString()
        {
            // shows values only if they aren't null.
            List<string?> values = typeof(Address).GetProperties().Select(prop => prop.GetValue(this, null))
                .Where(val => val != null).Select(val => val?.ToString()).Where(str => !string.IsNullOrEmpty(str))
                .ToList();

            return string.Join(", ", values);
        }
    }
}