using System.Collections.Generic;
using System.Threading.Tasks;
using ICalendar.ComponentProperties;
using Microsoft.AspNetCore.Http;

namespace CalDAV.Core
{
    public interface ICalDav
    {
        /// <summary>
        ///     CalDAV HTTP Method for create a new collection of COR (new calendar)
        /// </summary>
        /// <param name="httpContext"></param>
        Task MkCalendar(HttpContext httpContext);

        /// <summary>
        ///     WebDAV PROFIND HTTP Method .
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        Task PropFind(HttpContext httpContext);

        /// <summary>
        /// Synchronization Method for read the calendar home set properties.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        Task CHSetPropfind(HttpContext httpContext);

        /// <summary>
        ///     CalDav HTTP Method PROPPATCH
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        Task PropPatch(HttpContext httpContext);

        /// <summary>
        ///     CalDav HTTP Method REPORT for Calendar Collections
        ///     Call this method form the controller and will handle
        ///     the report for the collections.
        /// </summary>
        /// <returns></returns>
        Task Report(HttpContext context);

        /// <summary>
        ///     CalDAV PUT for create a new COR
        /// </summary>
        /// <param name="httpContext"></param>
        Task AddCalendarObjectResource(HttpContext httpContext);

        /// <summary>
        ///     CalDav Method for delete a Calendar Object Resource
        /// </summary>
        /// <param name="httpContext"></param>
        Task<bool> DeleteCalendarObjectResource(HttpContext httpContext);

        /// <summary>
        ///     CalDav & WebDav Method for delete a Collection
        /// </summary>
        /// <param name="url"></param>
        /// <param name="httpContext"></param>
        Task<bool> DeleteCalendarCollection(string url, HttpContext httpContext);

        /// <summary>
        ///     CalDav HTTP Method Get for a COR
        /// </summary>
        /// <returns></returns>
        /// <param name="httpContext"></param>
        Task ReadCalendarObjectResource(HttpContext httpContext);

        /// <summary>
        ///     CalDav Http method for get a Calendar Collection
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        string ReadCalendarCollection(HttpContext httpContext);

        /// <summary>
        ///     This method perfoms a profind on a principal.
        /// </summary>
        /// <param name="httpContext">
        ///     This contains the Request from the client, the response to be sended back and useful
        ///     data in the Session.
        /// </param>
        /// <returns></returns>
        Task ACLProfind(HttpContext httpContext);
    }
}