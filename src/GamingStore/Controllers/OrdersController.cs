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
using GamingStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace GamingStore.Controllers
{
    public class OrdersController : BaseController
    {
        public OrdersController(UserManager<Customer> userManager, StoreContext context, RoleManager<IdentityRole> roleManager) : base(userManager, context, roleManager)
        {
        }
        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var storeContext = Context.Orders.Include(o => o.Customer).Include(o => o.Store);
            return View(await storeContext.ToListAsync());
        }

        public async Task<IActionResult> CheckOutIndex()
        {
            try
            {
                Customer customer = await GetCurrentUserAsync();
                List<Cart> itemsInCart = await GetItemsInCart(customer);
                const int defaultPaymentCost = 10;
                double itemsCost =
                    itemsInCart.Aggregate<Cart, double>(0,
                        (current, cart) => current + cart.Item.Price * cart.Quantity);
                double totalCost = itemsCost + defaultPaymentCost;

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
            List<Cart> itemsInCart = await Context.Carts.Where(c => c.CustomerId == customer.Id).ToListAsync();

            foreach (Cart cartItem in itemsInCart)
            {
                Item item = Context.Items.First(i => i.Id == cartItem.ItemId);
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
            order.Payment.ItemsCost =
                itemsInCart.Aggregate<Cart, double>(0, (current, cart) => current + cart.Item.Price * cart.Quantity);
            order.Payment.Total = order.Payment.ItemsCost + order.Payment.ShippingCost;
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

            //items bundles


            /*
            for (var i = 0; i < itemsInCart.Count - 1; i++)
            {
                for (var j = i + 1; j < itemsInCart.Count; j++)
                {
                    var frontItemBundle = new RelatedItem(itemsInCart[i].ItemId, itemsInCart[j].ItemId);
                    var backItemBundle = new RelatedItem(itemsInCart[j].ItemId, itemsInCart[i].ItemId);
                    try
                    {
                        var a = _context.ItemsBundles.Where(i =>
                            i.CustomerId == frontItemBundle.CustomerId && i.ItemId == frontItemBundle.ItemId);
                        if (!a.Any())
                            await _context.ItemsBundles.AddAsync(frontItemBundle);

                        var b = _context.ItemsBundles.Where(i =>
                            i.CustomerId == backItemBundle.CustomerId && i.ItemId == backItemBundle.ItemId);
                        if (!b.Any())
                            await _context.ItemsBundles.AddAsync(backItemBundle);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                  
                }
            }*/

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

            return RedirectToAction("ThankYouIndex", new {id = order.Id, items});
        }

        public async Task<IActionResult> ThankYouIndex(string id, int items)
        {
            Customer customer = await GetCurrentUserAsync();
            List<Cart> carts = await Context.Carts.Where(c => c.CustomerId == customer.Id).ToListAsync();
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

            Order order = await Context.Orders.Include(x => x.OrderItems).ThenInclude(y => y.Item)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            order.Customer = await GetCurrentUserAsync();
            order.Payment = await Context.Payments.FirstOrDefaultAsync(payment => payment.Id == order.PaymentId);

            return View(order);
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
                Order = order
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Order order)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Order orderOnDb = await Context.Orders.Include(o => o.Payment)
                        .FirstOrDefaultAsync(o => o.Id == order.Id);
                    orderOnDb.Payment.Paid = order.Payment.Paid;
                    orderOnDb.Payment.PaymentMethod = order.Payment.PaymentMethod;
                    orderOnDb.Payment.ShippingCost = order.Payment.ShippingCost;
                    orderOnDb.Payment.ItemsCost = order.Payment.ItemsCost;
                    orderOnDb.Payment.Total = order.Payment.Total;
                    orderOnDb.State = order.State;

                    Context.Update(orderOnDb);
                    await Context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await OrderExists(order.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction("ListOrders", "Administration");
            }

            var viewModel = new EditOrdersViewModel
            {
                Customers = await Context.Customers.Where(customer => !string.IsNullOrWhiteSpace(customer.UserName))
                    .Distinct().ToListAsync(),
                Order = order
            };

            return View(viewModel);
        }

        private async Task<bool> OrderExists(string id)
        {
            return await Context.Orders.AnyAsync(e => e.Id == id);
        }

        // GET: Orders/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Order order = await Context.Orders.FirstOrDefaultAsync(m => m.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            Order order = await Context.Orders.FindAsync(id);
            Context.Orders.Remove(order);
            await Context.SaveChangesAsync();

            return RedirectToAction("ListOrders", "Administration");
        }

       
    }
}