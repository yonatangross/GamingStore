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
using Microsoft.AspNetCore.Authorization;
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
                List<Cart> itemsInCart = await _context.Carts.Where(c => c.CustomerId == customer.Id).ToListAsync();

                foreach (Cart cartItem in itemsInCart)
                {
                    Item item = _context.Items.First(i => i.Id == cartItem.ItemId);
                    cartItem.Item = item;
                }

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

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(Order order)
        {
            order.State = OrderState.New;
            order.OrderDate = DateTime.Now;
            order.Payment.PaymentMethod = PaymentMethod.CreditCard;
            order.Payment.Paid = true;
            order.Payment.Total = order.Payment.ItemsCost + order.Payment.ShippingCost;
            

            Customer customer = await GetCurrentUserAsync();
            order.Customer = customer;
            order.CustomerId = customer.Id;
            _context.Add(order);
            await _context.SaveChangesAsync();
            //order.Customer
            return RedirectToAction("ThankYouIndex");
        }


        public async Task<IActionResult> ThankYouIndex()
        {
            Customer customer = await GetCurrentUserAsync();
            List<Cart> qCarts = await _context.Carts.Where(c => c.CustomerId == customer.Id).ToListAsync();
            var storeContext = _context.Orders.Include(o => o.Customer).Include(o => o.Store);
            try
            {
                var viewModel = new OrderPlacedViewModel
                {
                    Cart = qCarts,
                    Customer = customer,
                    ShippingAddress = customer.Address,
                };
                return View(viewModel);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.StackTrace+"\n"+exception.Message);;
            }

            return View();
        }
    }
}