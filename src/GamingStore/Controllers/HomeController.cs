using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GamingStore.Contracts;
using GamingStore.Contracts.ML;
using GamingStore.Data;
using Microsoft.AspNetCore.Mvc;
using GamingStore.Models;
using GamingStore.Services.Email;
using GamingStore.Services.Email.Interfaces;
using GamingStore.ViewModels;
using GamingStore.ViewModels.Home;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Vereyon.Web;

namespace GamingStore.Controllers
{
    [AllowAnonymous]
    public class HomeController : BaseController
    {
        private readonly IEmailSender _emailSender;
        private readonly IFlashMessage _flashMessage;

        public HomeController(IEmailSender emailSender, UserManager<Customer> userManager, StoreContext context, RoleManager<IdentityRole> roleManager, SignInManager<Customer> signInManager,IFlashMessage flashMessage)
            : base(userManager, context, roleManager, signInManager)
        {
            _emailSender = emailSender;
            _flashMessage = flashMessage;
        }

        public async Task<IActionResult> Index()
        {
            List<ItemsCustomers> itemsCustomersList = Context.RelatedItems.Select(item => new ItemsCustomers() { CustomerNumber = item.CustomerNumber, ItemId = item.ItemId }).ToList();
            Customer user = await GetCurrentUserAsync();

            var viewModel = new HomeViewModel()
            {
                Items = new List<Item>(),
                ItemsInCart = await CountItemsInCart()
            };

            if (user == null)
            {
                return View(viewModel);
            }

            var mlRequest = new Request()
            {
                ItemCustomersList = itemsCustomersList,
                CustomerNumber = user.CustomerNumber,
                AllItemsIds = Context.Items.Select(i => i.Id).Distinct().ToList(),
            };

            try
            {
                Dictionary<int, double> itemsScores = await MachineLearning.ML.Run(mlRequest);
                IEnumerable<KeyValuePair<int, double>> topItems = itemsScores.OrderByDescending(pair => pair.Value).Take(6);
                IQueryable<Item> items = Context.Items.Take(int.MaxValue);
                List<Item> itemsList = (from keyValuePair in topItems from item in items where keyValuePair.Key == item.Id select item).ToList();

                viewModel.Items = itemsList;

                return View(viewModel);
            }
            catch
            {
                //ignored
            }

            return View(viewModel);
        }

        public async Task<IActionResult> ContactUs()
        {
            Customer customer = await GetCurrentUserAsync();

            if (customer != null)
            {
                return View(new ContactViewModel
                {
                    Name = $"{customer.FirstName} {customer.LastName}",
                    Email = customer.Email,
                    PhoneNumber = customer.PhoneNumber,
                    ItemsInCart = await CountItemsInCart()
                });
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmContactUs(Mail model)
        {
            Customer customer = await GetCurrentUserAsync();
            string customerId = customer != null ? customer.Id : "Not Logged In";

            //prepare email
            const string toAddress = "gamingstoreproject+form@gmail.com";
            string subject = $"ContactUs inquiry from {model.Name}";
            var message = new StringBuilder();
            message.Append($"Name: {model.Name}{Environment.NewLine}");
            message.Append($"Email: {model.Email}{Environment.NewLine}");
            message.Append($"Telephone: {model.PhoneNumber}{Environment.NewLine}");
            message.Append($"CustomerId: '{customerId}'{Environment.NewLine}");
            message.Append(model.Message);

            //start email Thread
            await Task.Run(() => { _emailSender.SendEmailAsync(toAddress, subject, message.ToString()); }).ConfigureAwait(false);
            _flashMessage.Confirmation("Message Sent");

            return RedirectToAction(nameof(ContactUs));
        }
    }
}