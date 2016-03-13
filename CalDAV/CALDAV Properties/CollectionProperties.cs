using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalDAV.XML_Processors;
using CalDAV.Models;

namespace CalDAV.CALDAV_Properties
{
    public class CollectionProperties : ICollectionProperties
    {
        public string NameSpace => "urn:ietf:params:xml:ns:caldav";
        public string CalendarDescription(string userEmail, string collectionName)
        {           
            using(var db = new CalDavContext())
            {
                var collection = db.GetCollection(userEmail, collectionName);
                return XML_Processors.XMLBuilders.XmlBuilder("calendar-description", NameSpace, collection.CalendarDescription);
            }
        }

        public string CalendarTimeZone(string userEmail, string collectionName)
        {
            using (var db = new CalDavContext())
            {
                var collection = db.GetCollection(userEmail, collectionName);
                return XML_Processors.XMLBuilders.XmlBuilder("calendar-timezone", NameSpace, collection.CalendarTimeZone);
            }
        }

        public string SupportedCalendarComponentSet(string userEmail, string collectionName)
        {
            //TODO: fix this
            return "";
            /* using (var db = new CalDavContext())
            {
                var collection = db.GetCollection(userEmail, collectionName);
                return XML_Processors.XMLBuilders.XmlBuilder("supported-calendar-component-set", NameSpace, collection.SupportedCalendarComponentSet);
            }*/
        }

        public string MaxResourcesSize(string userEmail, string collectionName)
        {
            using (var db = new CalDavContext())
            {
                var collection = db.GetCollection(userEmail, collectionName);
                return XML_Processors.XMLBuilders.XmlBuilder("max-resource-size", NameSpace, collection.MaxResourceSize.ToString());
            }
        }

        public string MinDateTime(string userEmail, string collectionName)
        {
            using (var db = new CalDavContext())
            {
                var collection = db.GetCollection(userEmail, collectionName);
                return XML_Processors.XMLBuilders.XmlBuilder("min-date-time", NameSpace, collection.MinDateTime.ToString());
            }
        }

        public string MaxDateTime(string userEmail, string collectionName)
        {
            using (var db = new CalDavContext())
            {
                var collection = db.GetCollection(userEmail, collectionName);
                return XML_Processors.XMLBuilders.XmlBuilder("max-date-time", NameSpace, collection.MaxDateTime.ToString());
            }
        }

        public string MaxIntances(string userEmail, string collectionName)
        {
            using (var db = new CalDavContext())
            {
                var collection = db.GetCollection(userEmail, collectionName);
                return XML_Processors.XMLBuilders.XmlBuilder("max-intances", NameSpace, collection.MaxIntences.ToString());
            }
        }

        //TODO: Fix this method it has to return multiples values 
        public string ResourceType(string userEmail, string collectionName)
        {
            using (var db = new CalDavContext())
            {
                var collection = db.GetCollection(userEmail, collectionName);
                return XML_Processors.XMLBuilders.XmlBuilder("resourcetype", NameSpace, collection.ResourceType.ToString());
            }
        } 
    }
}
