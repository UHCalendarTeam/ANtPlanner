using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalDAV.Models;
using CalDAV.Utils.XML_Processors;
using TreeForXml;
using System.Reflection;

namespace CalDAV.CALDAV_Properties
{
    public static class CollectionResourceProperties
    {
        public static string CaldavNs => "urn:ietf:params:xml:ns:caldav";
        public static string DavNs => "DAV";

        /// <summary>
        /// Returns the value of a resource property given its name.
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static XmlTreeStructure ResolveProperty(this CalendarResource resource, string propertyName, string mainNS)
        {
            //First I look to see if is one of the static ones.
            if (XmlGeneralProperties.ContainsKey(propertyName))
            {
                var svalue = XmlGeneralProperties[propertyName];
                var sprop = new XmlTreeStructure(propertyName, mainNS);
                sprop.AddValue(svalue);
                return sprop;
            }

            ////this must be fixed later because not all properties are of type string.
            var value = (string)resource.GetType().GetProperty(propertyName).GetValue(resource);
            if(value == null)
                throw new Exception("Property is not accesible or not exist");
            var prop = (XmlTreeStructure)XmlTreeStructure.Parse(value);


            return prop;
        }

        /// <summary>
        /// Contains all the properties that are common for all Calendar Collection Resources.
        /// </summary>
        private static Dictionary<string, string> XmlGeneralProperties = new Dictionary<string, string>()
        {
            {"getcontentlanguage","es"}, {"resourcetype", ""}
        };

        private static List<string> VisibleGeneralProperties = new List<string>()
        {
            "getetag", "displayname", "creationdate", "getcontentlenght", "getcontenttype", "getlastmodified"
        };

        /// <summary>
        /// Returns all the properties of a resource that must be returned for
        /// an "allprop" property method of Propfind. 
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static List<XmlTreeStructure> GetAllVisibleProperties(this CalendarResource calendarResource)
        {
            List<XmlTreeStructure> list = new List<XmlTreeStructure>();
            foreach (var property in VisibleGeneralProperties)
            {
                list.Add(ResolveProperty(calendarResource, property, "DAV"));
            }
            return list;
        }

        /// <summary>
        /// Returns all property names of the resource
        /// </summary>
        /// <param name="calendarResource"></param>
        /// <returns></returns>
        public static List<XmlTreeStructure> GetAllPropertyNames(this CalendarResource calendarResource)
        {
            var list = new List<XmlTreeStructure>();
            //TODO: annadir el ns a los de abajo
            //Display Name
            var displayName = new XmlTreeStructure("displayname");
            list.Add(displayName);

            //resource type
            var resourceType = new XmlTreeStructure("resourcetype");
            list.Add(resourceType);

            //creation date
            var creationDate = new XmlTreeStructure("creationdate");
            list.Add(creationDate);

            //getcontent length
            var getcontentlegth = new XmlTreeStructure("getcontentlenght");
            list.Add(getcontentlegth);

            //getcontenttype
            var getContentType = new XmlTreeStructure("getcontenttype");
            list.Add(getContentType);

            //getetag
            var getEtag = new XmlTreeStructure("getetag");
            list.Add(getEtag);

            //getLastModified
            var getLastModified = new XmlTreeStructure("getlastmodified");
            list.Add(getLastModified);

            //supported lock

            return list;
        }
    }
}
