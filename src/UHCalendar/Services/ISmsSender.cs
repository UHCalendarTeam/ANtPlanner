using System.Threading.Tasks;

namespace UHCalendar.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}