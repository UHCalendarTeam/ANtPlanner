using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DataLayer;
using Microsoft.AspNet.Http;

namespace CalDAV.Core.ConditionsCheck.Preconditions
{
    public class ProppatchPrecondition : IPrecondition
    {
        private CalDavContext db { get; set; }
        private IFileSystemManagement fs { get; set; }

        public ProppatchPrecondition(IFileSystemManagement manager, CalDavContext context)
        {
            db = context;
            fs = manager;
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
