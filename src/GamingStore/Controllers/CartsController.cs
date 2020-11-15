using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GamingStore.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GamingStore.Data;
using GamingStore.Models;
using GamingStore.ViewModels;
using GamingStore.ViewModels.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GamingStore.Controllers
{
    [Route("cart")]
    public class CartsController : BaseController
    {
        public CartsController(UserManager<Customer> userManager, StoreContext context, RoleManager<IdentityRole> roleManager, SignInManager<Customer> signInManager)
            : base(userManager, context, roleManager, signInManager)
        {
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            try
            {
                Customer customer = await GetCurrentUserAsync();
                IQueryable<Cart> itemsInCart = Context.Carts.Where(c => c.CustomerId == customer.Id);

                foreach (Cart cartItem in itemsInCart)
                {
                    Item item = Context.Items.First(i => i.Id == cartItem.ItemId);
                    cartItem.Item = item;
                }

                var itemsPrice = 0.00;

                foreach (Cart cart in itemsInCart)
                {
                    itemsPrice += cart.Item.Price * cart.Quantity;
                }

                var viewModel = new CartViewModel
                {
                    Carts = await itemsInCart.ToListAsync(),
                    Payment = new Payment
                    {
                        ItemsCost = itemsPrice
                    },
                    ItemsInCart = await CountItemsInCart()
                };

                return View(viewModel);
            }
            catch
            {
                //no items in cart
                return View(new CartViewModel {ItemsInCart = 0});
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Delete()
        {
            Customer customer = await GetCurrentUserAsync();
            IQueryable<Cart> cart = Context.Carts.Where(c => c.CustomerId == customer.Id);
            Context.Carts.RemoveRange(cart);
            await Context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

       
    }
}
