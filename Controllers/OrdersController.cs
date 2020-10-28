using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GamingStore.Data;
using GamingStore.Models;
using GamingStore.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Query;

namespace GamingStore.Controllers
{
    public class OrdersController : Controller
    {
        private readonly StoreContext _context;
        private readonly UserManager<Customer> _userManager;

        public OrdersController(StoreContext context, UserManager<Customer> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private Task<Customer> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var storeContext = _context.Orders.Include(o => o.Customer).Include(o => o.Store);
            return View(await storeContext.ToListAsync());
        }

        public async Task<IActionResult> CheckOutIndex()
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

                List<Cart> carts = await _context.Carts.Where(c => c.CustomerId == customer.Id).ToListAsync();

                var viewModel = new CreateOrderViewModel
                {
                    Cart = carts
                };

                return View(viewModel);

            }
            catch (Exception e)
            {

            }

            return View();
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }

        public async Task<IActionResult> ThankYouIndex()
        {
            var storeContext = _context.Orders.Include(o => o.Customer).Include(o => o.Store);
            return View(await storeContext.ToListAsync());
        }
    }
}
