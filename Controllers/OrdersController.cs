using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GamingStore.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GamingStore.Data;
using GamingStore.Models;
using GamingStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

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
                List<Cart> itemsInCart = await GetItemsInCart(customer);

                var viewModel = new CreateOrderViewModel
                {
                    Cart = itemsInCart,
                    Customer = customer,
                    ShippingAddress = customer.Address,
                    Payment = new Payment
                    {
                        ShippingCost = 10
                    }
                };

                return View(viewModel);
            }
            catch (Exception e)
            {
            }

            return View();
        }

        private async Task<List<Cart>> GetItemsInCart(Customer customer)
        {
            List<Cart> itemsInCart = await _context.Carts.Where(c => c.CustomerId == customer.Id).ToListAsync();

            foreach (Cart cartItem in itemsInCart)
            {
                Item item = _context.Items.First(i => i.Id == cartItem.ItemId);
                cartItem.Item = item;
            }

            return itemsInCart;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(Order order)
        {
            //handle customer
            Customer customer = await GetCurrentUserAsync();
            order.Customer = customer;
            order.CustomerId = customer.Id;

            //handle order
            order.State = OrderState.New;
            order.OrderDate = DateTime.Now;
            order.Payment.PaymentMethod = PaymentMethod.CreditCard;
            order.Payment.Paid = true;
            order.PaymentId = order.Payment.Id;
            List<Cart> itemsInCart = await GetItemsInCart(customer);
            order.Payment.ItemsCost = itemsInCart.Aggregate<Cart, double>(0, (current, cart) => current + cart.Item.Price * cart.Quantity);
            order.Payment.Total = order.Payment.ItemsCost + order.Payment.ShippingCost;
            order.ShippingMethod = order.Payment.ShippingCost switch
            {
                0 => ShippingMethod.Pickup,
                10 => ShippingMethod.Standard,
                45 => ShippingMethod.Express,
                _ => ShippingMethod.Other
            };

            //add order to db
            _context.Add(order);
            await _context.SaveChangesAsync();

            //clear cart
            IQueryable<Cart> carts = _context.Carts.Where(c => c.CustomerId == customer.Id);

            var items = 0;

            foreach (Cart itemInCart in carts)
            {
                items += itemInCart.Quantity;
            }

            _context.Carts.RemoveRange(carts);
            await _context.SaveChangesAsync();

            return RedirectToAction("ThankYouIndex", new { id = order.Id, items });
        }

        public async Task<IActionResult> ThankYouIndex(string id, int items)
        {
            Customer customer = await GetCurrentUserAsync();
            List<Cart> carts = await _context.Carts.Where(c => c.CustomerId == customer.Id).ToListAsync();
            Order order = null;

            try
            {
                order = _context.Orders.Include(o => o.Customer).First(o => o.Id == id);
            }
            catch
            {
                //ignored
            }

            if (order == null)
            {
                return View();
            }

            var viewModel = new OrderPlacedViewModel
            {
                ItemsCount = items,
                Customer = customer,
                ShippingAddress = order.ShippingAddress
            };

            return View(viewModel);
        }


        // GET: Orders/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            Order order = await _context.Orders.FirstOrDefaultAsync(order => order.Id == id);
            order.Customer = await _userManager.GetUserAsync(User);
            order.Payment = await _context.Payments.FirstOrDefaultAsync(payment => payment.Id == order.PaymentId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
    }
}