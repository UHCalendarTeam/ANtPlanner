using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CalDAV.Utils.XML_Processors;

namespace CalDAV.Core
{
    public interface ICalDav
    {
        /// <summary>
        /// CalDAV HTTP Method for create a new collection of COR (new calendar)
        /// </summary>
        /// <param name="propertiesAndHeaders"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        string MkCalendar(Dictionary<string, string> propertiesAndHeaders, string body);

        /// <summary>
        /// WebDAV HTTP Method
        /// </summary>
        /// <param name="propertiesAndHeaders"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        XMLTreeStructure PropFind(Dictionary<string, string> propertiesAndHeaders, string  body);

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
        /// <param name="retEtag"></param>
        bool AddCalendarObjectResource(Dictionary<string, string> propertiesAndHeaders, out string retEtag);

        /// <summary>
        /// CalDav Method for delete a Calendar Object Resource
        /// </summary>
        /// <param name="propertiesAndHeaders"></param>
        bool DeleteCalendarObjectResource(Dictionary<string, string> propertiesAndHeaders);

        /// <summary>
        /// CalDav & WebDav Method for delete a Collection
        /// </summary>
        /// <param name="propertiesAndHeaders"></param>
        bool DeleteCalendarCollection(Dictionary<string, string> propertiesAndHeaders);

        /// <summary>
        /// CalDav HTTP Method Get for a COR
        /// </summary>
        /// <returns></returns>
        /// <param name="propertiesAndHeaders"></param>
        /// <param name="etag"></param>
        string ReadCalendarObjectResource(Dictionary<string, string> propertiesAndHeaders, out string etag);

        /// <summary>
        /// CalDav Http method for get a Calendar Collection
        /// </summary>
        /// <param name="propertiesAndHeaders"></param>
        /// <returns></returns>
        string ReadCalendarCollection(Dictionary<string, string> propertiesAndHeaders);

    }
}
