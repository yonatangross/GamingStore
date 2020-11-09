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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace GamingStore.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly StoreContext _context;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<Customer> _userManager;

        private Task<Customer> GetCurrentUserAsync() => _userManager.GetUserAsync(User);

        public HomeController(IEmailSender emailSender, StoreContext context, UserManager<Customer> userManager)
        {
            _emailSender = emailSender;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            List<ItemsCustomers> itemsCustomersList = _context.RelatedItems.Select(item => new ItemsCustomers() { CustomerNumber = item.CustomerNumber, ItemId = item.ItemId }).ToList();
            Customer user = await GetCurrentUserAsync();

            if (user == null)
            {
                return View();
            }

            var mlRequest = new Request()
            {
                ItemCustomersList = itemsCustomersList,
                CustomerNumber = user.CustomerNumber,
                AllItemsIds = _context.Items.Select(i => i.Id).Distinct().ToList(),
            };

            try
            {
                Dictionary<int, double> itemsScores = await MachineLearning.ML.Run(mlRequest);
                IEnumerable<KeyValuePair<int, double>> topItems = itemsScores.OrderByDescending(pair => pair.Value).Take(6);
                IQueryable<Item> items = _context.Items.Take(int.MaxValue);
                List<Item> itemsList = (from keyValuePair in topItems from item in items where keyValuePair.Key == item.Id select item).ToList();

                return View(itemsList);
            }
            catch
            {
                //ignored
            }

            return View();
        }

        public IActionResult ContactUs()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ContactUs(Mail form)
        {
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