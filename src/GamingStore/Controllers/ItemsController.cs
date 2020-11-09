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
using GamingStore.Services;
using GamingStore.Services.Twitter;
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

        public ItemsController(StoreContext context, UserManager<Customer> userManager,
            IHostingEnvironment hostingEnvironment)
        {
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
            _context = context;
        }

        private Task<Customer> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        // GET: Items
        public async Task<IActionResult> Index(string category, string manufacture, int? priceMin, int? priceMax,
            string keywords, SortByFilter sortBy = SortByFilter.NewestArrivals)
        {
            List<Item> items = await _context.Items.ToListAsync();

            List<Category> categories = items.Select(i => i.Category).Distinct().ToList();
            var manufactures = new List<string>();

            if (!string.IsNullOrWhiteSpace(category) && category != "All Categories")
            {
                items = items.Where(item => item.Category.ToString() == category).ToList();
            }

            if (!string.IsNullOrWhiteSpace(manufacture))
            {
                items = items.Where(item => item.Manufacturer == manufacture).ToList();
            }

            if (priceMin > 0)
            {
                items = items.Where(item => item.Price >= priceMin).ToList();
            }

            if (priceMax > 0)
            {
                items = items.Where(item => item.Price <= priceMax).ToList();
            }

            if (!string.IsNullOrWhiteSpace(keywords))
            {
                items = items.Where(item =>
                    item.Title.ToLower().Contains(keywords.ToLower()) ||
                    item.Manufacturer.ToLower().Contains(keywords.ToLower())).ToList();
            }

            items = sortBy switch
            {
                SortByFilter.NewestArrivals => items.OrderByDescending(i => i.Id).ToList(),
                SortByFilter.PriceLowToHigh => items.OrderBy(i => i.Price).ToList(),
                SortByFilter.PriceHighToLow => items.OrderByDescending(i => i.Price).ToList(),
                _ => throw new ArgumentOutOfRangeException(nameof(sortBy), sortBy, null)
            };
            manufactures = items.Select(i => i.Manufacturer).Distinct().ToList();
            var viewModel = new GetItemsViewModel()
            {
                Categories = categories,
                Manufactures = manufactures,
                Items = items.ToArray(),
                ItemsFilter = new ItemsFilter()
                {
                    Category = category,
                    Manufacture = manufacture,
                    SortBy = sortBy,
                    PriceMin = priceMin,
                    PriceMax = priceMax,
                    Keywords = keywords
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
            Customer user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return View(item);
            }

            var userIntId = GetCurrentUserAsync().Result.CustomerNumber;
            var relatedItem = new RelatedItem(userIntId, item.Id);
            bool relatedItems =
                _context.RelatedItems.Any(ri => ri.ItemId == item.Id && ri.CustomerNumber == user.CustomerNumber);
            if (!relatedItems) await _context.RelatedItems.AddAsync(relatedItem);

            await _context.SaveChangesAsync();

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
                var fileStream = new FileStream(filePath, FileMode.Create);
                await model.File1.CopyToAsync(fileStream);
                fileStream.Close();

                model.Item.ImageUrl = $"images/items/{directoryName}";
            }

            if (model.File2 != null)
            {
                const string uniqueFileName = "2.jpg";
                var filePath = Path.Combine(uploadFolder, uniqueFileName);
                var fileStream = new FileStream(filePath, FileMode.Create);
                await model.File2.CopyToAsync(fileStream);
                fileStream.Close();
            }

            if (model.File3 != null)
            {
                const string uniqueFileName = "3.jpg";
                var filePath = Path.Combine(uploadFolder, uniqueFileName);
                var fileStream = new FileStream(filePath, FileMode.Create);
                await model.File3.CopyToAsync(fileStream);
                fileStream.Close();
            }

            await _context.Items.AddAsync(model.Item);
            await _context.SaveChangesAsync();


            #region TwitterPost

            if (model.PublishItemFlag)
            {
              
                PublishTweet( model.Item, uploadFolder);
            }

            #endregion

            return RedirectToAction("ListItems", "Administration");
        }

        private void PublishTweet( Item item, string itemImagesPath)
        {
            //todo: twitter async
            string ConsumerKey = "CVXDTooXKg62g4qq6Ww0QujEV",
                ConsumerKeySecret = "mz081uiCZwY6rogFahNqfz2DBfA5CaKoLWXjRqhDSvEd1Z1HTf",
                AccessToken = "1324135248574238726-Kyrpj3MY96pLyHYbdSuXcUN4Claic4",
                AccessTokenSecret = "vxBHC98lIdimimk79Ok53e8iskW8NN74SpbO5voIa0PrD";

            var twitter = new Twitter(ConsumerKey, ConsumerKeySecret, AccessToken, AccessTokenSecret);

            var fullImageUrl = itemImagesPath + "\\1.jpg";
            //var fullImageUrl = "C:\\Users\\Yonatan\\Source\\Repos\\yonatangross\\GamingStore\\src\\GamingStore\\wwwroot\\images\\user.png";

            var tweet ="Gaming Store now sells "+ item.Title+ " only on "+item.Price+"$";
            try
            {

                string response = twitter.PublishToTwitter(tweet, fullImageUrl);
                Console.WriteLine(response);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
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