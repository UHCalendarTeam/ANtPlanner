using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Models.Entities;
using Microsoft.AspNet.Identity;

namespace DataLayer.ExtensionMethods
{
    /// <summary>
    /// This class contains some useful methods
    /// for the verification of objects in the DB
    /// </summary>
    public static class VerificationExtensions
    {
        /// <summary>
        /// Hash the user password;
        /// </summary>
        /// <param name="user">The istance of the user whos password has to be hash</param>
        /// <param name="passwordToHash">The password to be hashed.</param>
        /// <returns></returns>
        public static string PasswordHasher(this User user, string passwordToHash)
        {
            var passHasher = new PasswordHasher<User>();
            string hashedPassword = passHasher.HashPassword(user, passwordToHash);
            return hashedPassword;
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
