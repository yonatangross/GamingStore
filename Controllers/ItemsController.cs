using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GamingStore.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GamingStore.Data;
using GamingStore.Models;
using GamingStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace GamingStore.Controllers
{
    public class ItemsController : Controller
    {
        private readonly StoreContext _context;
        private readonly UserManager<Customer> _userManager;
        private readonly IHostingEnvironment _hostingEnvironment;

        public ItemsController(StoreContext context, UserManager<Customer> userManager, IHostingEnvironment hostingEnvironment)
        {
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
            _context = context;
        }

        private Task<Customer> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        // GET: Items
        public async Task<IActionResult> Index(string category)
        {
            if (string.IsNullOrEmpty(category))
            {
                return View(await _context.Items.ToListAsync());
            }

            List<Item> items = new List<Item>();

            foreach (var item in _context.Items)
            {
                if (item.Category.ToString() == category)
                {
                    items.Add(item);
                }
            }
         
            return View(items);
        }

        // GET: Items/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Item item = await _context.Items.FirstOrDefaultAsync(m => m.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // GET: Items/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            Category[] allCategories = _context.Items.Select(item => item.Category).Distinct().ToArray();
            string categoriesString = $"[\"{string.Join("\",\"", allCategories)}\"]";

            return View(new CreateItemViewModel
            {
                Categories = allCategories
            });
        }

        // POST: Items/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateItemViewModel model)
        {
            var directoryName = model.Item.Title.Trim().Replace(" ", string.Empty);
            var uploadFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images", "items", directoryName);
            
            // do other validations on your model as needed
            if (model.File1 != null)
            {
                Directory.CreateDirectory(uploadFolder);
                const string uniqueFileName = "1.jpg";
                var filePath = Path.Combine(uploadFolder, uniqueFileName);
                await model.File1.CopyToAsync(new FileStream(filePath, FileMode.Create));

                model.Item.ImageUrl = $"images/items/{directoryName}";
            }

            if (model.File2 != null)
            {
                const string uniqueFileName = "2.jpg";
                var filePath = Path.Combine(uploadFolder, uniqueFileName);
                await model.File2.CopyToAsync(new FileStream(filePath, FileMode.Create));
            }

            if (model.File3 != null)
            {
                const string uniqueFileName = "3.jpg";
                var filePath = Path.Combine(uploadFolder, uniqueFileName);
                await model.File3.CopyToAsync(new FileStream(filePath, FileMode.Create));
            }

            await _context.Items.AddAsync(model.Item);
            await _context.SaveChangesAsync();

            return RedirectToAction("ListItems", "Administration");
        }



        // GET: Items/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Manufacturer,Price,Category,PropertiesList")] Item item)
        {
            if (id != item.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemExists(item.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }

        // GET: Items/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Items.FirstOrDefaultAsync(m => m.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Item item = await _context.Items.FindAsync(id);
            _context.Items.Remove(item);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool ItemExists(int id)
        {
            return _context.Items.Any(e => e.Id == id);
        }

        [Authorize]
        public async Task<IActionResult> AddToCart(int id, int quantity = 1)
        {
            try
            {
                Customer customer = await GetCurrentUserAsync();
                Cart cart = await _context.Carts.FirstOrDefaultAsync(c => c.CustomerId == customer.Id);

                if (cart == null || cart.ItemId != id)
                {
                    cart = new Cart()
                    {
                        ItemId = id,
                        Quantity = quantity,
                        CustomerId = customer.Id
                    };

                    _context.Carts.Add(cart);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

                cart.Quantity++;
                _context.Update(cart);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
