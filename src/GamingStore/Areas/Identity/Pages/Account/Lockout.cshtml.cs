using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GamingStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GamingStore.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LockoutModel : ViewModelBase
    {
        public void OnGet()
        {

        }
    }
}
