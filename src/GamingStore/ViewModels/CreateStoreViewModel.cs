using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GamingStore.Contracts;
using GamingStore.Models;
using GamingStore.Models.Relationships;

namespace GamingStore.ViewModels
{
    public class CreateStoreViewModel : ViewModelBase
    {
        public Store Store { get; set; }
    }
}

