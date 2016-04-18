using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using TreeForXml;


namespace CalDAV.Core
{
    public interface ICalDav
    {
        /// <summary>
        /// CalDAV HTTP Method for create a new collection of COR (new calendar)
        /// </summary>
        /// <param name="propertiesAndHeaders"></param>
        /// <param name="body"></param>
        /// <param name="response"></param>
        Task MkCalendar(Dictionary<string, string> propertiesAndHeaders, string body, HttpResponse response);

        /// <summary>
        /// WebDAV HTTP Method
        /// </summary>
        /// <param name="propertiesAndHeaders"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        XmlTreeStructure PropFind(Dictionary<string, string> propertiesAndHeaders, string  body);

        /// <summary>
        /// CalDav HTTP Method PROPPATCH
        /// </summary>
        /// <param name="propertiesAndHeaders"></param>
        /// <param name="Body"></param>
        /// <returns></returns>
        string PropPatch(Dictionary<string, string> propertiesAndHeaders, string Body);

        /// <summary>
        /// CalDav HTTP Method REPORT for Calendar Collections
        /// </summary>
        /// <param name="propertiesAndHeaders"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        string Report(Dictionary<string, string> propertiesAndHeaders, string body);

        /// <summary>
        /// CalDAV PUT for create a new COR
        /// </summary>
        /// <param name="propertiesAndHeaders"></param>
        /// <param name="response"></param>
        Task AddCalendarObjectResource(Dictionary<string, string> propertiesAndHeaders, HttpResponse response);

        /// <summary>
        /// CalDav Method for delete a Calendar Object Resource
        /// </summary>
        /// <param name="propertiesAndHeaders"></param>
        /// <param name="response"></param>
        bool DeleteCalendarObjectResource(Dictionary<string, string> propertiesAndHeaders, HttpResponse response);

        /// <summary>
        /// CalDav & WebDav Method for delete a Collection
        /// </summary>
        /// <param name="propertiesAndHeaders"></param>
        /// <param name="response"></param>
        bool DeleteCalendarCollection(Dictionary<string, string> propertiesAndHeaders, HttpResponse response);

        /// <summary>
        /// CalDav HTTP Method Get for a COR
        /// </summary>
        /// <returns></returns>
        /// <param name="propertiesAndHeaders"></param>
        /// <param name="response"></param>
        Task ReadCalendarObjectResource(Dictionary<string, string> propertiesAndHeaders, HttpResponse response);

        /// <summary>
        /// CalDav Http method for get a Calendar Collection
        /// </summary>
        /// <param name="propertiesAndHeaders"></param>
        /// <returns></returns>
        string ReadCalendarCollection(Dictionary<string, string> propertiesAndHeaders);

    }
}
