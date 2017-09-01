using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACL.Core.Authentication;
using ACL.Core.CheckPermissions;
using CalDAV.Core.Interfaces;
using CalDAV.Core.Method_Extensions;
using CalDAV.Method_Extensions;
using CalDAV.Utils;
using DataLayer;
using DataLayer.Models.NonMappedEntities;
using ICalendar.Calendar;
using Microsoft.AspNetCore.Http;
using TreeForXml;

namespace CalDAV.Core.BusinessServices
{
    /// <inheritdoc />
    public class EasyCalendarService:IEasyCalendarService
    {
        private readonly IFileManagement _fileManagement;
        private readonly IPermissionChecker _permissionChecker;
        private readonly IAuthenticate _authenticate;

        public EasyCalendarService(IFileManagement fileManagement, IPermissionChecker permissionChecker, IAuthenticate authenticate)
        {
            _permissionChecker = permissionChecker;
            _fileManagement = fileManagement;
            _authenticate = authenticate;
        }
        /// <inheritdoc />
        public async Task<IEnumerable<EasyCalendarEvent>> GetEasyCalendarEventsWithinMonthRangeAsync(int monthRange, HttpContext context)
        {
            var url = context.Request.GetRealUrlInEasyController();
            var principalUrl = (await _authenticate.AuthenticateRequestAsync(context))?.PrincipalUrl;

            if (!_permissionChecker.CheckPermisionForMethod(url, principalUrl, context.Response,
                SystemProperties.HttpMethod.Get))
                return null;
            

            var strBuilderStart = new StringBuilder();
            var strBuilderEnd = new StringBuilder();
            var start = DateTime.Now.Subtract(new TimeSpan(30*monthRange));
            var end = DateTime.Now.AddMonths(monthRange);
            strBuilderStart.Append(start.ToString("yyyyMMddTHHmmss") + (start.Kind == DateTimeKind.Utc ? "Z" : ""));
            strBuilderEnd.Append(end.ToString("yyyyMMddTHHmmss") + (end.Kind == DateTimeKind.Utc ? "Z" : ""));

            var filterstr = $@"<?xml version=""1.0"" encoding=""utf-8""?>
 <C:filter xmlns:C=""urn:ietf:params:xml:ns:caldav"">
   <C:comp-filter name=""VCALENDAR"">
     <C:comp-filter name=""VEVENT"">
       <C:time-range start=""{start}"" end=""{end}""/>
     </C:comp-filter> 
   </C:comp-filter>  
 </C:filter>";

            var filter = XmlTreeStructure.Parse(filterstr);

           
            var userResources = new Dictionary<string, string>();
            await _fileManagement.GetAllCalendarObjectResource(url, userResources);
            var userCalendars = userResources.ToDictionary(userResource => userResource.Key,
                userResource => VCalendar.Parse(userResource.Value));

            //apply the filters to the calendars
            var filteredCalendars = userCalendars.Where(x => x.Value.FilterResource(filter));
            var easyEvents = filteredCalendars.Select(fc => fc.Value.ToEasyCalendarEvent()).ToList();
            return easyEvents;
        }
    }
}
