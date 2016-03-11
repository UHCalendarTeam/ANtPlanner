using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalDAV.Core.Interfaces;
using CalDAV.Models;

namespace CalDAV.Core
{
    /// <summary>
    /// Contains the logic for the creation of new clients of the system.
    /// </summary>
    public class StartUp: IStartUp
    {
        public bool CreateUserInSystem(string userEmail, string userName, string userLastName)
        {
            using (var db = new CalDavContext())
            {
                db.Users.Add(new User() {Email = userEmail, FirstName = userName, LastName = userLastName});
            }
            return false;
        }

        public bool CreateCollectionForUser(string userEmail, string collectionName)
        {
            throw new NotImplementedException();
        }
    }
}
