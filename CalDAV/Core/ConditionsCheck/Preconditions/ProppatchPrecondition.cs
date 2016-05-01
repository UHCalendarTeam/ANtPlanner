using System.Collections.Generic;
using System.Net;
using DataLayer;
using Microsoft.AspNet.Http;

namespace CalDAV.Core.ConditionsCheck.Preconditions
{
    public class ProppatchPrecondition : IPrecondition
    {
        public IFileSystemManagement Manager { get; set; }
        private CalDavContext db { get; }

        public ProppatchPrecondition(IFileSystemManagement manager, CalDavContext context)
        {
            Manager = manager;
            db = context;
        }

        public bool PreconditionsOK(Dictionary<string, string> propertiesAndHeaders, HttpResponse response)
        {
            string calendarResourceId;
            propertiesAndHeaders.TryGetValue("calendarResourceID", out calendarResourceId);

            string url;
            propertiesAndHeaders.TryGetValue("url", out url);

            if (calendarResourceId == null && !db.CollectionExist(url))
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
                return false;
            }
            if (calendarResourceId != null && !db.CalendarResourceExist(url))
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
                return false;
            }
            return true;
        }
    }
}
