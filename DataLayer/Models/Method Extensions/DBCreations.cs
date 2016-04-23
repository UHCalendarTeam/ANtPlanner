using System.Collections.Generic;
using System.IO;
using System.Linq;
using DataLayer.Models.ACL;
using DataLayer.Models.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.Data.Entity.Scaffolding.Internal.Configuration;

namespace DataLayer.ExtensionMethods
{
    /// <summary>
    ///     This class defines some useful extension
    ///     methods for the initialization of new
    ///     entities in the system.
    /// </summary>
    public static class DbCreations
    {
        /// <summary>
        ///     Add a user to the system.
        /// </summary>
        /// <param name="context">The system Db context.</param>
        /// <param name="email">The user email.</param>
        /// <param name="fullName">The user full name. This gonna be the displayname for the system.</param>
        /// <param name="password">THe user not encrypted password</param>
        /// <returns>
        ///     The instance of the new User. Have to change the changes with the
        ///     returned object.
        /// </returns>
        public static User CreateUserInSystem(this CalDavContext context, string email, string fullName,
            string password)
        {
            ///create the core passHasher
            var passHasher = new PasswordHasher<User>();


            var user = new User
            {
                Email = email,
                Password = password,
                DisplayName = fullName
            };
            ///create the principal the represents the user
            var principal = new Principal()
            {
                Email = email,
                PrincipalURL = DataLayer.SystemProperties._userPrincipalUrl + email,
            };

            user.Principal = principal;

            ///hass the user password 
            /// the instance of the user has to be pass but is not used
            /// so it need to be updated
            var hashedPassword = passHasher.HashPassword(user, password);
            user.Password = hashedPassword;

            //add the user and its principal to the context
            context.Users.Add(user);
            context.Principals.Add(principal);

            return user;
        }

        /// <summary>
        ///     Initialize a student in the system.
        ///     Create the student in the DB and add him some aditional properties.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="email"></param>
        /// <param name="fullname"></param>
        /// <param name="password"></param>
        /// <param name="career"></param>
        /// <param name="group"></param>
        /// <param name="year"></param>
        /// <returns>
        ///     A new instance of the student. The changes has to be saved
        ///     in the returned object.
        /// </returns>
        public static Student CreateStudentInSystem(this CalDavContext context, string email, string fullname,
            string password, string career, string group, int year)
        {
            var student = new Student(fullname, email, password, career, group, year);
            student.Password = student.PasswordHasher(password);

            ///create the principal the represents the user
            var principal = new Principal()
            {
                Email = email,
                PrincipalURL = DataLayer.SystemProperties._userPrincipalUrl + email,
                //Initialize the properties of the principal with the 
                //require property that represents the groups in wich the principal
                //is directly a member
                Properties = new List<Property>() { GroupMembershipCreation(DataLayer.SystemProperties._groupPrincipalUrl+group) }
            };

            student.Principal = principal;

            //take the collection with user group name
            var collection = context.CalendarCollections.FirstOrDefault(col => col.Group == group);

            //if the collection doent exit then something is wrong with the group
            //and either has to be created or the user group is not valid
            if (collection == null)
                throw new InvalidDataException("The user group doesnt exit in the system." +
                                               "Check if the user group is valid or create the group collection in the system.");

            context.Students.Add(student);
            context.Principals.Add(principal);

            return student;
        }


        public static Worker CreateWorkerInSystem(this CalDavContext context, string email, string password,
            string fullname, string faculty, string department)
        {
            var worker = new Worker(fullname, email, password, department, faculty);
            worker.Password = worker.PasswordHasher(password);


            var collection = new CalendarCollection(DataLayer.SystemProperties._userCollectionUrl + email,
                DataLayer.SystemProperties._defualtInitialCollectionName)
            {
                User = worker,
                UserId = worker.UserId
            };
            worker.CalendarCollections.Add(collection);

            return worker;
        }

        /// <summary>
        /// Create and return a DAV:group-membership property.
        /// </summary>
        /// <param name="groupHref"></param>
        /// <returns></returns>
        private static Property GroupMembershipCreation(string groupHref)
        {
            var property = new Property("group-membership", "DAV:")
            {
                Value = $"<D:group-membership xmlns:D=\"DAV:\"><D:href>{groupHref}</D:href></D:group-membership>"
            };
            return property;
        }
    }
}