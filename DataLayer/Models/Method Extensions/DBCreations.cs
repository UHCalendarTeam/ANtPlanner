using System;
using System.IO;
using System.Linq;
using DataLayer.Models.ACL;
using DataLayer.Models.Entities;
using Microsoft.AspNet.Identity;

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

            var defaultCalName = email;

            //create useful properties for the principal
            var calHomeSet = PropertyCreation.CreateCalendarHomeSet(SystemProperties.PrincipalType.User, email, defaultCalName);
            var displayName = PropertyCreation.CreateProperty("displayname", "D", fullName);
            

            ///create the principal the represents the user
            var principal = new Principal(email, SystemProperties.PrincipalType.User,
                displayName, calHomeSet);

            user.Principal = principal;

            #region create the collection for the principal
            //create the collection for the user.
            var col =
                new CalendarCollection(
                    $"{SystemProperties._userCollectionUrl}{email}/{defaultCalName}/",
                    defaultCalName)
                {
                    Principal = principal
                };

            //add the ACL properties to the collection
            var ownerProp = PropertyCreation.CreateProperty("owner", "D", $"<D:href>{principal.PrincipalURL}</D:href>",
                false, false, true);

            col.Properties.Add(ownerProp);
            col.Properties.Add(PropertyCreation.CreateAclPropertyForUserCollections(principal.PrincipalURL));

            //add the calaendar to the collection of the principal
            principal.CalendarCollections.Add(col);

            #endregion

            //create the folder that will contain the 
            //calendars of the user
            new FileSystemManagement().AddCalendarCollectionFolder(col.Url);

            ///hass the user password 
            /// the instance of the user has to be pass but is not used
            /// so it need to be updated
            var hashedPassword = passHasher.HashPassword(user, password);
            user.Password = hashedPassword;

            //add the user and its principal to the context
            context.Users.Add(user);
            context.Principals.Add(principal);
            context.CalendarCollections.Add(col);

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

            ///create the necessary properties for the students
            /// 
            /// create the DAV:group-membership
            var gMembership = PropertyCreation.CreateGroupMembership(SystemProperties._groupPrincipalUrl + group + "/");

            

            //create the displayname
            var displayName = PropertyCreation.CreateProperty("displayname", "D", fullname);

            ///create the principal the represents the user
            var principal = new Principal(email, SystemProperties.PrincipalType.Student,  gMembership, displayName);

            student.Principal = principal;
            principal.User = student;

            //take the collection with user group name
            var collection =
                context.CalendarCollections.FirstOrDefault(
                    col => col.Url ==SystemProperties._groupCollectionUrl + group);

            //if the collection doent exit then something is wrong with the group
            //and either has to be created or the user group is not valid
            if (collection == null)
                throw new InvalidDataException("The user group doesnt exit in the system." +
                                               "Check if the user group is valid or create the group collection in the system.");

            //create the calendar-home-set
            var calHomeSet = PropertyCreation.CreateCalendarHomeSet(SystemProperties.PrincipalType.Group, group, collection.Name);
            //add the property to the principal
            principal.Properties.Add(calHomeSet);

            context.Students.Add(student);
            context.Principals.Add(principal);

            return student;
        }


        public static Worker CreateWorkerInSystem(this CalDavContext context, string email, string password,
            string fullName, string faculty, string department)
        {
            var worker = new Worker(fullName, email, password, department, faculty);
            worker.Password = worker.PasswordHasher(password);


            //create useful properties for the principal
            var calHomeSet = PropertyCreation.CreateCalendarHomeSet(SystemProperties.PrincipalType.User, email, SystemProperties._defualtInitialCollectionName);
            var displayName = PropertyCreation.CreateProperty("displayname", "D", fullName);

            ///create the principal the represents the user
            var principal = new Principal(email, SystemProperties.PrincipalType.User,
                displayName, calHomeSet);

            worker.Principal = principal;

            //create the collection for the user.
            var col =
                new CalendarCollection(
                    $"{SystemProperties._userCollectionUrl}{email}/{SystemProperties._defualtInitialCollectionName}/",
                    SystemProperties._defualtInitialCollectionName)
                {
                    Principal = principal
                };

            //add the calaendar to the collection of the principal
            principal.CalendarCollections.Add(col);

            //create the folder that will contain the 
            //calendars of the user
            new FileSystemManagement().AddCalendarCollectionFolder(col.Url);

            //add the user and its principal to the context
            context.Workers.Add(worker);
            context.Principals.Add(principal);
            context.CalendarCollections.Add(col);

            return worker;
        }


        public static Principal CreateGroup(this CalDavContext context, string pUrl, string groupName)
        {
            throw new NotImplementedException();
        }
    }
}