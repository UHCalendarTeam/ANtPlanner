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
using Microsoft.Extensions.Configuration;


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
        /// <returns>The instance of the new User. Have to change the changes with the 
        /// returned object.</returns>
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
            
            return user;
        }

        /// <summary>
        /// Initialize a student in the system.
        /// Create the student in the DB and add him some aditional properties.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="email"></param>
        /// <param name="fullname"></param>
        /// <param name="password"></param>
        /// <param name="career"></param>
        /// <param name="group"></param>
        /// <param name="year"></param>
        /// <returns>A new instance of the student. The changes has to be saved
        /// in the returned object.</returns>
        public static Student CreateStudentInSystem(this CalDavContext context, string email, string fullname,
            string password, string career, string group, int year)
        {
            var student = new Student(fullname, email, password, career, group, year);
            student.Password = student.PasswordHasher(password);

            //take the collection with user group name
            var collection = context.CalendarCollections.FirstOrDefault(col => col.Group == group);

            //if the collection doent exit then something is wrong with the group
            //and either has to be created or the user group is not valid
            if(collection == null)
                throw new InvalidDataException("The user group doesnt exit in the system." +
                                           "Check if the user group is valid or create the group collection in the system.");
            return student;

        }


        public static Worker CreateWorkerInSystem(this CalDavContext context, string email, string password,
            string fullname, string faculty, string department)
        {
            var worker = new Worker(fullname, email, password, department, faculty);
            worker.Password = worker.PasswordHasher(password);

           
            

            var collection = new CalendarCollection(context._userCollectionUrl+email, context._defualtInitialCollectionName)
            {
                User = worker,
                UserId = worker.UserId
                
            };
            worker.CalendarCollections.Add(collection);

            return worker;
        }





        /// <summary>
        /// Hash the user password;
        /// </summary>
        /// <param name="user">The istance of the user whos password has to be hash</param>
        /// <param name="passwordToHash">The password to be hashed.</param>
        /// <returns></returns>
        private static string PasswordHasher(this User user, string passwordToHash)
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
