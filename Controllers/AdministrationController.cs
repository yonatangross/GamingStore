using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GamingStore.Models;
using GamingStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GamingStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<Customer> _userManager;

        public AdministrationController(RoleManager<IdentityRole> roleManager, UserManager<Customer> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            Customer user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return Error(id, "user");
            }

            // GetRolesAsync returns the list of user Roles
            IList<string> userRoles = await _userManager.GetRolesAsync(user);

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Roles = userRoles
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            Customer user = await _userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                return Error(model.Id, "user");
            }

            user.Email = model.Email;
            user.UserName = model.UserName;
            IdentityResult result = await _userManager.UpdateAsync(user);

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
        public IActionResult ListRoles()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }

        public JsonResult ListUsersBySearch(string searchUserString)
        {
            var users = from user in _userManager.Users select user;

            if (!string.IsNullOrEmpty(searchUserString))
            {
                users = users.Where(user => user.Email.Contains(searchUserString));
            }

            var jsonResult= new JsonResult(users.ToList());
            return jsonResult;
        }

        public async Task<IActionResult> ListUsers()
        {
            var users = _userManager.Users;

            Console.WriteLine("users was loaded from ListUsers");
            return View(await users.ToListAsync());
        }

        /// <summary>
        /// Role ID is passed from the URL to the action
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            // Find the role by Role ID
            IdentityRole role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                return Error(id, "role");
            }

            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };

            // Retrieve all the Users
            foreach (Customer user in _userManager.Users)
            {
                // If the user is in this role, add the username to
                // Users property of EditRoleViewModel. This model
                // object is then passed to the view for display
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }

            return View(model);
        }

        // This action responds to HttpPost and receives EditRoleViewModel
        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(model.Id);

            if (role == null)
            {
                return Error(model.Id, "role");
            }

            role.Name = model.RoleName;

            // Update the Role using UpdateAsync
            IdentityResult result = await _roleManager.UpdateAsync(role);

            if (result.Succeeded)
            {
                return RedirectToAction("ListRoles");
            }

            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.roleId = roleId;

            IdentityRole role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                return Error(roleId, "role");
            }

            var model = new List<UserRoleViewModel>();

            foreach (Customer user in _userManager.Users)
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.Email,
                    Email = user.Email,
                    IsSelected = await _userManager.IsInRoleAsync(user, role.Name)
                };

                model.Add(userRoleViewModel);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                return Error(roleId, "role");
            }

            for (var i = 0; i < model.Count; i++)
            {
                Customer user = await _userManager.FindByIdAsync(model[i].UserId);

                IdentityResult result;

                if (model[i].IsSelected && !(await _userManager.IsInRoleAsync(user, role.Name)))
                {
                    user.UserName = user.Email;
                    result = await _userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && await _userManager.IsInRoleAsync(user, role.Name))
                {
                    user.UserName = user.Email;
                    result = await _userManager.RemoveFromRoleAsync(user, role.Name);
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
            Customer user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return Error(id, "user");
            }

            IdentityResult result = await _userManager.DeleteAsync(user);

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