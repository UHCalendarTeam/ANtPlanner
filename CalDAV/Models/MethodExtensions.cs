using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CalDAV.Models
{
    public static  class Queries
    {
       
     /// <summary>
        /// return a User for a given name
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static User getUser(this CalDavContext source, string userEmail)
        {
            return source.Users.Where(u => u.Email == userEmail).First();
        }

        /// <summary>
        /// return a collection for a given user and collectionName
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static CalendarCollection getCollection(this CalDavContext source, string userEmail, string collectionName)
        {
            return source.getUser(userEmail).CalendarCollections.Where(cl => cl.Name == collectionName).First();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="userEmail">The email of the calendarResource's user.</param>
        /// <param name="collectionName">The collection where is the resource</param>
        /// <param name="calResource">the name of the resource</param>
        /// <returns>return a calendar resource by the given name.</returns>
        public static CalendarResource GetCalendarResource(this CalDavContext source, string userEmail,
            string collectionName, string calResource)
        {
            return source.getCollection(userEmail, collectionName)
                .CalendarResources.Where(cr => cr.Name == calResource)
                .First();
        }
    }
}
