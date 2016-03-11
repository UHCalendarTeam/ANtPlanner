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
                db.SaveChanges();
                //TODO: call the Directory User Creator
            }
            return true;
        }

        public bool CreateCollectionForUser(string userEmail, string collectionName, string calendarDescription = "")
        {
            using (var db = new CalDavContext())
            {
                var user = db.getUser(userEmail);
                CalendarCollection collection = new CalendarCollection()
                {
                    User = user,
                    Name = collectionName,
                    CalendarDescription = calendarDescription==""?"This is a desfault calendar collection. Should provide the calendar description":calendarDescription,
                    //TODO: set here the other values that are gonna have the calendar collection by default
                                
                };
                user.CalendarCollections.Add(collection);
                db.SaveChanges();
                //TODO: call here the directory collection creator.
                return true;
            }
        }
    }
}
