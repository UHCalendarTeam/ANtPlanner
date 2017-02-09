using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Contexts;
using DataLayer.ExtensionMethods;
using DataLayer.Models.Entities.ACL;
using DataLayer.Models.Entities.ResourcesAndCollections;
using DataLayer.Models.Entities.Users;
using DataLayer.Models.Interfaces.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace DataLayer.Models.Repositories
{
    public class PrincipalRepository : PropertyContainerRepository<Principal, string>, IPrincipalRepository
    {
        private ICollectionRepository _collectionRepository;
        private ICalendarHomeRepository _homeRepository;
        private IUserRepository _userRepository;

        public PrincipalRepository(CalDavContext context) : base(context)
        {
            _collectionRepository = new  CalendarCollectionRepository(context);
            _homeRepository = new CalendarHomeRepository(context);
            _userRepository = new UserRepository(context);
        }

        public new Principal Find(string id)
        {
            return Context.Principals.Include(p => p.Properties).Include(p => p.CalendarHome).ThenInclude(x => x.CalendarCollections)
                .FirstOrDefault(p => p.PrincipalUrl == id);
        }

        public new async Task<Principal> FindAsync(string id)
        {
            return await Task.FromResult(Context.Principals.Include(p => p.Properties)
                .FirstOrDefault(p => p.PrincipalUrl == id));
        }

        /// <summary>
        ///     Verify if the provided password match with the user's password.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="userEmail">The user email (this is out username)</param>
        /// <param name="principal"></param>
        /// <param name="password">The provided password.</param>
        /// <returns></returns>
        public static bool VerifyPassword(Principal principal, string password = "")
        {
            //if the user doesn't exit return false
            if (principal == null)
                return false;

            var user = principal.User;
            var passwordHasher = new PasswordHasher<User>();
            var result = passwordHasher.VerifyHashedPassword(user, user.Password, password);

            //return the result of the password verification
            return result != PasswordVerificationResult.Failed;
        }

        bool IPrincipalRepository.VerifyPassword(Principal principal, string password)
        {
            return VerifyPassword(principal, password);
        }

        public async Task<Principal> FindByIdentifierAsync(string identifier)
        {
            return await Context.Principals.Include(p => p.User)
                .Include(p => p.Properties)
                .Include(p => p.CalendarHome).ThenInclude(x => x.CalendarCollections)
                .FirstOrDefaultAsync(u => u.PrincipalStringIdentifier == identifier);
        }

        public Principal FindByIdentifier(string identifier)
        {
            using (var context = new CalDavContext())
            {
                return context.Principals.Include(p => p.User)
                    .Include(p => p.Properties)
                    .Include(p => p.CalendarHome)
                    .FirstOrDefault(u => u.PrincipalStringIdentifier == identifier);
            }
        }

        /// <summary>
        ///     Get a principal with the given cookie value
        /// </summary>
        /// <param name="cookieValue"></param>
        /// <returns></returns>
        public async Task<Principal> GetByCookie(string cookieValue)
        {
            return await Context.Principals.FirstOrDefaultAsync(p => p.SessionId == cookieValue);
        }

        public async Task<bool> ExistByStringIs(string identifier)
        {
            return await Context.Principals.AnyAsync(p => p.PrincipalStringIdentifier == identifier);
        }

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
        public Principal CreateUserInSystem(string email, string fullName,
            string password)
        {
            //create the core passHasher
            var passHasher = new PasswordHasher<User>();
            var fsm = new FileManagement();

            var user = new User
            {
                Email = email,
                Password = password,
                DisplayName = fullName
            };

            var defaultCalName = "DefaultCalendar";
            var defaultCalHomeName = "HomeCollection";

            //create useful properties for the principal
            //var calHomeSet = PropertyCreation.CreateCalendarHomeSet(SystemProperties.PrincipalType.User, email,
            //    defaultCalHomeName);
            var displayName = PropertyCreation.CreateProperty("displayname", "D", fullName);


            //create the principal the represents the user
            var principal = new Principal(email, SystemProperties.PrincipalType.User,
                displayName);


            user.PrincipalId = principal.Id;
            principal.UserId = user.Id;


            //create the cal home for the principal
            var calHome = HomeRepository.CreateCalendarHome(principal);

            var calHomeSet = PropertyCreation.CreateCalHomeSetWithHref(calHome.Url);
            principal.Properties.Add(calHomeSet);

            calHome.PrincipalId = principal.Id;
            principal.CalendarHomeId = calHome.Id;


            //hash the user password 
            // the instance of the user has to be pass but is not used
            // so it need to be updated with the encrypted password
            var hashedPassword = passHasher.HashPassword(user, password);
            user.Password = hashedPassword;


            _collectionRepository.AddRange(calHome.CalendarCollections);
            //calHome.CalendarCollections = null;
            //add the user and its principal to the context
            //Context.Users.Add(user);
            Add(principal);
            calHome.CalendarCollections = null;
            _homeRepository.Add(calHome);
            _userRepository.Add(user);

            //Context.CalendarCollections.AddRange(calHome.CalendarCollections);
            //_collectionRepository.AddRange(calHome.CalendarCollections);
            //Context.Properties.AddRange(calHome.Properties);
            //Context.Properties.AddRange(principal.Properties);
            try
            {
                Context.SaveChanges();

            }
            catch (NpgsqlException e)
             {
                Console.WriteLine(e.Message);
            }

            return principal;
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
        public Student CreateStudentInSystem(string email, string fullname,
            string password, string career, string group, int year)
        {
            var student = new Student(fullname, email, password, career, group, year);

            student.Password = student.PasswordHasher(password);

            //create the necessary properties for the students
            // 
            // create the DAV:group-membership
            var gMembership = PropertyCreation.CreateGroupMembership(SystemProperties._groupPrincipalUrl + group + "/");


            //create the displayname
            var displayName = PropertyCreation.CreateProperty("displayname", "D", fullname);

            //create the principal the represents the user
            var principal = new Principal(email, SystemProperties.PrincipalType.Student, gMembership, displayName);

            student.Principal = principal;
            principal.User = student;

            //take the collection with user group name
            var collection =
                Context.CalendarCollections.FirstOrDefault(
                    col => col.Url == SystemProperties._groupCollectionUrl + group);

            //if the collection doent exit then something is wrong with the group
            //and either has to be created or the user group is not valid
            if (collection == null)
                throw new InvalidDataException("The user group doesnt exit in the system." +
                                               "Check if the user group is valid or create the group collection in the system.");

            //create the calendar-home-set
            var calHomeSet = PropertyCreation.CreateCalendarHomeSet(SystemProperties.PrincipalType.Group, group,
                collection.Name);
            //add the property to the principal
            principal.Properties.Add(calHomeSet);

            Context.Students.Add(student);
            Context.Principals.Add(principal);

            Context.SaveChanges();

            return student;
        }

        public Worker CreateWorkerInSystem(string email, string password,
            string fullName, string faculty, string department)
        {
            var worker = new Worker(fullName, email, password, department, faculty);
            worker.Password = worker.PasswordHasher(password);


            //create useful properties for the principal
            var calHomeSet = PropertyCreation.CreateCalendarHomeSet(SystemProperties.PrincipalType.User, email,
                SystemProperties._defualtInitialCollectionName);
            var displayName = PropertyCreation.CreateProperty("displayname", "D", fullName);

            //create the principal the represents the user
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
            principal.CalendarHome.CalendarCollections.Add(col);

            //create the folder that will contain the 
            //calendars of the user
            new FileManagement().CreateFolder(col.Url);

            //add the user and its principal to the context
            Context.Workers.Add(worker);
            Context.Principals.Add(principal);
            Context.CalendarCollections.Add(col);

            Context.SaveChanges();
            return worker;
        }

        public Principal CreateGroup(string pUrl, string groupName)
        {
            throw new NotImplementedException();
        }

        public async Task SetCookie(string principalEmail, string cookieValue)
        {
            var principal = FindByIdentifier(principalEmail);
            if (principal == null)
                return;

            principal.SessionId = cookieValue;
            await Context.SaveChangesAsync();
        }
    }
}