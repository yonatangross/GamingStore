using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GamingStore.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GamingStore.Data;
using GamingStore.Models;
using GamingStore.Models.Relationships;
using GamingStore.Services.Currency;
using GamingStore.ViewModels;
using GamingStore.ViewModels.Orders;
using LoggerService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Vereyon.Web;

namespace GamingStore.Controllers
{
    public class OrdersController : BaseController
    {
        private readonly IFlashMessage _flashMessage;
        private readonly ILoggerManager _logger;

        public OrdersController(UserManager<Customer> userManager, StoreContext context, RoleManager<IdentityRole> roleManager, SignInManager<Customer> signInManager, IFlashMessage flashMessage, ILoggerManager logger)
            : base(userManager, context, roleManager, signInManager)
        {
            _flashMessage = flashMessage;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                Customer customer = await GetCurrentUserAsync();
                List<Cart> itemsInCart = await GetItemsInCart(customer);
                bool inactiveItemsInCart = itemsInCart.Any(c => !c.Item.Active);
                
                if (inactiveItemsInCart)
                {
                    _flashMessage.Danger("Some items in cart are no longer available");
                    return RedirectToAction("Index", "Carts");
                }
                
                const int defaultPaymentCost = 10;
                double itemsCost = itemsInCart.Aggregate<Cart, double>(0, (current, cart) => current + cart.Item.Price * cart.Quantity);

                if (itemsCost == 0)
                {
                    _flashMessage.Danger("Your cart does no longer has items, please add items to cart before proceed to checkout");
                    return RedirectToAction("Index", "Carts");
                }
                
                var totalCost = itemsCost + defaultPaymentCost;
                customer.Address ??= new Address();

                var currencies = await CurrencyConvert.GetExchangeRate(new List<string>
                {
                    "EUR", "GBP", "ILS"
                });

                foreach (CurrencyInfo currency in currencies)
                {
                    currency.Total = totalCost * currency.Value;
                }

                var viewModel = new CreateOrderViewModel
                {
                    Cart = itemsInCart,
                    Customer = customer,
                    ShippingAddress = customer.Address,
                    Payment = new Payment
                    {
                        ShippingCost = defaultPaymentCost,
                        ItemsCost = itemsCost,
                        Total = totalCost
                    },
                    ItemsInCart = await CountItemsInCart(),
                    Currencies = currencies
                };

                return View(viewModel);
            }
            catch (Exception e)
            {
                // ignored
            }

            return View(new CreateOrderViewModel() { ItemsInCart = await CountItemsInCart() });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(Order order)
        {

            //handle customer
            Customer customer = await GetCurrentUserAsync();
            order.Customer = customer;
            order.CustomerId = customer.Id;

            List<Cart> itemsInCart = await GetItemsInCart(customer);

            //handle order
            order.State = OrderState.New;
            order.OrderDate = DateTime.Now;
            order.Payment.PaymentMethod = PaymentMethod.CreditCard;
            order.Payment.Paid = true;
            order.PaymentId = order.Payment.Id;
            order.Payment.ItemsCost = itemsInCart.Aggregate<Cart, double>(0, (current, cart) => current + cart.Item.Price * cart.Quantity);
            order.Payment.Total = order.Payment.ItemsCost + order.Payment.ShippingCost;
            order.Store = await Context.Stores.FirstOrDefaultAsync(s => s.Name == "Website");
            order.ShippingMethod = order.Payment.ShippingCost switch
            {
                0 => ShippingMethod.Pickup,
                10 => ShippingMethod.Standard,
                45 => ShippingMethod.Express,
                _ => ShippingMethod.Other
            };

            //handle order items
            foreach (var cartItem in itemsInCart)
            {
                order.OrderItems.Add(new OrderItem()
                {
                    ItemId = cartItem.ItemId,
                    Item = cartItem.Item,
                    ItemsCount = cartItem.Quantity,
                    Order = order,
                    OrderId = order.Id
                });
            }

            //add order to db
            Context.Add(order);
            await Context.SaveChangesAsync();

            //clear cart
            IQueryable<Cart> carts = Context.Carts.Where(c => c.CustomerId == customer.Id);
            var items = 0;

            foreach (Cart itemInCart in carts)
            {
                items += itemInCart.Quantity;
            }

            Context.Carts.RemoveRange(carts);
            await Context.SaveChangesAsync();

            return RedirectToAction("ThankYouIndex", new { id = order.Id, items });
        }

        private async Task<List<Cart>> GetItemsInCart(Customer customer)
        {
            List<Cart> itemsInCart = await Context.Carts.Where(c => c.CustomerId == customer.Id).ToListAsync();

            foreach (Cart cartItem in itemsInCart)
            {
                Item item = Context.Items.First(i => i.Id == cartItem.ItemId);
                cartItem.Item = item;
            }

            return itemsInCart;
        }

        public async Task<IActionResult> ThankYouIndex(string id, int items)
        {
            Customer customer = await GetCurrentUserAsync();
            List<Cart> itemsInCart = await GetItemsInCart(customer);
            bool inactiveItemsInCart = itemsInCart.Any(c => !c.Item.Active);

            if (inactiveItemsInCart)
            {
                _flashMessage.Danger("Some items in cart are no longer available");
                return RedirectToAction("Index", "Carts");
            }

            double itemsCost = itemsInCart.Aggregate<Cart, double>(0, (current, cart) => current + cart.Item.Price * cart.Quantity);

            if (itemsCost == 0)
            {
                _flashMessage.Danger("Your cart does no longer has items, please add items to cart before proceed to checkout");
                return RedirectToAction("Index", "Carts");
            }
            Order order = null;

            try
            {
                order = Context.Orders.Include(o => o.Customer).First(o => o.Id == id);
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
                ShippingAddress = order.ShippingAddress,
                ItemsInCart = await CountItemsInCart(),
                OrderId = id
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

            Order order = await Context.Orders
                .Include(x => x.OrderItems)
                .ThenInclude(y => y.Item)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            order.Customer = await GetCurrentUserAsync();
            order.Payment = await Context.Payments.FirstOrDefaultAsync(payment => payment.Id == order.PaymentId);

            var viewModel = new OrderDetailsViewModel()
            {
                ItemsInCart = await CountItemsInCart(),
                Order = order
            };

            return View(viewModel);
        }

        // GET: Items/Edit/5
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Order order = await Context.Orders.Include(o => o.Payment).FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            var viewModel = new EditOrdersViewModel
            {
                Customers = await Context.Customers.Where(customer => !string.IsNullOrWhiteSpace(customer.UserName))
                    .Distinct().ToListAsync(),
                Order = order,
                ItemsInCart = await CountItemsInCart(),
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Order order)
        {
            if (!await OrderExists(order.Id))
            {
                _flashMessage.Danger("You cannot edit product that is no longer exists");
                return RedirectToAction("ListOrders", "Administration");
            }

            try
            {
                Order orderOnDb = await Context.Orders.Include(o => o.Payment).FirstOrDefaultAsync(o => o.Id == order.Id);
                orderOnDb.Payment.Paid = order.Payment.Paid;
                orderOnDb.Payment.PaymentMethod = order.Payment.PaymentMethod;
                orderOnDb.Payment.ShippingCost = order.Payment.ShippingCost;
                orderOnDb.Payment.ItemsCost = order.Payment.ItemsCost;
                orderOnDb.Payment.Total = order.Payment.Total;
                orderOnDb.Payment.RefundAmount = order.Payment.RefundAmount;
                orderOnDb.Payment.Notes = order.Payment.Notes;
                orderOnDb.State = order.State;

                Context.Update(orderOnDb);
                await Context.SaveChangesAsync();
                _flashMessage.Confirmation("Order has been updated");
            }
            catch (Exception e)
            {
                _flashMessage.Danger("Order could not be updated");
                _logger.LogError($"Product# '{order.Id}' could not be updated, ex: {e}");
            }

            return RedirectToAction("ListOrders", "Administration");
        }

        private async Task<bool> OrderExists(string id)
        {
            return await Context.Orders.AnyAsync(e => e.Id == id);
        }
    }
}