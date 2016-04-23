using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DataLayer.Models.ACL;
using Microsoft.AspNet.Identity.EntityFramework;

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
           
        }

        public User(string displayName, string email)
        {
            Email = email;
            DisplayName = displayName;
        }

        /// <summary>
        ///     Create a new instance of a user
        /// </summary>
        /// <param name="displayName">The user full name.</param>
        /// <param name="email">The user email.</param>
        /// <param name="password">The user password.</param>
        public User(string displayName, string email, string password)
        {
            Email = email;
            Password = password;
            DisplayName = displayName;
        }

        [ScaffoldColumn(false)]
        public int UserId { get; set; }

        [Required]
        public string Email { get; set; }
      
        ///     Contains the user fullName.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        ///     Contains the user password.
        ///     The password is encrypted before save it.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Contains the principal that represent the user. 
        /// </summary>
        public Principal Principal { get; set; }

        public int? PrincipalId { get; set; }
        

       
    }
}