using System.Threading.Tasks;

namespace GamingStore.Services.Email.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string toEmail, string subject, string message);
    }
}