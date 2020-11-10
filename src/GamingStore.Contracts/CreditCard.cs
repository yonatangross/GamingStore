using System.ComponentModel.DataAnnotations;

namespace GamingStore.Contracts
{
    public class CreditCard
    {
        [DataType(DataType.Text), Display(Name = "Owner Name")]
        public string OwnerName { get; set; }

        [DataType(DataType.CreditCard)]
        [Range(100000000000, 9999999999999999999, ErrorMessage = "Credit card number is not valid")]
        public string Number { get; set; }

        [Range(00, 12, ErrorMessage = "Expiration month is not valid")]
        [DataType(DataType.Currency), Display(Name = "Expiration Month")]
        public string ExpirationMonth { get; set; }

        [Range(2020, 2034, ErrorMessage = "Expiration month is not valid")]
        [DataType(DataType.Currency), Display(Name = "Expiration Year")]
        public string ExpirationYear { get; set; }

        [Range(000, 999, ErrorMessage = "CVV is not valid")]
        [DataType(DataType.Currency), Display(Name = "CVV (Code)")]
        public string CVV { get; set; }
    }
}