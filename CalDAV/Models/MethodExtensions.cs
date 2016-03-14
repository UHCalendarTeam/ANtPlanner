using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace CalDAV.Models
{
    public static class Queries
    {
        /// <summary>
        /// Check if a User exist in the system
        /// </summary>
        /// <param name="source"></param>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public static bool UserExist(this CalDavContext source, string userEmail)
        {
            return (
                from user in source.Users
                where user.Email == userEmail
                select user).Count() > 0;
        }
        /// <summary>
        /// return a User for a given name
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static User GetUser(this CalDavContext source, string userEmail)
        {
            return source.Users.Where(u => u.Email == userEmail).First();
        }

        public static bool CollectionExist(this CalDavContext source, string userEmail, string collectionName)
        {
            if (!UserExist(source, userEmail)) 
            return false;

            return (
                from collection in GetUser(source, userEmail).CalendarCollections
                where collection.Name == collectionName
                select collection
                ).Count() > 0;
        }
        /// <summary>
        /// return a collection for a given user and collectionName
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static CalendarCollection GetCollection(this CalDavContext source, string userEmail, string collectionName)
        {
            return source.GetUser(userEmail).CalendarCollections.Where(cl => cl.Name == collectionName).First();
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
            return source.GetCollection(userEmail, collectionName)
                .CalendarResources
                .First(cr => cr.FileName == calResource);
        }

        /// <summary>
        /// Filter the resources of the user in the given collection
        /// by filter of the dates.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="starTime">The startTime of the  </param>
        /// <param name="endTime"></param>
        /// <param name="ownerName"></param>
        /// <param name="colName"></param>
        /// <returns></returns>
        public static IEnumerable<CalendarResource> TimeRangeFilter(this CalDavContext source, DateTime starTime,
            DateTime endTime, string ownerName, string colName)
        {
            //TODO: expand the recurrence instances of the resources.
            //TODO: convert the datetimes to the UTC of the request
            //TODO: check if the where predicate is the same that in the protocol
            var resources= source.GetCollection(ownerName, colName).CalendarResources;
            var output =
                resources.Select(resource => resource)
                    .Where(resource => resource.DtStart == starTime && resource.DtEnd < endTime);
            return output;

        }
    }
}
