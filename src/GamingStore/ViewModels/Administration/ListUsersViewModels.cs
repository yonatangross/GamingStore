using System.Collections.Generic;
using GamingStore.Models;

namespace GamingStore.ViewModels.Administration
{
    public class ListUsersViewModels: ViewModelBase
    {
        public List<Customer> Users { get; set; }
    }
}