using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalDAV.XML_Processors;
using CalDAV.Models;

namespace CalDAV.CALDAV_Properties
{
    public static class CollectionProperties
    {
        public static string NameSpace => "urn:ietf:params:xml:ns:caldav";

        public static string ResolveProperty(this CalendarCollection collection, string propertyName)
        {
            throw new NotImplementedException();
        }

        public static string CalendarDescription(this CalendarCollection collection, string userEmail,
            string collectionName)
        {

            return XML_Processors.XMLBuilders.XmlBuilder("calendar-description", NameSpace,
                collection.CalendarDescription);

        }

        public static string CalendarTimeZone(this CalendarCollection collection, string userEmail,
            string collectionName)
        {



            return XML_Processors.XMLBuilders.XmlBuilder("calendar-timezone", NameSpace, collection.CalendarTimeZone);

        }

        public static string SupportedCalendarComponentSet(this CalendarCollection collection, string userEmail,
            string collectionName)
        {
            //TODO: fix this
            return "";
            /* 
            {                var collection = db.GetCollection(userEmail, collectionName);
              NameSpace, collection.SupportedCalendarComponentSet);
            }*/
        }

        public static string MaxResourcesSize(this CalendarCollection collection, string userEmail,
            string collectionName)
        {



            return XML_Processors.XMLBuilders.XmlBuilder("max-resource-size", NameSpace,
                collection.MaxResourceSize.ToString());

        }

        public static string MinDateTime(this CalendarCollection collection, string userEmail, string collectionName)
        {



            return XML_Processors.XMLBuilders.XmlBuilder("min-date-time", NameSpace, collection.MinDateTime.ToString());

        }

        public static string MaxDateTime(this CalendarCollection collection, string userEmail, string collectionName)
        {



            return XML_Processors.XMLBuilders.XmlBuilder("max-date-time", NameSpace, collection.MaxDateTime.ToString());

        }

        public static string MaxIntances(this CalendarCollection collection, string userEmail, string collectionName)
        {



            return XML_Processors.XMLBuilders.XmlBuilder("max-intances", NameSpace, collection.MaxIntences.ToString());

        }

        //TODO: Fix this method it has to return multiples values 
        public static string ResourceType(this CalendarCollection collection, string userEmail, string collectionName)
        {



            return XML_Processors.XMLBuilders.XmlBuilder("resourcetype", NameSpace, collection.ResourceType.ToString());

        }
    }
}
