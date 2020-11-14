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

namespace GamingStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdministrationController : BaseController
    {
        public AdministrationController(UserManager<Customer> userManager, StoreContext context, RoleManager<IdentityRole> roleManager, SignInManager<Customer> signInManager) : base(userManager, context, roleManager, signInManager)
        {
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            Customer user = await UserManager.FindByIdAsync(id);

            if (user == null)
            {
                return Error(id, "user");
            }

            // GetRolesAsync returns the list of user Roles
            IList<string> userRoles = await UserManager.GetRolesAsync(user);

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

            var viewModel = new IndexViewModel()
            {
                Customers = Context.Customers,
                Items = Context.Items,
                Stores = Context.Stores,
                Orders = Context.Orders.Include(o => o.Customer).Include(o => o.Store),
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
                    var categoryName = orderItem.Item.Category.ToString();
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

        /*
        public async Task<IActionResult> GenerateStoreRevenuePieChart()
        {
            var orders = await Context.Orders.Include(o => o.Payment).Include(o => o.Store).ToListAsync();
            createMonthlyPurchasesGraphData(orders);

            return View("Statistics/GenerateStoreRevenuePieChart");
        }*/

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
                return Error(model.Id, "user");
            }

            user.Email = model.Email;
            user.UserName = model.UserName;
            model.ItemsInCart = await CountItemsInCart();
            IdentityResult result = await UserManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction("ListUsers");
            }

            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

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
                return Error(id, "role");
            }

            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name,
                ItemsInCart = await CountItemsInCart()
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
                return Error(model.Id, "role");
            }

            role.Name = model.RoleName;

            // Update the Role using UpdateAsync
            IdentityResult result = await RoleManager.UpdateAsync(role);

            if (result.Succeeded)
            {
                return RedirectToAction("ListRoles");
            }

            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            model.ItemsInCart = await CountItemsInCart();

            return View(model);
        }

        public JsonResult ListUsersBySearch(string searchUserString)
        {
            var users = UserManager.Users;

            if (!string.IsNullOrEmpty(searchUserString))
            {
                users = users.Where(user => user.Email.Contains(searchUserString));
            }

            var jsonResult = new JsonResult(users.ToList());
            return jsonResult;
        }

        public async Task<IActionResult> ListUsers()
        {
            var users = await UserManager.Users.ToListAsync();

            var viewModel = new ListUsersViewModels()
            {
                Users = users,
                ItemsInCart = await CountItemsInCart()
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
                ItemsInCart = await CountItemsInCart()
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ListItems()
        {
            List<Item> items = await Context.Items.ToListAsync();
            var viewModel = new ListItemsViewModel()
            {
                Items = items,
                ItemsInCart = await CountItemsInCart()
            };
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ListOrders()
        {
            List<Order> orders = await Context.Orders.Include(order => order.Customer).Include(order => order.Payment).ToListAsync();

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
                return Error(roleId, "role");
            }

            var viewModel = new ListUserRoleViewModel()
            {
                ItemsInCart = await CountItemsInCart(),
                List = new List<UserRoleViewModel>()
            };

            foreach (Customer user in UserManager.Users.ToList())
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.Email,
                    Email = user.Email,
                    ItemsInCart = await CountItemsInCart()
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
                return Error(roleId, "role");
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

                return RedirectToAction("EditRole", new {Id = roleId});
            }

            return RedirectToAction("EditRole", new {Id = roleId});
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            // Todo: change from deleteUser to disable user.
            Customer user = await UserManager.FindByIdAsync(id);

            if (user == null)
            {
                return Error(id, "user");
            }

            IdentityResult result = await UserManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction("ListUsers");
            }

            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View("ListUsers");
        }

        private IActionResult Error(string id, string type)
        {
            ViewBag.ErrorMessage = $"{type} with Id = {id} cannot be found";
            return View("NotFound");
        }
    }
}