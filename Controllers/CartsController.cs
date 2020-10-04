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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GamingStore.Controllers
{
    [Route("cart")]
    public class CartsController : Controller
    {
        private readonly UserManager<Customer> _userManager;
        private readonly StoreContext _context;

        public CartsController(StoreContext context, UserManager<Customer> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private Task<Customer> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            Customer customer = await GetCurrentUserAsync();
            Cart cart = await _context.Carts.FirstOrDefaultAsync(c => c.CustomerId == customer.Id);

            if (cart == null)
            {
                return NotFound();
            }

            try
            {
                foreach ((int key, uint value) in cart.ShoppingCart)
                {
                    Item item = _context.Items.First(i => i.Id == key);
                    cart.ItemsInCart.Add(item, value);
                }
            }
            catch (Exception e)
            {
                
            }

            //var list = new List<ItemsInCart>();

            //foreach (KeyValuePair<int, uint> item in cart.ShoppingCart)
            //{
            //    foreach (Item contextItem in _context.Items)
            //    {
            //        if (contextItem.Id != item.Value)
            //        {
            //            continue;
            //        }

            //        var itemInCart = new ItemsInCart()
            //        {
            //            Price = contextItem.Price,
            //            ItemId = contextItem.Id,
            //            Quantity = item.Value,
            //            Title = contextItem.Title
            //        };

            //        list.Add(itemInCart);
            //        break;
            //    }
            //}

            return View(cart);
        }

        // POST: Carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var cart = await _context.Carts.FindAsync(id);
            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CartExists(string id)
        {
            return _context.Carts.Any(e => e.CustomerId == id);
        }
    }
}
