using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GamingStore.Contracts;
using GamingStore.Data;
using GamingStore.Models;
using GamingStore.Models.Relationships;
using GamingStore.ViewModels;
using GamingStore.ViewModels.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Vereyon.Web;

namespace GamingStore.Controllers
{
    [Authorize(Roles = "Admin,Viewer")]
    public class AdministrationController : BaseController
    {
        private readonly IFlashMessage _flashMessage;

        public AdministrationController(UserManager<Customer> userManager, StoreContext context, RoleManager<IdentityRole> roleManager, SignInManager<Customer> signInManager, IFlashMessage flashMessage) 
            : base(userManager, context, roleManager, signInManager)
        {
            _flashMessage = flashMessage;
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            Customer user = await UserManager.FindByIdAsync(id);

            if (user == null)
            {
                _flashMessage.Danger("You can not edit a user that no longer exists");
                return RedirectToAction("ListUsers");
            }
            
            // GetRolesAsync returns the list of user Roles
            IList<string> userRoles = await UserManager.GetRolesAsync(user);
            if (!userRoles.Any(r => r.Equals("Admin")))
            {
                _flashMessage.Danger("Your changes were not saved.\nYou do not have the right permissions to edit a user.");
                return RedirectToAction("ListUsers");
            }

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Roles = userRoles,
                ItemsInCart = await CountItemsInCart()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var revenue = await CalcRevenue();
            var ordersNumber = await Context.Orders.CountAsync();
            var itemsNumber = await Context.Items.CountAsync();
            var clientsNumber = await Context.Customers.CountAsync();

            revenue = Math.Round(revenue, 2);
            CreateMonthlyRevenueBarChartData(await Context.Orders.Include(o => o.Payment).Include(o => o.Store).ToListAsync());
            CreateRevenueByCategoryPieChartData(await Context.Orders.Include(o => o.OrderItems).ThenInclude(oi => oi.Item).ToListAsync());

            List<Order> orders = await Context.Orders
                .Include(order => order.Customer)
                .Include(order => order.Payment)
                .Include(order => order.Store)
                .OrderByDescending(order => order.OrderDate)
                .ToListAsync();

            var viewModel = new IndexViewModel()
            {
                Customers = Context.Customers,
                Items = Context.Items,
                Stores = Context.Stores,
                Orders = orders,
                ItemsInCart = await CountItemsInCart(),
                WidgetsValues = new Dictionary<string, double>()
                {
                    {"Revenue", revenue}, {"Orders", ordersNumber}, {"Items", itemsNumber}, {"Clients", clientsNumber}
                }
            };

            return View(viewModel);
        }

        private async Task<double> CalcRevenue()
        {
            var orders = await Context.Orders.Include(o => o.Payment).ToListAsync();

            return orders.Sum(order => order.Payment.Total);
        }

        private void CreateRevenueByCategoryPieChartData(List<Order> orders)
        {
            var purchaseByCategoryList = new List<PieChartFormat>();

            foreach (var order in orders)
            {
                foreach (OrderItem orderItem in order.OrderItems)
                {
                    var categoryName = orderItem.Item.Category;
                    var itemsCost = orderItem.ItemsCount * orderItem.Item.Price;
                    if (purchaseByCategoryList.Any(d => d.Name == categoryName))
                    {
                        PieChartFormat pieChartFormat = purchaseByCategoryList.FirstOrDefault(d => d.Name == categoryName);
                        if (pieChartFormat != null)
                        {
                            pieChartFormat.Value += itemsCost;
                        }
                    }
                    else
                    {
                        purchaseByCategoryList.Add(new PieChartFormat()
                        {
                            Name = categoryName,
                            Value = itemsCost
                        });
                    }
                }
            }

            purchaseByCategoryList.Sort((a, b) => a.Value.CompareTo(b.Value));
            var serializeObject = JsonConvert.SerializeObject(purchaseByCategoryList, Formatting.Indented);

            //write string to file
            string pieChartDataPath = "data\\RevenueByCategoryPieChartData.json";
            var fileDir = $@"{Directory.GetCurrentDirectory()}\wwwroot\{pieChartDataPath}";
            System.IO.File.WriteAllText(fileDir, serializeObject);
        }

        private static void CreateMonthlyRevenueBarChartData(List<Order> orders)
        {
            orders.Sort((x, y) => x.OrderDate.CompareTo(y.OrderDate));

            var groupByCheck = from order in orders
                               group order by order.OrderDate.Date.ToString("Y")
                into dateGroup
                               select new BarChartFormat()
                               {
                                   Date = dateGroup.Key,
                                   Value = dateGroup.Sum(o => o.Payment.ItemsCost)
                               };

            var serializedGroupBy = JsonConvert.SerializeObject(groupByCheck, Formatting.Indented);

            //write string to file
            string barChartDataPath = "data\\MonthlyRevenueBarChartData.json";
            var fileDir = $@"{Directory.GetCurrentDirectory()}\wwwroot\{barChartDataPath}";
            System.IO.File.WriteAllText(fileDir, serializedGroupBy);
        }

        private void createStoresRevenueGraphData(List<Order> orders)
        {
            var orderMonthlyList = new List<PieChartFormat>();

            foreach (var order in orders)
            {
                var storeName = order.Store.Name.Replace("Store", "");
                var itemsCost = order.Payment.ItemsCost;

                if (orderMonthlyList.Any(d => d.Name == storeName))
                {
                    PieChartFormat pieChartFormat = orderMonthlyList.FirstOrDefault(d => d.Name == storeName);
                    if (pieChartFormat != null)
                    {
                        pieChartFormat.Value += itemsCost;
                    }
                }
                else
                {
                    orderMonthlyList.Add(new PieChartFormat()
                    {
                        Name = storeName,
                        Value = itemsCost
                    });
                }
            }

            orderMonthlyList.Sort((a, b) => a.Value.CompareTo(b.Value));

            var serializeObject = JsonConvert.SerializeObject(orderMonthlyList, Formatting.Indented);

            //write string to file
            string pieChartDataPath = "data\\StoresRevenuePieChart.json";
            var fileDir = $@"{Directory.GetCurrentDirectory()}\wwwroot\{pieChartDataPath}";
            System.IO.File.WriteAllText(fileDir, serializeObject);
        }

       
        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            Customer user = await UserManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                _flashMessage.Danger("You can not edit a user that no longer exists");
                return RedirectToAction("ListUsers");
            }

            // GetRolesAsync returns the list of user Roles
            IList<string> userRoles = await UserManager.GetRolesAsync(user);

            if (!userRoles.Any(r => r.Equals("Admin")))
            {
                _flashMessage.Danger("Your changes were not saved.\nYou do not have the right permissions to edit users.");
                return RedirectToAction("ListUsers");
            }

            user.Email = model.Email;
            user.UserName = model.UserName;
            model.ItemsInCart = await CountItemsInCart();
            IdentityResult result = await UserManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                _flashMessage.Confirmation("Edit user succeeded");
                return RedirectToAction("ListUsers");
            }

            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            _flashMessage.Danger("En error while trying to edit a user");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ListRoles()
        {
            IQueryable<IdentityRole> roles = RoleManager.Roles;

            var viewModel = new ListRolesViewModel()
            {
                Roles = roles,
                ItemsInCart = await CountItemsInCart()
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            IdentityRole role = await RoleManager.FindByIdAsync(id);

            if (role == null)
            {
                _flashMessage.Danger("You can not edit a role that no longer exists");
                return RedirectToAction("ListUsers");
            }

            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name,
            };

            // Retrieve all the Users
            foreach (Customer user in UserManager.Users)
            {
                /*If the user is in this role, add the username to
                Users property of EditRoleViewModel. This model
                object is then passed to the view for display*/
                if (await UserManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            IdentityRole role = await RoleManager.FindByIdAsync(model.Id);

            if (role == null)
            {
                _flashMessage.Danger("You can not edit a role that no longer exists");
                return RedirectToAction("ListRoles");
            }

            var currentUser = await GetCurrentUserAsync();
            IList<string> userRoles = await UserManager.GetRolesAsync(currentUser);

            if (!userRoles.Any(r => r.Equals("Admin")))
            {
                _flashMessage.Danger("Your changes were not saved.\n You do not have the right permissions to edit roles.");
                return RedirectToAction("ListRoles");
            }


            role.Name = model.RoleName;

            // Update the Role using UpdateAsync
            IdentityResult result = await RoleManager.UpdateAsync(role);

            if (result.Succeeded)
            {
                _flashMessage.Confirmation("Edit user succeeded");
                return RedirectToAction("ListRoles");
            }

            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            _flashMessage.Danger("En error while trying to edit a role");
            return View(model);
        }

        public JsonResult ListUsersBySearch(string searchUserString)
        {
            IQueryable<Customer> users = UserManager.Users;

            if (!string.IsNullOrEmpty(searchUserString))
            {
                users = users.Where(user => user.Email.Contains(searchUserString));
            }

            var jsonResult = new JsonResult(users.ToList());
            return jsonResult;
        }

        public async Task<IActionResult> ListUsers()
        {
            List<Customer> users = await UserManager.Users.ToListAsync();
            var currentUser = await GetCurrentUserAsync();
            IList<string> userRoles = await UserManager.GetRolesAsync(currentUser);

            var viewModel = new ListUsersViewModels
            {
                Users = users,
                CurrentUser = currentUser,
                CurrentUserRoles=userRoles
            };

            return View(viewModel);
        }


        [HttpGet]
        public async Task<IActionResult> ListStores()
        {
            List<Store> stores = await Context.Stores.Include(s => s.Orders).ToListAsync();
            var viewModel = new ListStoresViewModel()
            {
                Stores = stores,
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ListItems()
        {
            List<Item> items = await Context.Items.ToListAsync();
            
            var viewModel = new ListItemsViewModel()
            {
                Items = items
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ListOrders()
        {
            List<Order> orders = await Context.Orders
                .Include(order => order.Customer)
                .Include(order => order.Payment)
                .Include(order => order.Store)
                .OrderByDescending(order => order.OrderDate)
                .ToListAsync();

            var viewModel = new ListOrdersViewModel()
            {
                Orders = orders,
                ItemsInCart = await CountItemsInCart()
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.roleId = roleId;

            IdentityRole role = await RoleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                _flashMessage.Danger("You can not edit a role that no longer exists");
                return RedirectToAction("ListUsers");
            }

            var viewModel = new ListUserRoleViewModel()
            {
                List = new List<UserRoleViewModel>()
            };

            foreach (Customer user in UserManager.Users.ToList())
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.Email,
                    Email = user.Email,
                };

                if (await UserManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }

                viewModel.List.Add(userRoleViewModel);
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(ListUserRoleViewModel model, string roleId)
        {
            IdentityRole role = await RoleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                _flashMessage.Danger("You can not edit a role that no longer exists");
                return RedirectToAction("ListUsers");
            }

            var currentUser = await GetCurrentUserAsync();
            IList<string> userRoles = await UserManager.GetRolesAsync(currentUser);

            if (!userRoles.Any(r => r.Equals("Admin")))
            {
                _flashMessage.Danger("Your changes were not saved.\n You do not have the right permissions to edit user list in a role.");
                return RedirectToAction("ListUsers");
            }

            for (var i = 0; i < model.List.Count; i++)
            {
                Customer user = await UserManager.FindByIdAsync(model.List[i].UserId);

                IdentityResult result;

                if (model.List[i].IsSelected && !(await UserManager.IsInRoleAsync(user, role.Name)))
                {
                    user.UserName = user.Email;
                    result = await UserManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model.List[i].IsSelected && await UserManager.IsInRoleAsync(user, role.Name))
                {
                    user.UserName = user.Email;
                    result = await UserManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }

                if (!result.Succeeded)
                {
                    continue;
                }

                if (i < (model.List.Count - 1))
                {
                    continue;
                }

                _flashMessage.Confirmation("Edit user's role succeeded");
                return RedirectToAction("EditRole", new {Id = roleId});
            }

            _flashMessage.Confirmation("Edit user's role succeeded");
            return RedirectToAction("EditRole", new {Id = roleId});
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            Customer user = await UserManager.FindByIdAsync(id);

            if (user == null)
            {
                _flashMessage.Danger("You can not delete a user that no longer exists");
                return RedirectToAction("ListUsers");
            }

            var currentUser = await GetCurrentUserAsync();

            IList<string> userRoles = await UserManager.GetRolesAsync(currentUser);

            if (!userRoles.Any(r => r.Equals("Admin")))
            {
                _flashMessage.Danger("Your changes were not saved.\n You do not have the right permissions to delete users.");
                return RedirectToAction("ListUsers");
            }

            if (user.Id == currentUser.Id)
            {
                _flashMessage.Danger("You can not delete your own user.");
                return RedirectToAction("ListUsers");
            }

            IdentityResult result = await UserManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                _flashMessage.Confirmation("Delete user succeeded");
                return RedirectToAction("ListUsers");
            }

            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View("ListUsers");
        }
    }
}