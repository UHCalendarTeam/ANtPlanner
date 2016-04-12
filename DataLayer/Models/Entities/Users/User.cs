using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataLayer.Entities
{
    /// <summary>
    ///     Defines the main properties for the users
    ///     of the system.
    /// </summary>
    public class User
    {
        [ScaffoldColumn(false)]
        public int UserId { get; set; }


        public string Email { get; set; }


        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        public ICollection<CalendarResource> Resources { get; set; }

        public ICollection<CalendarCollection> CalendarCollections { get; set; }
    }
}