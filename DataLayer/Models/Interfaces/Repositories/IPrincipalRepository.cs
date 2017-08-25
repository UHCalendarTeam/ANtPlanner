using System.Diagnostics;
using System.Threading.Tasks;
using DataLayer.Models.Entities;
using DataLayer.Models.Entities.ACL;
using DataLayer.Models.Entities.ResourcesAndCollections;
using DataLayer.Models.Entities.Users;
using DataLayer.Models.Identity;

namespace DataLayer.Models.Interfaces.Repositories
{
    public interface IPrincipalRepository:IUrlContainerRepository<Principal>,IPropertyContainerRepository<Principal,string>
    {
        /// <summary>
        ///     Verify if the provided password match with the user's password.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="userEmail">The user email (this is out username)</param>
        /// <param name="principal"></param>
        /// <param name="password">The provided password.</param>
        /// <returns></returns>
        bool VerifyPassword(Principal principal, string password = "");

        Task<Principal> FindByIdentifierAsync(string identifier);

        Principal FindByIdentifier(string identifier);

        /// <summary>
        /// Find a <see cref="Principal"/> given a its PrincipalId
        /// </summary>
        /// <param name="principalId">Primary Key of the Principal</param>
        /// <returns></returns>
        Principal FindByPrincipalId(string principalId);

        /// <summary>
        /// Find a <see cref="Principal"/> given a its PrincipalId including
        /// it's <see cref="Property"/>, <see cref="CalendarHome"/>, <see cref="CalendarCollection"/>, <see cref="CalendarResource"/> and <see cref="ApplicationUser"/> 
        /// </summary>
        /// <param name="principalId">Primary Key of the Principal</param>
        /// <returns></returns>
        Principal FindByPrincipalIdWithAll(string principalId);

        /// <summary>
        ///     Get a principal with the given cookie value
        /// </summary>
        /// <param name="cookieValue"></param>
        /// <returns></returns>
        Task<Principal> GetByCookie(string cookieValue);

        Task<bool> ExistByStringIs(string identifier);

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
        Principal CreateUserInSystem(string email, string fullName,
           string password);

        /// <summary>
        ///     Add a user to the system.
        /// </summary>
        /// <returns>
        ///     The instance of the new User. Have to change the changes with the
        ///     returned object.
        /// </returns>
        Principal CreateUserAppPrincipalInSystem(ApplicationUser user);


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
        Student CreateStudentInSystem(string email, string fullname,
           string password, string career, string group, int year);

        Worker CreateWorkerInSystem(string email, string password,
           string fullName, string faculty, string department);

        Principal CreateGroup(string pUrl, string groupName);

        Task SetCookie(string principalEmail, string cookieValue);
    }
}
