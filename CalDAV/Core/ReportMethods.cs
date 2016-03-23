using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalDAV.Models;
using ICalendar.Calendar;
using ICalendar.Utils;
using TreeForXml;

namespace CalDAV.Core
{
    public class ReportMethods:IReportMethods
    {
        private string UserEmail { get; set; }
        private string CollectionName { get; set; }

        

        public ReportMethods(string userEmail, string collectionName)
        {
            UserEmail = userEmail;
            CollectionName = collectionName;
        }

        

        public string ExpandProperty()
        {
            throw new NotImplementedException();
        }

        public string CalendarQuery(IXMLTreeStructure filters )
        {
            //realiza una busqueda y devuelve todos los elementos que cumplan los filtros
            //devuelve todos los COR especificados en el pedido
            //con el uso de del elemento XML calendar-data se puede especificar cuales componentes de calendaerio y cuales propiedades retornar
                //de los COR que cumplan los filtros

            //get the time-filters if has one
            Dictionary<string, string> userResources;
            var fileM = new FileSystemManagement();
            fileM.GetAllCalendarObjectResource(UserEmail, CollectionName, out userResources);
            Dictionary<string, VCalendar> userCalendars = userResources.ToDictionary(userResource => userResource.Key, userResource => new VCalendar(userResource.Value));
            return "";

        }


        private IEnumerable<CalendarResource> TimeRangeFilter(string start, string end)
        {
            DateTime? startTime;
            DateTime? endTime;
            if (!start.ToDateTime(out startTime))
                return null;
            if (!end.ToDateTime(out endTime))
                return null;
            using (var context = new CalDavContext())
            {
               var resources = context.TimeRangeFilter(startTime.Value, endTime.Value, UserEmail, CollectionName);
                return resources;
            }
        }
    }
}
