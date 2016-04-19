using System.Threading.Tasks;

namespace UHCalendar.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}