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
        public AdministrationController(UserManager<Customer> userManager, StoreContext context,
            RoleManager<IdentityRole> roleManager) : base(userManager, context, roleManager)
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
        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> BarChart()
        {
            var orders = await Context.Orders.Include(o => o.Payment).Include(o => o.Store).ToListAsync();
            CreateBarChartData(orders);

            return View("Statistics/BarChart");
        }

        [HttpGet]
        public async Task<IActionResult> PieChartCategory()
        {
            var orders = await Context.Orders.Include(o => o.OrderItems).ThenInclude(oi => oi.Item).ToListAsync();
            CreatePieChartByCategoryData(orders);

            return View("Statistics/PieChartCategory");
        }


        public async Task<IActionResult> PieChart()
        {
            var orders = await Context.Orders.Include(o => o.Payment).Include(o => o.Store).ToListAsync();
            createMonthlyPurchasesGraph(orders);

            return View("Statistics/PieChart");
        }

        private void createMonthlyPurchasesGraph(List<Order> orders)
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
            string pieChartDataPath = "data\\PieChart.json";
            var fileDir = $@"{Directory.GetCurrentDirectory()}\wwwroot\{pieChartDataPath}";
            System.IO.File.WriteAllText(fileDir, serializeObject);
        }

        private void CreatePieChartByCategoryData(List<Order> orders)
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
                        PieChartFormat pieChartFormat =
                            purchaseByCategoryList.FirstOrDefault(d => d.Name == categoryName);
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
            string pieChartDataPath = "data\\PieChartCategory.json";
            var fileDir = $@"{Directory.GetCurrentDirectory()}\wwwroot\{pieChartDataPath}";
            System.IO.File.WriteAllText(fileDir, serializeObject);
        }

        private static void CreateBarChartData(List<Order> orders)
        {
            var orderMonthlyList = new List<BarChartFormat>();
            orders.Sort((x, y) => x.OrderDate.CompareTo(y.OrderDate));
            orders.Reverse();

            foreach (var order in orders)
            {
                var orderDate = order.OrderDate.Date.ToString("Y");
                var itemsCost = order.Payment.ItemsCost;

                if (orderMonthlyList.Any(d => d.Date == orderDate))
                {
                    BarChartFormat barChartFormat = orderMonthlyList.FirstOrDefault(d => d.Date == orderDate);
                    if (barChartFormat != null)
                    {
                        barChartFormat.Value += itemsCost;
                    }
                }
                else
                {
                    orderMonthlyList.Add(new BarChartFormat()
                    {
                        Date = orderDate,
                        Value = itemsCost
                    });
                }
            }

            var serializeObject = JsonConvert.SerializeObject(orderMonthlyList, Formatting.Indented);

            //write string to file
            string barChartDataPath = "data\\BarChartData.json";
            var fileDir = $@"{Directory.GetCurrentDirectory()}\wwwroot\{barChartDataPath}";
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
            List<Order> orders = await Context.Orders.Include(order => order.Customer).Include(order => order.Payment)
                .ToListAsync();

            var viewModel = new ListOrdersViewModel()
            {
                Orders = orders,
                ItemsInCart = await CountItemsInCart()
            };



            return View(viewModel);
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
                ItemsInCart = await CountItemsInCart()
            };

            foreach (Customer user in UserManager.Users)
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.Email,
                    Email = user.Email,
                    IsSelected = await UserManager.IsInRoleAsync(user, role.Name),
                    ItemsInCart = await CountItemsInCart()
                };

                viewModel.List.Add(userRoleViewModel);
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
        {
            IdentityRole role = await RoleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                return Error(roleId, "role");
            }

            for (var i = 0; i < model.Count; i++)
            {
                Customer user = await UserManager.FindByIdAsync(model[i].UserId);

                IdentityResult result;

                if (model[i].IsSelected && !(await UserManager.IsInRoleAsync(user, role.Name)))
                {
                    user.UserName = user.Email;
                    result = await UserManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && await UserManager.IsInRoleAsync(user, role.Name))
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

                if (i < (model.Count - 1))
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