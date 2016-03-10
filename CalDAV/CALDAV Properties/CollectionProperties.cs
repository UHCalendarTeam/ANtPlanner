using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalDAV.XML_Processors;

namespace CalDAV.CALDAV_Properties
{
    public class CollectionProperties : ICollectionProperties
    {
        public string NameSpace => "urn:ietf:params:xml:ns:caldav";
        public string CalendarDescription(string description)
        {
            return XML_Processors.XMLBuilders.XmlBuilder("calendar-description", NameSpace, description);
        }

        public string CalendarTimeZone(string timeZone)
        {
            return XML_Processors.XMLBuilders.XmlBuilder("calendar-timezone", NameSpace, timeZone);
        }

        public string SupportedCalendarComponentSet(string components)
        {
            return XML_Processors.XMLBuilders.XmlBuilder("supported-calendar-component-set", NameSpace, components);
        }

        public string MaxResourcesSize(string size)
        {
            return XML_Processors.XMLBuilders.XmlBuilder("max-resource-size", NameSpace, size);
        }

        public string MinDateTime(string date)
        {
            return XML_Processors.XMLBuilders.XmlBuilder("min-date-time", NameSpace, date);
        }

        public string MaxDateTime(string date)
        {
            return XML_Processors.XMLBuilders.XmlBuilder("max-date-time", NameSpace, date);
        }

        public string MaxIntances(string intances)
        {
            return XML_Processors.XMLBuilders.XmlBuilder("max-intances", NameSpace, intances);
        }
    }
}
