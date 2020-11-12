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
using GamingStore.ViewModels.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace GamingStore.Controllers
{
    public class StoresController : BaseController
    {
        public StoresController(UserManager<Customer> userManager, StoreContext context, RoleManager<IdentityRole> roleManager, SignInManager<Customer> signInManager)
            : base(userManager, context, roleManager, signInManager)
        {
        }

        // GET: Stores
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var stores = await Context.Stores.ToListAsync();
            var uniqueCities = GetUniqueCities(stores);
            var openStores = GetOpenStores(stores);


            var viewModel = new StoresCitiesViewModel
            {
                Stores = stores,
                CitiesWithStores = uniqueCities.ToArray(),
                OpenStores = openStores,
                ItemsInCart = await CountItemsInCart()
            };

            return View(viewModel);
        }

        private static List<Store> GetOpenStores(List<Store> stores)
        {
            // get open stores
            var openStores = stores.Where(store => store.IsOpen()).ToList();
            return openStores;
        }

        // Post: Stores
        [HttpPost]
        public async Task<IActionResult> Index(StoresCitiesViewModel received)
        {
            var stores = await Context.Stores.ToListAsync();
            var uniqueCities = GetUniqueCities(stores);

            // get open stores
            var openStores = stores.Where(store => store.IsOpen()).ToList();


            if (!string.IsNullOrEmpty(received.Name))
            {
                stores = stores.Where(store => store.Name.ToLower().Contains(received.Name.ToLower())).ToList();
            }

            if (!string.IsNullOrEmpty(received.City))
            {
                stores = stores.Where(store => store.Address.City == received.City).ToList();
            }

            if (received.IsOpen)
            {
                stores = stores.Where(store => store.IsOpen()).ToList();
            }

            var viewModel = new StoresCitiesViewModel
            {
                Stores = stores,
                CitiesWithStores = uniqueCities.ToArray(),
                OpenStores = openStores,
                Name = received.Name,
                City = received.City,
                IsOpen = received.IsOpen,
                ItemsInCart = await CountItemsInCart()
            };

            return View(viewModel);
        }


        private static HashSet<string> GetUniqueCities(List<Store> stores)
        {
            // get stores with cities uniquely 
            HashSet<string> uniqueCities = new HashSet<string>();
            foreach (var element in stores)
                uniqueCities.Add(element.Address.City);
            return uniqueCities;
        }


        // GET: Stores/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Store store = await Context.Stores.FirstOrDefaultAsync(m => m.Id == id);
            if (store == null)
            {
                return NotFound();
            }

            var viewModel = new StoreDetailsViewModel()
            {
                Store = store,
                ItemsInCart = await CountItemsInCart()
            };

            return View(viewModel);
        }

        // GET: Stores/Create
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var viewModel = new CreateStoreViewModel()
            {
                Store = new Store()
                {
                    Active = true,
                    OpeningHours = new List<OpeningHours>(7)
                    {
                        new OpeningHours() {OpeningTime = new TimeSpan(00, 00, 00), ClosingTime = new TimeSpan(23, 59, 00), DayOfWeek = DayOfWeek.Sunday},
                        new OpeningHours() {OpeningTime = new TimeSpan(00, 00, 00), ClosingTime = new TimeSpan(23, 59, 00), DayOfWeek = DayOfWeek.Monday},
                        new OpeningHours() {OpeningTime = new TimeSpan(00, 00, 00), ClosingTime = new TimeSpan(23, 59, 00), DayOfWeek = DayOfWeek.Tuesday},
                        new OpeningHours() {OpeningTime = new TimeSpan(00, 00, 00), ClosingTime = new TimeSpan(23, 59, 00), DayOfWeek = DayOfWeek.Wednesday},
                        new OpeningHours() {OpeningTime = new TimeSpan(00, 00, 00), ClosingTime = new TimeSpan(23, 59, 00), DayOfWeek = DayOfWeek.Thursday},
                        new OpeningHours() {OpeningTime = new TimeSpan(00, 00, 00), ClosingTime = new TimeSpan(23, 59, 00), DayOfWeek = DayOfWeek.Friday},
                        new OpeningHours() {OpeningTime = new TimeSpan(00, 00, 00), ClosingTime = new TimeSpan(23, 59, 00), DayOfWeek = DayOfWeek.Saturday}
                    }
                },
                ItemsInCart = await CountItemsInCart()
            };
            return View(viewModel);
        }

        // POST: Stores/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Name,Address,PhoneNumber,Email,OpeningHours")]
            Store store)
        {
            var viewModel = new CreateStoreViewModel()
            {
                Store = store,
                ItemsInCart = await CountItemsInCart()
            };

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            Context.Add(store);
            await Context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Stores/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Store store = await Context.Stores.FindAsync(id);
            var viewModel = new StoreDetailsViewModel()
            {
                Store = store,
                ItemsInCart = await CountItemsInCart()
            };

            if (store == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }

        // POST: Stores/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Store store)
        {
            if (!ModelState.IsValid)
            {
                return View(new StoreDetailsViewModel {Store = store, ItemsInCart = await CountItemsInCart()});
            }

            try
            {
                Context.Update(store);
                await Context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StoreExists(store.Id))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction("ListStores", "Administration");
        }

        // GET: Stores/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Store store = await Context.Stores.FirstOrDefaultAsync(m => m.Id == id);
            var viewModel = new StoreDetailsViewModel()
            {
                Store = store, ItemsInCart = await CountItemsInCart()
            };
            if (store == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }

        // POST: Stores/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Store store = await Context.Stores.Include(s => s.Orders).FirstOrDefaultAsync(s => s.Id == id);
            if (store.Orders.Count > 0)
            {
                return RedirectToAction("ListStores", "Administration");
            }
            Context.Stores.Remove(store);
            await Context.SaveChangesAsync();

            return RedirectToAction("ListStores", "Administration");
        }

        private bool StoreExists(int id)
        {
            return Context.Stores.Any(e => e.Id == id);
        }
    }
}