using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GamingStore.ViewModels
{
    public class ListUserRoleViewModel : ViewModelBase
    {
        public List<UserRoleViewModel> List { get; set; }
    }
}
