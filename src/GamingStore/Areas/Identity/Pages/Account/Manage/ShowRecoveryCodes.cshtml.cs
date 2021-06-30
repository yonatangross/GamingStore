using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GamingStore.Data;
using GamingStore.Models;
using GamingStore.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace GamingStore.Areas.Identity.Pages.Account.Manage
{
    public class ShowRecoveryCodesModel : ViewPageModel
    {
        private readonly UserManager<Customer> _userManager;

        public ShowRecoveryCodesModel(UserManager<Customer> userManager, StoreContext context)
            : base(context)
        {
            _userManager = userManager;
        }

        [TempData]
        public string[] RecoveryCodes { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGet()
        {
            Customer user = await _userManager.GetUserAsync(User);
            ItemsInCart = await CountItemsInCart(user);

            if (RecoveryCodes == null || RecoveryCodes.Length == 0)
            {
                return RedirectToPage("./TwoFactorAuthentication");
            }

            return Page();
        }
    }
}
