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

        public BaseController(UserManager<Customer> userManager, StoreContext context, RoleManager<IdentityRole> roleManager)
        {
            UserManager = userManager;
            Context = context;
            RoleManager = roleManager;
        }

        protected Task<Customer> GetCurrentUserAsync() => UserManager.GetUserAsync(User);

        protected async Task<int> CountItemsInCart()
        {
            var user = await GetCurrentUserAsync();
            var userCart = await Context.Carts.FirstOrDefaultAsync(cart => cart.CustomerId == user.Id);

            return userCart?.Quantity ?? 0;
        }
    }
}
