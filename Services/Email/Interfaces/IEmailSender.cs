using System.Threading.Tasks;

namespace GamingStore.Services.Email.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}