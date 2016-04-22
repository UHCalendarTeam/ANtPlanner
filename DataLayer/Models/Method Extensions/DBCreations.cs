using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Models.Entities;
using Microsoft.AspNet.Cryptography.KeyDerivation;
using Microsoft.AspNet.Identity;


namespace DataLayer.ExtensionMethods
{
    /// <summary>
    /// This class defines some useful extension
    /// methods for the initialization of new 
    /// entities in the system.
    /// </summary>
    public static class DbCreations
    {
        /// <summary>
        /// Add a user to the system.
        /// </summary>
        /// <param name="context">The system Db context.</param>
        /// <param name="email">The user email.</param>
        /// <param name="fullName">The user full name. This gonna be the displayname for the system.</param>
        /// <param name="password">THe user not encrypted password</param>
        /// <returns>The instance of the new User.</returns>
        public static User CreateUserInSystem(this CalDavContext context, string email, string fullName,
            string password)
        {
            var passHasher = new PasswordHasher<User>();
            
           

            var user = new User()
            {
                Email = email,
                Password = password,
                DisplayName = fullName

            };
            ///hass the user password 
            /// the instance of the user has to be pass but is not used
            /// so it need to be updated
            string hashedPassword = passHasher.HashPassword(user, password);
            user.Password = hashedPassword;

            context.SaveChanges();
            return user;
        }

        /// <summary>
        /// Verify if the provided password match with the user's password.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="username">The user email (this is out username)</param>
        /// <param name="password">The provided password.</param>
        /// <returns></returns>
        public static bool VerifyPassword(this CalDavContext context, string username, string password)
        {
            var user = context.Users.FirstOrDefault(u => u.Email == username);

            //if the user doenst exit return false
            if (user == null)
                return false;

            var passwordHasher = new PasswordHasher<User>();

            var result = passwordHasher.VerifyHashedPassword(user, user.Password, password);

            ///return the result of the password verification
            return result != PasswordVerificationResult.Failed;
        }

        




    }
}
