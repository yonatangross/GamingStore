using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GamingStore.Data;
using GamingStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GamingStore.Controllers
{
    public class BaseController : Controller
    {
        protected readonly UserManager<Customer> UserManager;
        protected readonly StoreContext Context;
        protected readonly RoleManager<IdentityRole> RoleManager;
        protected readonly SignInManager<Customer> SignInManager;

        public BaseController(UserManager<Customer> userManager, StoreContext context, RoleManager<IdentityRole> roleManager, SignInManager<Customer> signInManager)
        {
            UserManager = userManager;
            Context = context;
            RoleManager = roleManager;
            SignInManager = signInManager;
        }

        protected Task<Customer> GetCurrentUserAsync() => UserManager.GetUserAsync(User);

        public Task<int> ItemsInCart =>  CountItemsInCart();

        protected async Task<int> CountItemsInCart()
        {
            Customer user = await GetCurrentUserAsync();

            if (user == null)
            {
                return 0;
            }

            var itemsInCart = 0;

            foreach (Cart itemInCart in Context.Carts.Where(c => c.CustomerId == user.Id))
            {
                itemsInCart += itemInCart.Quantity;
            }


            return itemsInCart;
        }
    }
}