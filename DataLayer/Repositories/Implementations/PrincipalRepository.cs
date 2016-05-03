using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.ExtensionMethods;
using DataLayer.Models.ACL;
using DataLayer.Models.Entities;
using Microsoft.Data.Entity;
using DataLayer.Repositories;
using Microsoft.AspNet.Identity;

namespace DataLayer.Repositories
{
    public class PrincipalRepository : IRepository<Principal, string>
    {
        private readonly CalDavContext _context;

        public PrincipalRepository(CalDavContext context)
        {
            _context = context;
        }




        public async Task<IList<Principal>> GetAll()
        {
            return await _context.Principals.ToListAsync();
        }

        public async Task<Principal> Get(string url)
        {
            return await Task.FromResult(_context.Principals.Include(p => p.Properties)
                .FirstOrDefault(p => p.PrincipalURL == url));
        }


        public void Add(Principal entity)
        {
            _context.Principals.Add(entity);
            _context.SaveChanges();
        }

        public async Task Remove(Principal entity)
        {
            _context.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Remove(string url)
        {
            var principal = await _context.Principals.FirstOrDefaultAsync(p => p.PrincipalURL == url);
            await Remove(principal);
        }

        public Task<int> Count()
        {
            return _context.Principals.CountAsync();
        }

        public async Task<bool> Exist(string url)
        {
            return await Task.FromResult(_context.Principals.Any(p => p.PrincipalURL == url));
        }

        public async Task<IList<Property>> GetAllProperties(string url)
        {
            var principal = await Get(url);

            return await Task.FromResult(principal?.Properties.Where(x => x.IsVisible).ToList());
        }

        public async Task<Property> GetProperty(string url, KeyValuePair<string, string> propertyNameandNs)
        {
            var principal = await Get(url);
            Property property;

            if (string.IsNullOrEmpty(propertyNameandNs.Value))
                property = principal.Properties.FirstOrDefault(
                    prop => prop.Name == propertyNameandNs.Key && prop.Namespace == propertyNameandNs.Value);
            else
                property = principal.Properties.FirstOrDefault(
                    prop => prop.Name == propertyNameandNs.Key);
            return await Task.FromResult(property);
        }

        public async Task<IList<KeyValuePair<string, string>>> GetAllPropname(string url)
        {
            var principal = await Get(url);
            return
               await Task.FromResult(principal?.Properties.Select(prop => new KeyValuePair<string, string>(prop.Name, prop.Namespace))
                    .ToList());
        }

        public async Task<bool> RemoveProperty(string url, KeyValuePair<string, string> propertyNameNs,
            Stack<string> errorStack)
        {
            var principal = await Get(url);
            var property =
                principal.Properties.FirstOrDefault(
                    prop => prop.Name == propertyNameNs.Key && prop.Namespace == propertyNameNs.Value);

            if (property == null)
                return false;
            if (property.IsDestroyable)
            {
                principal.Properties.Remove(property);
            }
            return await Task.FromResult(true);

        }

        public async Task<bool> CreateOrModifyProperty(string url, string propName, string propNs, string propValue,
            Stack<string> errorStack,
            bool adminPrivilege)
        {
            var principal = await Get(url);
            var propperty =
                principal.Properties.FirstOrDefault(prop => prop.Name == propName && prop.Namespace == propNs);

            //if the property is null then create it
            if (propperty == null)
            {
                var prop = new Property(propName, propNs)
                {
                    Value = propValue
                };
                principal.Properties.Add(prop);
            }
            else
            {
                if (propperty.IsMutable || adminPrivilege)
                {
                    propperty.Value = propValue;
                }
            }
            return await Task.FromResult(true);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        ///     Verify if the provided password match with the user's password.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="userEmail">The user email (this is out username)</param>
        /// <param name="password">The provided password.</param>
        /// <returns></returns>
        public bool VerifyPassword(Principal principal, string password = "")
        {
            //if the user doenst exit return false
            if (principal == null)
                return false;

            var user = principal.User;
            var passwordHasher = new PasswordHasher<User>();
            var result = passwordHasher.VerifyHashedPassword(user, user.Password, password);

            //return the result of the password verification
            return result != PasswordVerificationResult.Failed;

        }


        public async Task<Principal> GetByIdentifierAsync(string identifier)
        {
            return await _context.Principals.Include(p => p.User).Include(p=>p.Properties)
                .FirstOrDefaultAsync(u => u.PrincipalStringIdentifier == identifier);
        }

        public Principal GetByIdentifier(string identifier)
        {
            using (var context = new CalDavContext())
            {
                return context.Principals.Include(p => p.User).Include(p => p.Properties)
                    .FirstOrDefault(u => u.PrincipalStringIdentifier == identifier);
            }
        }

        /// <summary>
        /// Get a principal with the given cookie value
        /// </summary>
        /// <param name="cookieValue"></param>
        /// <returns></returns>
        public async Task<Principal> GetByCookie(string cookieValue)
        {
           return await _context.Principals.FirstOrDefaultAsync(p => p.SessionId == cookieValue);
        }


        public async Task<bool> ExistByStringIs(string identifier)
        {

            return await _context.Principals.AnyAsync(p => p.PrincipalStringIdentifier == identifier);
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


            //create the principal the represents the user
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
                false, false);

            col.Properties.Add(ownerProp);
            col.Properties.Add(PropertyCreation.CreateAclPropertyForUserCollections(principal.PrincipalURL));

            //add the calaendar to the collection of the principal
            principal.CalendarCollections.Add(col);

            #endregion

            //create the folder that will contain the 
            //calendars of the user
            new FileSystemManagement().AddCalendarCollectionFolder(col.Url);

            //hass the user password 
            // the instance of the user has to be pass but is not used
            // so it need to be updated
            var hashedPassword = passHasher.HashPassword(user, password);
            user.Password = hashedPassword;

            //add the user and its principal to the context
            _context.Users.Add(user);
            _context.Principals.Add(principal);
            _context.CalendarCollections.Add(col);

            _context.SaveChanges();

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
                _context.CalendarCollections.FirstOrDefault(
                    col => col.Url == SystemProperties._groupCollectionUrl + group);

            //if the collection doent exit then something is wrong with the group
            //and either has to be created or the user group is not valid
            if (collection == null)
                throw new InvalidDataException("The user group doesnt exit in the system." +
                                               "Check if the user group is valid or create the group collection in the system.");

            //create the calendar-home-set
            var calHomeSet = PropertyCreation.CreateCalendarHomeSet(SystemProperties.PrincipalType.Group, group, collection.Name);
            //add the property to the principal
            principal.Properties.Add(calHomeSet);

            _context.Students.Add(student);
            _context.Principals.Add(principal);

            _context.SaveChanges();

            return student;
        }


        public Worker CreateWorkerInSystem(string email, string password,
            string fullName, string faculty, string department)
        {
            var worker = new Worker(fullName, email, password, department, faculty);
            worker.Password = worker.PasswordHasher(password);


            //create useful properties for the principal
            var calHomeSet = PropertyCreation.CreateCalendarHomeSet(SystemProperties.PrincipalType.User, email, SystemProperties._defualtInitialCollectionName);
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
            principal.CalendarCollections.Add(col);

            //create the folder that will contain the 
            //calendars of the user
            new FileSystemManagement().AddCalendarCollectionFolder(col.Url);

            //add the user and its principal to the context
            _context.Workers.Add(worker);
            _context.Principals.Add(principal);
            _context.CalendarCollections.Add(col);

            _context.SaveChanges();
            return worker;
        }


        public Principal CreateGroup(string pUrl, string groupName)
        {
            throw new NotImplementedException();
        }


        public async Task SetCookie(string principalEmail, string cookieValue)
        {
            var principal =  GetByIdentifier(principalEmail);
            if (principal == null)
                return;

            principal.SessionId = cookieValue;
            await _context.SaveChangesAsync();
        }

        public async Task<int> SaveChangeAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}