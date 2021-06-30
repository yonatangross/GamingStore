using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GamingStore.Contracts;
using GamingStore.Data;
using GamingStore.Models;
using GamingStore.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GamingStore.Areas.Identity.Pages.Account.Manage
{
    public class SetAddressModel : ViewPageModel
    {
        private readonly UserManager<Customer> _userManager;

        public SetAddressModel(UserManager<Customer> userManager, StoreContext context) 
            : base(context)
        {
            _userManager = userManager;
        }

        public Address Address { get; set; }
        public string StatusMessage { get; set; }
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            public Address Address { get; set; }
        }

        private void Load(Customer user)
        {
            Address = user.Address;

            Input = new InputModel
            {
                Address = Address
            };
        }
        
        public async Task<IActionResult> OnPostSaveAddressAsync(Address address)
        {
            Customer user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            user.Address = address;
            await _userManager.UpdateAsync(user);

            StatusMessage = "Your email is unchanged.";
            return RedirectToPage();
        }


        public async Task<IActionResult> OnGetAsync()
        {
            Customer user = await _userManager.GetUserAsync(User);
            ItemsInCart = await CountItemsInCart(user);
            
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            Load(user);
            return Page();
        }
    }
}