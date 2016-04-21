using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataLayer.Models.Entities
{
    /// <summary>
    ///     Defines the main properties for the users
    ///     of the system.
    /// </summary>
    public class User
    {
        public User()
        {
            //CalendarCollections = new List<CalendarCollection>();
        }

        public User(string ftname, string email)
        {
            Email = email;
            FirstName = ftname;
            CalendarCollections = new List<CalendarCollection>();
        }

        [ScaffoldColumn(false)]
        public int UserId { get; set; }

        [Required]
        public string Email { get; set; }


        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        //public ICollection<CalendarResource> Resources { get; set; }

        public ICollection<CalendarCollection> CalendarCollections { get; set; }
    }
}