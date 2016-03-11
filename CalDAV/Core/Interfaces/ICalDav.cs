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
        /// <param name="userName"></param>
        /// <param name="collectionName"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        string MkCalendar(string userName, string collectionName, Stream body);
        /// <summary>
        /// WebDAV HTTP Method
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="collectionName"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        string PropFind(string userName, string collectionName, Stream body);
        /// <summary>
        /// CalDav HTTP Method PROPPATCH
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="collectionName"></param>
        /// <param name="Body"></param>
        /// <returns></returns>
        string PropPatch(string userName, string collectionName, Stream Body);
        /// <summary>
        /// CalDav HTTP Method REPORT
        /// </summary>
        /// <param name="username"></param>
        /// <param name="collectionName"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        string Report(string username, string collectionName, Stream body);
        /// <summary>
        /// CalDAV PUT for create a new COR
        /// </summary>
        /// <param name="username"></param>
        /// <param name="collectionName"></param>
        /// <param name="resourceId"></param>
        /// <param name="body"></param>
        void AddCalendarObjectResource(string username, string collectionName, string resourceId, Stream body);
        /// <summary>
        /// CalDav Method for delete a Calendar Object Resource
        /// </summary>
        /// <param name="username"></param>
        /// <param name="collectionName"></param>
        /// <param name="resourceId"></param>
        bool DeleteCalendarObjectResource(string username, string collectionName, string resourceId);
        /// <summary>
        /// CalDav & WebDav Method for delete a Collection
        /// </summary>
        /// <param name="username"></param>
        /// <param name="collectionName"></param>
        bool DeleteCalendarCollection(string username, string collectionName);
        /// <summary>
        /// CalDav HTTP Method Get for a COR
        /// </summary>
        /// <param name="username"></param>
        /// <param name="collectionName"></param>
        /// <param name="resourceID"></param>
        /// <returns></returns>
        string ReadCalendarObjectResource(string username, string collectionName, string resourceID);
        /// <summary>
        /// CalDav Http method for get a Calendar Collection
        /// </summary>
        /// <param name="username"></param>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        string ReadCalendarCollection(string username, string collectionName);
    }
}
