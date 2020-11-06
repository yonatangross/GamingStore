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
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
        public async Task<IActionResult> Index(string category, string manufacture, string priceRange)
        {

            var items = await _context.Items.ToListAsync();

            var categories = items.Select(i => i.Category).Distinct().ToList();
            var manufactures = new List<string>();

            if (!string.IsNullOrWhiteSpace(category))
            {
                items = items.Where(item => item.Category.ToString() == category).ToList();
                manufactures = items.Select(i => i.Manufacturer).Distinct().ToList();
            }

            if (!string.IsNullOrWhiteSpace(manufacture))
            {
                items = items.Where(item => item.Manufacturer == manufacture).ToList();
            }

            var endPrice = items.Select(i => i.Price).Max();
            if (!string.IsNullOrWhiteSpace(priceRange))
            {
                if (priceRange.Contains("0-25"))
                    items = items.Where(item => item.Price >= 0 && item.Price <= 25).ToList();
                if (priceRange.Contains("26-50"))
                    items = items.Where(item => item.Price >= 26 && item.Price <= 50).ToList();
                if (priceRange.Contains("51-99"))
                    items = items.Where(item => item.Price >= 51 && item.Price <= 99).ToList();
                if (priceRange.Contains("100+"))
                    items = items.Where(item => item.Price >= 100 && item.Price <= int.MaxValue).ToList();


                //items = items.Where(item => item.Manufacturer == manufacture).ToList();
            }

            var viewModel = new GetItemsViewModel()
            {
                Categories = categories,
                Manufactures = manufactures,
                Items = items.ToArray(),
                ItemsFilter = new ItemsFilter()
                {
                    Category = category,
                    Manufacture = manufacture,
                    PriceRange = priceRange
                }
            };

            return View(viewModel);
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

            return View(new CreateEditItemViewModel
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
        public async Task<IActionResult> Create(CreateEditItemViewModel model)
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
            Category[] allCategories = _context.Items.Select(i => i.Category).Distinct().ToArray();

            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Items.FindAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            var viewModel = new CreateEditItemViewModel()
            {
                Item = item,
                Categories = allCategories
            };

            return View(viewModel);
        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(CreateEditItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var itemOnDb = await _context.Items.FirstOrDefaultAsync(i => i.Id == model.Item.Id);

                    if (model.Item.Title != itemOnDb.Title)
                    {
                        itemOnDb.Title = model.Item.Title;
                    }

                    if (model.Item.Description != itemOnDb.Description)
                    {
                        itemOnDb.Description = model.Item.Description;
                    }

                    if (model.Item.Manufacturer != itemOnDb.Manufacturer)
                    {
                        itemOnDb.Manufacturer = model.Item.Manufacturer;
                    }

                    if (model.Item.Price != itemOnDb.Price)
                    {
                        itemOnDb.Price = model.Item.Price;
                    }

                    if (model.Item.Category != itemOnDb.Category)
                    {
                        itemOnDb.Category = model.Item.Category;
                    }

                    _context.Update(itemOnDb);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemExists(model.Item.Id))
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

            Category[] allCategories = _context.Items.Select(i => i.Category).Distinct().ToArray();
            model.Categories = allCategories;

            return View(model);
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
