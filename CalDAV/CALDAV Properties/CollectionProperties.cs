using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CalDAV.XML_Processors;
using CalDAV.Models;
using CalDAV.Utils.XML_Processors;
using TreeForXml;
using System.Reflection;

namespace CalDAV.CALDAV_Properties
{
    public static class CollectionProperties
    {
        public static string CaldavNs => "urn:ietf:params:xml:ns:caldav";
        public static string DavNs => "DAV";

        /// <summary>
        /// Contains all the properties that are common for all Calendar Collection.
        /// </summary>
        private static Dictionary<string, string> XmlGeneralProperties = new Dictionary<string, string>()
        {
            { "calendar-timezone", DateTimeKind.Local.ToString() }, {"max-resource-size", "102400"},
            { "min-date-time", MinDateTime()}, {"max-date-time", MaxDateTime()}, { "max-instances", "10"},
            {"getcontentlength", "0" }
        };

        private static List<string> VisibleGeneralProperties = new List<string>()
        {
            "displayname", "resourcetype", "creationdate", "calendar-description", "getcontenttype"
        };

        private static string MinDateTime()
        {
            return
                new DateTime(DateTime.Now.Year, (DateTime.Now.Month - 1) % 12, DateTime.Now.Day).ToUniversalTime()
                    .ToString("yyyyMMddTHHmmssZ");
        }

        private static string MaxDateTime()
        {
            return
                   new DateTime(DateTime.Now.Year, (DateTime.Now.Month + 1) % 12, DateTime.Now.Day).ToUniversalTime()
                       .ToString("yyyyMMddTHHmmssZ");
        }

        /// <summary>
        /// Returns the value of a collection property given its name.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="propertyName"></param>
        /// <param name="mainNS"></param>
        /// <returns></returns>
        public static XmlTreeStructure ResolveProperty(this CalendarCollection collection, string propertyName, string mainNS)
        {
            //First I look to see if is one of the static ones.
            if (XmlGeneralProperties.ContainsKey(propertyName))
            {
                var svalue = XmlGeneralProperties[propertyName];
                var sprop = new XmlTreeStructure(propertyName, mainNS);
                sprop.AddValue(svalue);
                return sprop;
            }
            //TODO: Check all property names
            //this must be fixed later because not all properties are of type string.
            var realPropName = propertyName.ToLower();
            string value;
            realPropName = realPropName[0].ToString().ToUpper() + realPropName.Substring(1);
            realPropName = realPropName.Replace("-", "");
            try
            {
                value = (string)collection.GetType().GetProperty(realPropName).GetValue(collection);
            }
            catch (Exception)
            {
                value = null;
                throw new Exception("The value could not be retrieved");
            }
            XmlTreeStructure prop;
            try
            {

                //TODO: Why this motherfucker is not parsing ok.
                if (value != null)
                    prop = (XmlTreeStructure)XmlTreeStructure.Parse(value);
                else
                    return null;
                
            }
            catch (Exception e)
            {
                prop = null;
                //throw new Exception("The Property Value Could Not Be Parsed");
                throw e;
            }

            return prop;
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
            foreach (var property in VisibleGeneralProperties)
            {
                list.Add(ResolveProperty(collection, property, "DAV"));
            }
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

            foreach (var property in XmlGeneralProperties)
            {
                list.Add(new XmlTreeStructure(property.Key, DavNs));
            }

            //calendar desription
            var description = new XmlTreeStructure("calendar-description", CaldavNs);
            description.AddNamespace("C", CaldavNs);
            list.Add(description);
            //a todos los que estan abajo le tienes q pasar el MainNs
            //Display Name
            var displayName = new XmlTreeStructure("displayname", DavNs);
            list.Add(displayName);

            //resource type
            var resourceType = new XmlTreeStructure("resourcetype", DavNs);
            list.Add(resourceType);

            //calendar-timezone
            var calendarTimeZone = new XmlTreeStructure("calendar-timezone", DavNs);
            list.Add(resourceType);

            //supported-calendar-component-set
            var supportedCalendarComp = new XmlTreeStructure("supported-calendar-component-set", DavNs);
            list.Add(supportedCalendarComp);

            //max-resource-size
            var maxResourceSize = new XmlTreeStructure("max-resource-size", DavNs);
            list.Add(maxResourceSize);

            //min-date-time
            var minDateTime = new XmlTreeStructure("min-date-time", DavNs);
            list.Add(minDateTime);

            //min-date-time
            var maxDateTime = new XmlTreeStructure("min-date-time", DavNs);
            list.Add(maxDateTime);

            //max-intances
            var maxIntances = new XmlTreeStructure("max-intances", DavNs);
            list.Add(maxIntances);


            return list;
        }

    }
}
