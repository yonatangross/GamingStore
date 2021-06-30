using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GamingStore.Data;
using GamingStore.Models;
using GamingStore.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Vereyon.Web;

namespace GamingStore.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : ViewPageModel
    {
        private readonly UserManager<Customer> _userManager;
        private readonly SignInManager<Customer> _signInManager;
        private readonly IFlashMessage _flushMessage;

        public IndexModel(UserManager<Customer> userManager, SignInManager<Customer> signInManager, StoreContext context, IFlashMessage flushMessage) 
            : base(context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _flushMessage = flushMessage;
        }


        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Display(Name = "Phone number")]
            [Required, DataType(DataType.PhoneNumber), StringLength(10), Phone]
            public string PhoneNumber { get; set; }

            public string Username { get; set; }
        }

        private async Task LoadAsync(Customer user)
        {
            Input = new InputModel
            {
                PhoneNumber = user.PhoneNumber,
                Username = user.UserName
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Customer user = await _userManager.GetUserAsync(User);
            ItemsInCart = await CountItemsInCart(user);

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";

            return RedirectToPage();
        }
    }
}
