using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalDAV.XML_Processors;
using CalDAV.Models;
using CalDAV.Utils.XML_Processors;

namespace CalDAV.CALDAV_Properties
{
    public static class CollectionProperties
    {
        public static string NameSpace => "urn:ietf:params:xml:ns:caldav";

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
        public static List<XMLTreeStructure> GetAllVisibleProperties(this CalendarCollection collection)
        {
            var list = new List<XMLTreeStructure>();

            //calendar desription
            var description = new XMLTreeStructure("calendar-description", new List<string>() { "C" });
            description.AddAttribute("C", NameSpace);
            description.AddValue(collection.CalendarDescription);
            list.Add(description);

            //Display Name
            var displayName = new XMLTreeStructure("displayname", new List<string>() { "D" });
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

            return list;

        }

        /// <summary>
        /// Returns the Name of all collection properties.
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static List<XMLTreeStructure> GetAllPropertyNames(this CalendarCollection collection)
        {
            var list = new List<XMLTreeStructure>();

            //calendar desription
            var description = new XMLTreeStructure("calendar-description");
            description.AddAttribute("C", NameSpace);
            list.Add(description);

            //Display Name
            var displayName = new XMLTreeStructure("displayname");
            list.Add(displayName);

            //resource type
            var resourceType = new XMLTreeStructure("resourcetype");
            list.Add(resourceType);

            //calendar-timezone
            var calendarTimeZone = new XMLTreeStructure("calendar-timezone");
            list.Add(resourceType);

            //supported-calendar-component-set
            var supportedCalendarComp = new XMLTreeStructure("supported-calendar-component-set");
            list.Add(supportedCalendarComp);

            //max-resource-size
            var maxResourceSize = new XMLTreeStructure("max-resource-size");
            list.Add(maxResourceSize);

            //min-date-time
            var minDateTime = new XMLTreeStructure("min-date-time");
            list.Add(minDateTime);

            //min-date-time
            var maxDateTime = new XMLTreeStructure("min-date-time");
            list.Add(maxDateTime);

            //max-intances
            var maxIntances = new XMLTreeStructure("max-intances");
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
            return XML_Processors.XMLBuilders.XmlBuilder("calendar-description", NameSpace,
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
            return XML_Processors.XMLBuilders.XmlBuilder("calendar-timezone", NameSpace, collection.CalendarTimeZone);
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
            return XML_Processors.XMLBuilders.XmlBuilder("max-resource-size", NameSpace,
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
            return XML_Processors.XMLBuilders.XmlBuilder("min-date-time", NameSpace, collection.MinDateTime.ToString());
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
            return XML_Processors.XMLBuilders.XmlBuilder("max-date-time", NameSpace, collection.MaxDateTime.ToString());
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
            return XML_Processors.XMLBuilders.XmlBuilder("max-intances", NameSpace, collection.MaxIntences.ToString());
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
            return XML_Processors.XMLBuilders.XmlBuilder("resourcetype", NameSpace, collection.ResourceType.ToString());
        }
    }
}
