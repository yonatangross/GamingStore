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
        public Mail Mail { get; set; }
    }
}
