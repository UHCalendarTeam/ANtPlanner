using System.ComponentModel.DataAnnotations;

namespace UHCalendar.ViewModels.Account
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}