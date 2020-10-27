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
using Microsoft.AspNetCore.Authorization;

namespace GamingStore.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
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
            const string toAddress = "gamingstoreproject_support@gmail.com";
            string fromAddress = form.Email;
            string subject = $"ContactUs inquiry from {form.Name}";
            var message = new StringBuilder();
            message.Append($"Name: {form.Name}\n");
            message.Append($"Email: {form.Email}\n");
            message.Append($"Telephone: {form.Telephone}\n\n");
            message.Append(form.Message);

            //start email Thread

            await Task.Run(() =>
            {
                SendEmail(toAddress, fromAddress, subject, message.ToString());
            }).ConfigureAwait(false);

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public void SendEmail(string toAddress, string fromAddress, string subject, string message)
        {
            try
            {
                using var mail = new MailMessage();
                const string email = "username@yahoo.com";
                const string password = "password!";

                var loginInfo = new NetworkCredential(email, password);

                mail.From = new MailAddress(fromAddress);
                mail.To.Add(new MailAddress(toAddress));
                mail.Subject = subject;
                mail.Body = message;
                mail.IsBodyHtml = true;

                try
                {
                    using var smtpClient = new SmtpClient("smtp.mail.yahoo.com", 465)
                    {
                        EnableSsl = true,
                        UseDefaultCredentials = false, 
                        Credentials = loginInfo
                    };

                    smtpClient.Send(mail);
                }

                finally
                {
                    //dispose the client
                    mail.Dispose();
                }
            }
            catch (SmtpFailedRecipientsException ex)
            {
                //foreach (SmtpFailedRecipientException t in ex.InnerExceptions)
                //{
                //    SmtpStatusCode status = t.StatusCode;
                //    if (status == SmtpStatusCode.MailboxBusy || status == SmtpStatusCode.MailboxUnavailable)
                //    {
                //        Response.Write("Delivery failed - retrying in 5 seconds.");
                //        System.Threading.Thread.Sleep(5000);
                //        //resend
                //        //smtpClient.Send(message);
                //    }
                //    else
                //    {
                //        Response.Write("Failed to deliver message to {0}",
                //            t.FailedRecipient);
                //    }
                //}
            }
            catch (SmtpException Se)
            {
                // handle exception here
                //Response.Write(Se.ToString());
            }

            catch (Exception ex)
            {
                //Response.Write(ex.ToString());
            }
        }
    }
}