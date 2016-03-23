using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalDAV.XML_Processors;
using CalDAV.Models;
using CalDAV.Utils.XML_Processors;
using TreeForXml;

namespace CalDAV.CALDAV_Properties
{
    public static class CollectionProperties
    {
        public static string CaldavNs => "urn:ietf:params:xml:ns:caldav";
        public static string DavNs => "DAV";

        /// <summary>
        /// Returns the value of a collection property given its name.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static string ResolveProperty(this CalendarCollection collection, string propertyName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns all the properties of a collection that must be returned for
        /// an "allprop" property method of Propfind.
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static List<XmlTreeStructure> GetAllVisibleProperties(this CalendarCollection collection)
        {
            var list = new List<XmlTreeStructure>();

            //calendar desription
            var description = new XmlTreeStructure("calendar-description", CaldavNs);
            description.AddNamespace("C", CaldavNs);
            description.AddValue(collection.CalendarDescription);
            list.Add(description);

            //Display Name
            var displayName = new XmlTreeStructure("displayname", DavNs);
            displayName.AddValue(collection.DisplayName);
            list.Add(displayName);

            //resource type
            var resourceType = collection.ResourceType;
            //var resourceType = new XMLTreeStructure("resourcetype", new List<string>() {"D"});
            //foreach (var res in collection.ResourceType)
            //{
            //    resourceType.AddChild(new XMLTreeStructure(res, new List<string>() { NameSpace }));
            //}
            list.Add(resourceType);

            //creation date
            var creationDate = new XmlTreeStructure("creationdate", DavNs);
            creationDate.AddValue(collection.CreationDate.ToString());
            list.Add(creationDate);

            //supported lock

            

            return list;

        }

        /// <summary>
        /// Returns the Name of all collection properties.
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static List<XmlTreeStructure> GetAllPropertyNames(this CalendarCollection collection)
        {
            var list = new List<XmlTreeStructure>();

            //calendar desription
            var description = new XmlTreeStructure("calendar-description", CaldavNs);
            description.AddNamespace("C", CaldavNs);
            list.Add(description);
            //a todos los que estan abajo le tienes q pasar el MainNs
            //Display Name
            var displayName = new XmlTreeStructure("displayname");
            list.Add(displayName);

            //resource type
            var resourceType = new XmlTreeStructure("resourcetype");
            list.Add(resourceType);

            //calendar-timezone
            var calendarTimeZone = new XmlTreeStructure("calendar-timezone");
            list.Add(resourceType);

            //supported-calendar-component-set
            var supportedCalendarComp = new XmlTreeStructure("supported-calendar-component-set");
            list.Add(supportedCalendarComp);

            //max-resource-size
            var maxResourceSize = new XmlTreeStructure("max-resource-size");
            list.Add(maxResourceSize);

            //min-date-time
            var minDateTime = new XmlTreeStructure("min-date-time");
            list.Add(minDateTime);

            //min-date-time
            var maxDateTime = new XmlTreeStructure("min-date-time");
            list.Add(maxDateTime);

            //max-intances
            var maxIntances = new XmlTreeStructure("max-intances");
            list.Add(maxIntances);


            return list;
        }

        /// <summary>
        /// Provides a human-readable description of the calendar collection
        /// </summary>
        /// <param name="description">The calendar collection description.</param>
        /// <returns>The XML with description.</returns>
        public static string CalendarDescription(this CalendarCollection collection, string userEmail,
            string collectionName)
        {
            return XML_Processors.XMLBuilders.XmlBuilder("calendar-description", CaldavNs,
                collection.CalendarDescription);
        }

        /// <summary>
        /// Purpose: Specifies a time zone on a calendar collection.
        /// Conformance: This property SHOULD be defined on all calendar collections. If defined, it SHOULD NOT
        /// be returned by a PROPFIND DAV:allprop request
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="userEmail"></param>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        public static string CalendarTimeZone(this CalendarCollection collection, string userEmail,
            string collectionName)
        {
            return XML_Processors.XMLBuilders.XmlBuilder("calendar-timezone", CaldavNs, collection.CalendarTimeZone);
        }

        /// <summary>
        /// Purpose: This property SHOULD be defined on all calendar collections. If defined, it SHOULD NOT
        /// be returned by a PROPFIND DAV:allprop request.
        /// Conformance: This property SHOULD be defined on all calendar collections. If defined, it SHOULD NOT
        /// be returned by a PROPFIND DAV:allprop request
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="userEmail"></param>
        /// <param name="collectionName"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Purpose: Provides a numeric value indicating the maximum size of a resource in octets that the server
        /// is willing to accept when a calendar object resource is stored in a calendar collection.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="userEmail"></param>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        public static string MaxResourcesSize(this CalendarCollection collection, string userEmail,
            string collectionName)
        {
            return XML_Processors.XMLBuilders.XmlBuilder("max-resource-size", CaldavNs,
                collection.MaxResourceSize.ToString());
        }

        /// <summary>
        /// Purpose: Provides a DATE-TIME value indicating the earliest date and time (in UTC) that the server is
        /// willing to accept for any DATE or DATE-TIME value in a calendar object resource stored in
        ///  a calendar collection.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="userEmail"></param>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        public static string MinDateTime(this CalendarCollection collection, string userEmail, string collectionName)
        {
            return XML_Processors.XMLBuilders.XmlBuilder("min-date-time", CaldavNs, collection.MinDateTime.ToString());
        }

        /// <summary>
        /// Purpose: Provides a DATE-TIME value indicating the latest date and time (in UTC) that the server is
        /// willing to accept for any DATE or DATE-TIME value in a calendar object resource stored in
        /// a calendar collection.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="userEmail"></param>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        public static string MaxDateTime(this CalendarCollection collection, string userEmail, string collectionName)
        {
            return XML_Processors.XMLBuilders.XmlBuilder("max-date-time", CaldavNs, collection.MaxDateTime.ToString());
        }

        /// <summary>
        ///  Purpose: Provides a numeric value indicating the maximum number of recurrence instances that a
        /// calendar object resource stored in a calendar collection can generate.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="userEmail"></param>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        public static string MaxIntances(this CalendarCollection collection, string userEmail, string collectionName)
        {
            return XML_Processors.XMLBuilders.XmlBuilder("max-intances", CaldavNs, collection.MaxIntences.ToString());
        }

        /// <summary>
        /// Provides the type of supported collections.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="userEmail"></param>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        //TODO: Fix this method it has to return multiples values 
        public static string ResourceType(this CalendarCollection collection, string userEmail, string collectionName)
        {
            return "";
            // return XML_Processors.XMLBuilders.XmlBuilder("resourcetype", CaldavNs, collection.ResourceType);
        }
    }
}
