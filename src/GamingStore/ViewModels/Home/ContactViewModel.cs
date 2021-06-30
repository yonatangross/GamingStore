using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GamingStore.Models;

namespace GamingStore.ViewModels.Home
{
    public class ContactViewModel : ViewModelBase
    {
        [Required]
        [DataType(DataType.Text)]
        [StringLength(50)]
        [RegularExpression(@"[a-zA-Z]{2,}$")]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(50)]
        [EmailAddress]
        public string Email { get; set; }

        
        [DataType(DataType.PhoneNumber), StringLength(10)]
        [Range(0000000000, 9999999999, ErrorMessage = "Phone number is not valid")]
        public string PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "The message length is too short")]
        public string Message { get; set; }
    }
}
