using System.Collections.Generic;
using GamingStore.Models;

namespace GamingStore.ViewModels
{
    public class ListUsersViewModels: ViewModelBase
    {
        public List<Customer> Users { get; set; }
    }
}