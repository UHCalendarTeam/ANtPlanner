using System.ComponentModel.DataAnnotations;

namespace UHCalendar.ViewModels.Account
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}