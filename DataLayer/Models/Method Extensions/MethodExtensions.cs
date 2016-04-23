using System;
using System.Linq;
using DataLayer.Models.ACL;
using DataLayer.Models.Entities;
using Microsoft.Data.Entity;

namespace DataLayer
{
    /// <summary>
    ///     This class contains method extension for the CaldavContext.
    /// </summary>
    public static class Queries
    {
        /// <summary>
        ///     Check if a User exist in the system
        /// </summary>
        /// <param name="source"></param>
        /// <param name="userEmail"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool PrincipalExist(this CalDavContext source, string url)
        {
            return (
                from principal in source.Principals
                where principal.PrincipalURL == url
                select principal).Any();
        }

        /// <summary>
        ///     return a Principal for a given url
        /// </summary>
        /// <param name="source"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Principal GetPrincipal(this CalDavContext source, string url)
        {
            return source.Principals.Include(p => p.Properties)
                .Include(c => c.CalendarCollections)
                .ThenInclude(cp => cp.Properties)
                .Include(c2 => c2.CalendarCollections)
                .ThenInclude(r => r.CalendarResources)
                .ThenInclude(rp => rp.Properties)
                .FirstOrDefault();
        }

        /// <summary>
        ///     Returns a Principal for a given UserEmail
        /// </summary>
        /// <param name="source"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        //public static Principal GetPrincipalByEmail(this CalDavContext source, string email)
        //{
        //    return source.Principals.Include(p => p.Properties)
        //            .Include(c => c.CalendarCollections)
        //            .ThenInclude(cp => cp.Properties)
        //            .Include(c2 => c2.CalendarCollections)
        //            .ThenInclude(r => r.CalendarResources)
        //            .ThenInclude(rp => rp.Properties)
        //        .FirstOrDefault(u => u.Email == email);
        //}

        /// <summary>
        ///     Check if a collection exist in the system.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="principalUrl"></param>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        public static bool CollectionExist(this CalDavContext source, string principalUrl, string collectionName)
        {
            if (!PrincipalExist(source, principalUrl))
                return false;

            return (
                from collection in GetPrincipal(source, principalUrl).CalendarCollections
                where collection.Name == collectionName
                select collection
                ).Any();
        }

        /// <summary>
        ///     return a collection for a given user and collectionName
        /// </summary>
        /// <param name="source"></param>
        /// <param name="userEmail"></param>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        public static CalendarCollection GetCollection(this CalDavContext source, string userEmail,
            string collectionName)
        {
           
                var principal = source.GetPrincipal(userEmail);
                return
                    source.CalendarCollections.FirstOrDefault(c => c.Name == collectionName && c.PrincipalId == principal.PrincipalId);
            }
           

        /// <summary>
        ///     Check if a CalendarResource Exist
        /// </summary>
        /// <param name="source"></param>
        /// <param name="principalUrl"></param>
        /// <param name="collectionName"></param>
        /// <param name="calResource"></param>
        /// <returns></returns>
        public static bool CalendarResourceExist(this CalDavContext source, string principalUrl, string collectionName,
            string calResource)
        {
            if (!CollectionExist(source, principalUrl, collectionName))
                return false;

            return (
                from resource in GetCollection(source, principalUrl, collectionName).CalendarResources
                where resource.Name == calResource
                select resource
                ).Any();
        }

        /// <summary>
        ///     Get the calendarResourse object in the given collection
        ///     by the given resource's name.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="principalUrl"></param>
        /// <param name="collectionName">The collection where is the resource</param>
        /// <param name="calResource">the name of the resource</param>
        /// <returns>return a calendar resource by the given name.</returns>
        public static CalendarResource GetCalendarResource(this CalDavContext source, string principalUrl,
            string collectionName, string calResource)
        {
                var collection = source.GetCollection(principalUrl, collectionName);
                return source.CalendarResources.FirstOrDefault(cr => cr.Name == calResource && cr.CalendarCollectionId==collection.CalendarCollectionId);
        }


        public static void ClearDB(this CalDavContext ctx)
        {
            if (ctx.Users.Any())
                ctx.Users.RemoveRange(ctx.Users.Include(x => x.Principal));
            if (ctx.Principals.Any())
                ctx.Principals.RemoveRange(ctx.Principals);
            if (ctx.CalendarCollections.Any())
                ctx.CalendarCollections.RemoveRange(ctx.CalendarCollections.Include(x => x.CalendarResources));
            if (ctx.CalendarResources.Any())
                ctx.CalendarResources.RemoveRange(ctx.CalendarResources);
            ctx.SaveChanges();
        }


        //TODO: Adriano ver esto
        /// <param name="ownerName"></param>
        /// <param name="colName"></param>
        /// <returns></returns>
        //public static IEnumerable<CalendarResource> TimeRangeFilter(this CalDavContext source, DateTime starTime,
        //    DateTime endTime, string ownerName, string colName)
        //{
        //    //TODO: expand the recurrence instances of the resources.
        //    //TODO: convert the datetimes to the UTC of the request
        //    //TODO: evaluate the condition depending on the definition fo the DTEND and DURATION(pg64 CALDAV)
        //    //TODO: check for when the dtStartTime if of type date.
        //    var resources= source.GetCollection(ownerName, colName).CalendarResources;

        //    var output =
        //        resources.Select(resource => resource)
        //            .Where(resource =>
        //            {
        //                //If the comp defines a DTEND property then should be use
        //                if (resource.DtEnd != DateTime.MaxValue)
        //                    return starTime < resource.DtEnd && endTime > resource.DtStart;
        //                //if exist the DURATION property
        //                if (resource.Duration != "")
        //                {
        //                    DurationType duration;
        //                    var result = resource.Duration.ToDuration(out duration);
        //                    var startPlusDuration = resource.DtStart.AddDuration(duration);
        //                    if (duration.IsPositive)
        //                        return starTime < startPlusDuration && endTime > resource.DtStart;
        //                    else
        //                        return starTime <= resource.DtStart && endTime > resource.DtStart;
        //                }
        //                //if there is not DTEND nor DURATION then this is the default behavior
        //                return starTime < resource.DtStart.AddDays(1) && endTime > resource.DtStart;
        //            });
        //    return output;

        //}
    }
}