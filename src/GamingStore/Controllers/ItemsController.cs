using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using GamingStore.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GamingStore.Data;
using GamingStore.Extensions;
using GamingStore.Models;
using GamingStore.Services.Twitter;
using GamingStore.ViewModels.Items;
using LoggerService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Vereyon.Web;

namespace GamingStore.Controllers
{
    public class ItemsController : BaseController
    {
        private readonly IHostingEnvironment _hostingEnvironment; 
        private readonly IFlashMessage _flashMessage;
        private readonly ILoggerManager _logger;

        public ItemsController(IHostingEnvironment hostingEnvironment, UserManager<Customer> userManager, StoreContext context, RoleManager<IdentityRole> roleManager, SignInManager<Customer> signInManager, IFlashMessage flashMessage, ILoggerManager logger)
            : base(userManager, context, roleManager, signInManager)
        {
            _hostingEnvironment = hostingEnvironment;
            _flashMessage = flashMessage;
            _logger = logger;
        }
        
        // GET: Items
        public async Task<IActionResult> Index(string category, string manufacturer, int? priceMin, int? priceMax, string keywords, int? pageNumber, SortByFilter sortBy = SortByFilter.NewestArrivals)
        {
            const int pageSize = 16;
            IQueryable<Item> items = Context.Items.Where(i => i.Active);

            List<string> categories = items.Select(i => i.Category).Distinct().ToList();

            if (!string.IsNullOrWhiteSpace(category) && category != "All Categories")
            {
                items = items.Where(item => item.Category == category);

            }

            if (!string.IsNullOrWhiteSpace(manufacturer))
            {
                items = items.Where(item => item.Manufacturer == manufacturer);
            }

            if (priceMin > 0)
            {
                items = items.Where(item => item.Price >= priceMin);
            }

            if (priceMax > 0)
            {
                items = items.Where(item => item.Price <= priceMax);
            }

            if (!string.IsNullOrWhiteSpace(keywords))
            {
                items = items.Where(item =>
                    item.Title.ToLower().Contains(keywords.ToLower()) ||
                    item.Manufacturer.ToLower().Contains(keywords.ToLower()));
            }

            items = sortBy switch
            {
                SortByFilter.NewestArrivals => items.OrderByDescending(i => i.Id),
                SortByFilter.PriceLowToHigh => items.OrderBy(i => i.Price),
                SortByFilter.PriceHighToLow => items.OrderByDescending(i => i.Price),
                _ => throw new ArgumentOutOfRangeException(nameof(sortBy), sortBy, null)
            };
            
            List<string> manufactures = items.Select(i => i.Manufacturer).Distinct().ToList();
            
            PaginatedList<Item> paginatedList = await PaginatedList<Item>.CreateAsync(items.AsNoTracking(), pageNumber ?? 1, pageSize);
            var viewModel = new GetItemsViewModel()
            {
                Categories = categories,
                Manufactures = manufactures,
                Items = items.ToArray(),
                PaginatedItems = paginatedList,
                ItemsFilter = new ItemsFilter()
                {
                    Category = category,
                    Manufacturer = manufacturer,
                    SortBy = sortBy,
                    PriceMin = priceMin,
                    PriceMax = priceMax,
                    Keywords = keywords
                },
                ItemsInCart = await CountItemsInCart()
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

            Item item = await Context.Items.FirstOrDefaultAsync(m => m.Id == id);
            Customer user = await UserManager.GetUserAsync(User);

            var viewModel = new ItemViewModel()
            {
                Item = item,
                ItemsInCart = await CountItemsInCart()
            };

            if (user == null)
            {
                return View(viewModel);
            }

            int userIntId = GetCurrentUserAsync().Result.CustomerNumber;
            var relatedItem = new RelatedItem(userIntId, item.Id);
            bool relatedItems = Context.RelatedItems.Any(ri => ri.ItemId == item.Id && ri.CustomerNumber == user.CustomerNumber);

            if (!relatedItems)
            {
                await Context.RelatedItems.AddAsync(relatedItem);
            }

            await Context.SaveChangesAsync();

            return View(viewModel);
        }

        // GET: Items/Create
        [Authorize(Roles = "Admin,Viewer")]
        public async Task<IActionResult> Create()
        {
            string[] allCategories = Context.Items.Select(item => item.Category).Distinct().ToArray();

            return View(new CreateEditItemViewModel
            {
                Categories = allCategories,
            });
        }

        // POST: Items/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin,Viewer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEditItemViewModel model)
        {
            try
            {
                model.Item.Manufacturer = model.Item.Manufacturer.Trim().FirstCharToUpper();
                string uploadFolder = await UploadImages(model);

                await Context.Items.AddAsync(model.Item);
                await Context.SaveChangesAsync();

                #region TwitterPost

                if (model.PublishItemFlag)
                {

                    PublishTweet(model.Item, uploadFolder);
                }

                #endregion

                _flashMessage.Confirmation($"Product '{model.Item.Title}' created with price of ${model.Item.Price}");
            }
            catch (Exception e)
            {
                _flashMessage.Danger("Product could not be created");
                _logger.LogError($"Product# '{model.Item.Title}' could not be created, ex: {e}");
            }

            return RedirectToAction("ListItems", "Administration");
        }

        private async Task<string> UploadImages(CreateEditItemViewModel model)
        {
            string directoryName = model.Item.Title.Trim().Replace(" ", string.Empty);
            string uploadFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images", "items", directoryName);

            if (model.File1 != null)
            {
                await CopyImage(model, uploadFolder, 1);
                model.Item.ImageUrl = $"images/items/{directoryName}";
            }

            if (model.File2 != null)
            {
                await CopyImage(model, uploadFolder, 1);
            }

            if (model.File3 != null)
            {
                await CopyImage(model, uploadFolder, 1);
            }

            return uploadFolder;
        }

        private static async Task CopyImage(CreateEditItemViewModel model, string uploadFolder, int imageNumber)
        {
            Directory.CreateDirectory(uploadFolder);
            string uniqueFileName = $"{imageNumber}.jpg";
            string filePath = Path.Combine(uploadFolder, uniqueFileName);
            var fileStream = new FileStream(filePath, FileMode.Create);
            await model.File1.CopyToAsync(fileStream);
            fileStream.Close();
        }

        private void PublishTweet(Item item, string itemImagesPath)
        {
            const string consumerKey = "CVXDTooXKg62g4qq6Ww0QujEV";
            const string consumerKeySecret = "mz081uiCZwY6rogFahNqfz2DBfA5CaKoLWXjRqhDSvEd1Z1HTf";
            const string accessToken = "1324135248574238726-Kyrpj3MY96pLyHYbdSuXcUN4Claic4";
            const string accessTokenSecret = "vxBHC98lIdimimk79Ok53e8iskW8NN74SpbO5voIa0PrD";

            var twitter = new Twitter(consumerKey, consumerKeySecret, accessToken, accessTokenSecret);

            string fullImageUrl = $"{itemImagesPath}\\1.jpg";
            string tweet = $"Gaming Store now sells {item.Title} only on {item.Price}$";

            try
            {

                string response = twitter.PublishToTwitter(tweet, fullImageUrl);
                Console.WriteLine(response);
            }
            catch (Exception e)
            {
                _logger.LogError($"Item could not be published to twitter, ex: {e}");
            }
        }

        // GET: Items/Edit/5
        [Authorize(Roles = "Admin,Viewer")]
        public async Task<IActionResult> Edit(int? id)
        {
            string[] allCategories = Context.Items.Select(i => i.Category).Distinct().ToArray();

            if (id == null)
            {
                return NotFound();
            }

            Item item = await Context.Items.FindAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            item.Manufacturer = item.Manufacturer.Trim().FirstCharToUpper();

            var viewModel = new CreateEditItemViewModel()
            {
                Item = item,
                Categories = allCategories,
            };

            return View(viewModel);
        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Viewer")]
        public async Task<IActionResult> Edit(CreateEditItemViewModel model)
        {
            Item itemOnDb = await Context.Items.FirstOrDefaultAsync(i => i.Id == model.Item.Id);

            if (itemOnDb == null)
            {
                _flashMessage.Danger("Item could not be found on DB");

                return RedirectToAction("ListItems", "Administration");
            }


            var currentUser = await GetCurrentUserAsync();

            IList<string> userRoles = await UserManager.GetRolesAsync(currentUser);

            if (!userRoles.Any(r => r.Equals("Admin")))
            {
                _flashMessage.Danger("Your changes were not saved.\n You do not have the right permissions to edit items.");
                return RedirectToAction("ListItems", "Administration");
            }

            try
            {
                itemOnDb.Title = model.Item.Title;
                itemOnDb.Description = model.Item.Description;
                itemOnDb.Manufacturer = model.Item.Manufacturer.Trim().FirstCharToUpper();
                itemOnDb.Price = model.Item.Price;
                itemOnDb.Category = model.Item.Category;
                itemOnDb.Active = model.Item.Active;

                await UploadImages(model);

                Context.Update(itemOnDb);
                await Context.SaveChangesAsync();
                _flashMessage.Confirmation("Item information has been updated");
            }
            catch (Exception e)
            {
                _flashMessage.Danger("Product could not be updated");
                _logger.LogError($"Product# '{model.Item.Id}' could not be updated, ex: {e}");
            }

            return RedirectToAction("ListItems", "Administration");
        }

        [Authorize]
        public async Task<IActionResult> AddToCart(int itemId, int quantity = 1)
        {
            try
            {
                if (quantity == 0)
                {
                    return RedirectToAction(nameof(Index));
                }

                Customer customer = await GetCurrentUserAsync();

                if (customer == null) // Not Log In
                {
                    return NotFound();
                }

                Cart cart = await Context.Carts.FirstOrDefaultAsync(c => c.CustomerId == customer.Id && c.ItemId == itemId);

                if (cart == null)
                {
                    cart = new Cart()
                    {
                        CustomerId = customer.Id,
                        ItemId = itemId,
                        Quantity = quantity
                    };

                    await Context.Carts.AddAsync(cart);
                    await Context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }

                cart.Quantity = cart.Quantity + quantity;
                Context.Update(cart);
                await Context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogError($"Could not add an item to cart, ex: {e}");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}