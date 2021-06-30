using System;
using System.Threading.Tasks;
using GamingStore.Services.Email.Interfaces;
using MailKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace GamingStore.Services.Email
{
    public class EmailSender : IEmailSender
    {
        public EmailSender(IOptions<AuthMessageOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }

        public AuthMessageOptions Options { get; } //set only via Secret Manager

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress(Options.AppName, Options.GmailUser));
            mimeMessage.To.Add(new MailboxAddress(address: toEmail));
            mimeMessage.Subject = subject; //Subject
            mimeMessage.Body = new TextPart("html") // message rendered as html
            {
                Text = message
            };
            
            //logs the smtp protocol transmission 
            using var client = new SmtpClient(new ProtocolLogger("smtp.log"))
            {
                ServerCertificateValidationCallback = (sender, certificate, certChainType, errors) => true
            };

            await client.ConnectAsync(Options.SmtpServer, Options.SmtpPortNumber, false).ConfigureAwait(false);
            await client.AuthenticateAsync(Options.GmailUser, Options.GmailKey).ConfigureAwait(false);
            await client.SendAsync(mimeMessage).ConfigureAwait(false);
            client.MessageSent += OnMessageSent;
            await client.DisconnectAsync(true).ConfigureAwait(false);
        }


        private static void OnMessageSent(object sender, MessageSentEventArgs e)
        {
            Console.WriteLine("The message was sent!");
        }
    }
}