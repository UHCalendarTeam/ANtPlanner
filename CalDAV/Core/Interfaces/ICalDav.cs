using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;

namespace CalDAV.Core
{
    public interface ICalDav
    {
        /// <summary>
        ///     CalDAV HTTP Method for create a new collection of COR (new calendar)
        /// </summary>
        /// <param name="propertiesAndHeaders"></param>
        /// <param name="body"></param>
        /// <param name="response"></param>
        Task MkCalendar(Dictionary<string, string> propertiesAndHeaders, string body, HttpResponse response);

        /// <summary>
        ///     WebDAV HTTP Method
        /// </summary>
        /// <param name="propertiesAndHeaders"></param>
        /// <param name="body"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        void PropFind(Dictionary<string, string> propertiesAndHeaders, string body, HttpResponse response);

        /// <summary>
        ///     CalDav HTTP Method PROPPATCH
        /// </summary>
        /// <param name="propertiesAndHeaders"></param>
        /// <param name="Body"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        void PropPatch(Dictionary<string, string> propertiesAndHeaders, string Body, HttpResponse response);

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
        /// <param name="propertiesAndHeaders"></param>
        /// <param name="response"></param>
        Task AddCalendarObjectResource(Dictionary<string, string> propertiesAndHeaders, HttpResponse response);

        /// <summary>
        ///     CalDav Method for delete a Calendar Object Resource
        /// </summary>
        /// <param name="propertiesAndHeaders"></param>
        /// <param name="response"></param>
        bool DeleteCalendarObjectResource(Dictionary<string, string> propertiesAndHeaders, HttpResponse response);

        /// <summary>
        ///     CalDav & WebDav Method for delete a Collection
        /// </summary>
        /// <param name="propertiesAndHeaders"></param>
        /// <param name="response"></param>
        bool DeleteCalendarCollection(Dictionary<string, string> propertiesAndHeaders, HttpResponse response);

        /// <summary>
        ///     CalDav HTTP Method Get for a COR
        /// </summary>
        /// <returns></returns>
        /// <param name="propertiesAndHeaders"></param>
        /// <param name="response"></param>
        Task ReadCalendarObjectResource(Dictionary<string, string> propertiesAndHeaders, HttpResponse response);

        /// <summary>
        ///     CalDav Http method for get a Calendar Collection
        /// </summary>
        /// <param name="propertiesAndHeaders"></param>
        /// <returns></returns>
        string ReadCalendarCollection(Dictionary<string, string> propertiesAndHeaders);

        /// <summary>
        /// This method perfoms a profind on a principal.
        /// </summary>
        /// <param name="request">The request from the controller.</param>
        /// <param name="response">The response from the controller.</param>
        /// <param name="data">SOme useful data that could be send from the controller.</param>
        /// <returns></returns>
        Task ACLProfind(HttpRequest request, HttpResponse response, Dictionary<string, string> data);
    }
}