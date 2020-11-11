using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GamingStore.Data;
using GamingStore.Models;
using GamingStore.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GamingStore.Areas.Identity.Pages.Account
{
    public class AccessDeniedModel : ViewPageModel
    {
        private readonly UserManager<Customer> _userManager;

        public AccessDeniedModel(UserManager<Customer> userManager, StoreContext context)
            : base(context)
        {
            _userManager = userManager;
        }

        public async Task OnGet()
        {
            Customer user = await _userManager.GetUserAsync(User);
            ItemsInCart = await CountItemsInCart(user);
        }
    }
}

