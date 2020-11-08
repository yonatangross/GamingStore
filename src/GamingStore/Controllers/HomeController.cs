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
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<HomeController> _logger;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<Customer> _userManager;

        private Task<Customer> GetCurrentUserAsync() => _userManager.GetUserAsync(User);

        public HomeController(ILogger<HomeController> logger, IEmailSender emailSender, StoreContext context)
        {
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var itemsCustomersList = _context.RelatedItems.Select(item => new ItemsCustomers() { CustomerNumber = item.CustomerNumber, ItemId = item.ItemId }).ToList();

            var user = await GetCurrentUserAsync();
            var mlRequest = new Request()
            {
                IdsList = itemsCustomersList,
                CustomerNumber = user.CustomerNumber,
                AllItemsIds = _context.Items.Select(i => i.Id).Distinct().ToList()
            };


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
            await Task.Run(() => { _emailSender.SendEmailAsync(toAddress, subject, message.ToString()); })
                .ConfigureAwait(false);

            return View();
        }
    }
}