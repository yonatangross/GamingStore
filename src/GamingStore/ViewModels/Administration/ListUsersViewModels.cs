using System.Collections.Generic;
using GamingStore.Models;

namespace GamingStore.ViewModels.Administration
{
    public class ListUsersViewModels
    {
        public List<Customer> Users { get; set; }
        public Customer CurrentUser { get; set; }
        public IList<string> CurrentUserRoles { get; set; }
    }
}