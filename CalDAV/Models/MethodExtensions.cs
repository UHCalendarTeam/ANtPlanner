using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// Check if a collection exist in the system.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="userEmail"></param>
        /// <param name="collectionName"></param>
        /// <returns></returns>
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
        /// Check if a CalendarResource Exist
        /// </summary>
        /// <param name="source"></param>
        /// <param name="userEmail"></param>
        /// <param name="collectionName"></param>
        /// <param name="calResource"></param>
        /// <returns></returns>
        public static bool CalendarResourceExist(this CalDavContext source, string userEmail, string collectionName,
            string calResource)
        {
            if (!CollectionExist(source, userEmail, collectionName))
                return false;

            return (
                from resource in GetCollection(source, userEmail, collectionName).CalendarResources
                where resource.FileName == calResource
                select resource
                ).Count() > 0;
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
    }
}
