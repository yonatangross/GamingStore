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
using Microsoft.AspNetCore.Authorization;
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

        [Authorize]
        public async Task<IActionResult> Index()
        {
            try
            {
                Customer customer = await GetCurrentUserAsync();
                IQueryable<Cart> itemsInCart = _context.Carts.Where(c => c.CustomerId == customer.Id);

                foreach (Cart cartItem in itemsInCart)
                {
                    Item item = _context.Items.First(i => i.Id == cartItem.ItemId);
                    cartItem.Item = item;
                }

                return View(await _context.Carts.Where(c => c.CustomerId == customer.Id).ToListAsync());

            }
            catch (Exception e)
            {
                
            }

            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Delete()
        {
            Customer customer = await GetCurrentUserAsync();
            IQueryable<Cart> cart = _context.Carts.Where(c => c.CustomerId == customer.Id);
            _context.Carts.RemoveRange(cart);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
