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
using GamingStore.Contracts.ML;
using GamingStore.Data;
using Microsoft.AspNetCore.Mvc;
using GamingStore.Models;
using GamingStore.Services.Email;
using GamingStore.Services.Email.Interfaces;
using GamingStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GamingStore.Controllers
{
    [AllowAnonymous]
    public class HomeController : BaseController
    {
        private readonly IEmailSender _emailSender;

        public HomeController(IEmailSender emailSender, UserManager<Customer> userManager, StoreContext context, RoleManager<IdentityRole> roleManager) : base( userManager,  context, roleManager)
        {
            _emailSender = emailSender;
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

        public IActionResult ContactUs()
        {
            //todo: change to viewmodel but move first to mail service
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ContactUs(Mail form)
        {
            //todo: change to viewmodel but move first to mail service

            if (!ModelState.IsValid)
            {
                return View();
            }

            //prepare email
            const string toAddress = "gamingstoreproject+form@gmail.com";
            string subject = $"ContactUs inquiry from {form.Name}";
            var message = new StringBuilder();
            message.Append($"Name: {form.Name}\n");
            message.Append($"Email: {form.Email}\n");
            message.Append($"Telephone: {form.Telephone}\n\n");
            message.Append(form.Message);

            //start email Thread
            await Task.Run(() => { _emailSender.SendEmailAsync(toAddress, subject, message.ToString()); }).ConfigureAwait(false);

            return View();
        }
    }
}