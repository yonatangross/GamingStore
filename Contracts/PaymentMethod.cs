using System.ComponentModel.DataAnnotations;

namespace GamingStore.Contracts
{
    public enum PaymentMethod
    {
        Cash,
        [Display(Name = "Credit Card")]
        CreditCard,
        Paypal
    }
}