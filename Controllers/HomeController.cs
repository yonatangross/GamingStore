using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GamingStore.Models;
using GamingStore.Services.Email;
using GamingStore.Services.Email.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace GamingStore.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEmailSender _emailSender;

        public HomeController(ILogger<HomeController> logger, IEmailSender emailSender)
        {
            _logger = logger;
            _emailSender = emailSender;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
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
            await Task.Run(() =>
            {
                _emailSender.SendEmailAsync(toAddress, subject, message.ToString());
            }).ConfigureAwait(false);

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}