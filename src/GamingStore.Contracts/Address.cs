#nullable enable
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace GamingStore.Contracts
{
    public class Address 
    {
        [Display(Name = "Full Name")]
        public string? FullName { get; set; }

        [Required]
        [Display(Name = "Address")]
        public string? Address1 { get; set; }

        [Required] 
        public string? City { get; set; }

        [Required]
        [Display(Name = "Postal Code")]
        public string? PostalCode { get; set; }

        public string? Country { get; set; } = "Israel";

        public override string ToString()
        {
            // shows values only if they aren't null.
            List<string?> values
                = typeof(Address).GetProperties()
                    .Select(prop => prop.GetValue(this, null))
                    .Where(val => val != null)
                    .Select(val => val?.ToString())
                    .Where(str => !string.IsNullOrEmpty(str))
                    .ToList();

            return string.Join(", ", values);
        }

    }
}