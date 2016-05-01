using System.Collections.Generic;
using System.Net;
using DataLayer;
using Microsoft.AspNet.Http;

namespace CalDAV.Core.ConditionsCheck.Preconditions
{
    public class PropfindPrecondition:IPrecondition
    {
        private CalDavContext db { get; }

        public PropfindPrecondition(CalDavContext context)
        {
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
                response.StatusCode = (int) HttpStatusCode.NotFound;
                return false;
            }
            if (calendarResourceId !=null && !db.CalendarResourceExist(url))
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
                return false;
            }
            return true;
        }
    }
}
