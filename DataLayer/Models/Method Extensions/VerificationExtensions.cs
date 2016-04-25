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



    

    }
}
