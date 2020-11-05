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
using Microsoft.AspNetCore.Authorization;

namespace GamingStore.Controllers
{
    public class StoresController : Controller
    {
        private readonly StoreContext _context;

        public StoresController(StoreContext context)
        {
            _context = context;
        }

        // GET: Stores
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var stores = await _context.Stores.ToListAsync();
            var uniqueCities = GetUniqueCities(stores);
            var openStores = GetOpenStores(stores);


            var viewModel = new StoresCitiesViewModel
            {
                Stores = stores,
                CitiesWithStores = uniqueCities.ToArray(),
                OpenStores = openStores
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
            var stores = await _context.Stores.ToListAsync();
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
                Name = "",
                City = null,
                IsOpen = false,
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

            Store store = await _context.Stores.FirstOrDefaultAsync(m => m.Id == id);
            if (store == null)
            {
                return NotFound();
            }

            return View(store);
        }

        // GET: Stores/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
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
            if (!ModelState.IsValid)
            {
                return View(store);
            }

            _context.Add(store);
            await _context.SaveChangesAsync();
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

            Store store = await _context.Stores.FindAsync(id);

            if (store == null)
            {
                return NotFound();
            }

            return View(store);
        }

        // POST: Stores/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address,PhoneNumber,Email,OpeningHours")]
            Store store)
        {
            if (id != store.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(store);
            }

            try
            {
                _context.Update(store);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StoreExists(store.Id))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Stores/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Store store = await _context.Stores.FirstOrDefaultAsync(m => m.Id == id);

            if (store == null)
            {
                return NotFound();
            }

            return View(store);
        }

        // POST: Stores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Store store = await _context.Stores.FindAsync(id);
            _context.Stores.Remove(store);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool StoreExists(int id)
        {
            return _context.Stores.Any(e => e.Id == id);
        }
    }
}