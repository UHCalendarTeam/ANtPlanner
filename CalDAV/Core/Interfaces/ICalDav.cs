using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CalDAV.Core
{
    public interface ICalDav
    {
        /// <summary>
        /// CalDAV HTTP Method for create a new collection of COR (new calendar)
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="collectionName"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        string MkCalendar(string userEmail, string collectionName, Stream body);
        /// <summary>
        /// WebDAV HTTP Method
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="collectionName"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        string PropFind(string userEmail, string collectionName, Stream body);
        /// <summary>
        /// CalDav HTTP Method PROPPATCH
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="collectionName"></param>
        /// <param name="Body"></param>
        /// <returns></returns>
        string PropPatch(string userEmail, string collectionName, Stream Body);
        /// <summary>
        /// CalDav HTTP Method REPORT
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="collectionName"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        string Report(string userEmail, string collectionName, Stream body);
        /// <summary>
        /// CalDAV PUT for create a new COR
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="collectionName"></param>
        /// <param name="resourceId"></param>
        /// <param name="body"></param>
        void AddCalendarObjectResource(string userEmail, string collectionName, string resourceId, Stream body);
        /// <summary>
        /// CalDav Method for delete a Calendar Object Resource
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="collectionName"></param>
        /// <param name="resourceId"></param>
        bool DeleteCalendarObjectResource(string userEmail, string collectionName, string resourceId);
        /// <summary>
        /// CalDav & WebDav Method for delete a Collection
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="collectionName"></param>
        bool DeleteCalendarCollection(string userEmail, string collectionName);

        /// <summary>
        /// CalDav HTTP Method Get for a COR
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="collectionName"></param>
        /// <param name="resourceId"></param>
        /// <param name="etag"></param>
        /// <returns></returns>
        string ReadCalendarObjectResource(string userEmail, string collectionName, string resourceId, out string etag);
        /// <summary>
        /// CalDav Http method for get a Calendar Collection
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        string ReadCalendarCollection(string userEmail, string collectionName);
    }
}
