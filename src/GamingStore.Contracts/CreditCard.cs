using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GamingStore.Contracts
{
    public class CreditCard
    {
        [DataType(DataType.Text), Display(Name = "Owner Name")] public string OwnerName { get; set; }
        [DataType(DataType.CreditCard)] public string Number { get; set; }
        [DataType(DataType.Currency), Display(Name = "Expiration Month")] public string ExpirationMonth { get; set; }
        [DataType(DataType.Currency),Display(Name = "Expiration Year")] public string ExpirationYear { get; set; }
        [DataType(DataType.Currency), Display(Name = "CVV (Code)")] public string CVV { get; set; }
    }
}