using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalDAV.Models;

namespace CalDAV.CALDAV_Properties
{
    public static class CollectionResourceProperties
    {
        public static string NameSpace => "urn:ietf:params:xml:ns:caldav";

        public static List<KeyValuePair<string, string>> GetAllVisibleProperties(this CalendarResource calendarResource)
        {
            throw new NotImplementedException();
        }
        
    }
}
