using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CalDAV.Core
{
    /// <summary>
    /// Define the methods that should be use for the initial configuration of new users and collection.
    /// </summary>
    public interface IStartUp
    {
        /// <summary>
        /// Create a new user in the system( add him to the DB and create a directory for his collections)
        /// </summary>
        /// <param name="userEmail">THe email of the new user.</param>
        /// <param name="userName">The firstName of the new user.</param>
        /// <param name="userLastName">The lastName of the new user.</param>
        /// <returns>True if success, false otherwise.</returns>
        bool CreateUserInSystem(string userEmail, string userName, string userLastName);
        /// <summary>
        /// Create a new collection for a given user.( Add to the DB and relate it with the user, 
        /// Create a new folder for the collection
        /// </summary>
        /// <param name="userEmail">The email of the collection's user.</param>
        /// <param name="collectionName">THe name for the new collection.</param>
        /// <param name="calendarDescription">THe calendar description.</param>
        /// <returns>True if success, false otherwise</returns>
        bool CreateCollectionForUser(string userEmail, string collectionName, string calendarDescription = "",
            string calDisplayName = "", List<string> supportedCalendarComponentSet = null, string calTimeZone = "");


    }
}
