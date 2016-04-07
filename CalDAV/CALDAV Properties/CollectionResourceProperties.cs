using System.Collections.Generic;
using System.Linq;
using CalDAV.Models;
using TreeForXml;

namespace CalDAV.CALDAV_Properties
{
    public static class CollectionResourceProperties
    {
        public static string CaldavNs => "urn:ietf:params:xml:ns:caldav";
        public static string DavNs => "DAV:";

        /// <summary>
        /// Returns the value of a resource property given its name.
        /// If error returns the property without value and puts the error in the
        /// error stack.
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="propertyName"></param>
        /// <param name="mainNs"></param>
        /// <param name="errorStack"></param>
        /// <returns></returns>
        public static XmlTreeStructure ResolveProperty(this CalendarResource resource, string propertyName, string mainNs, Stack<string> errorStack)
        {
            //First I look to see if is one of the static ones.
            //if (XmlGeneralProperties.ContainsKey(propertyName))
            //{
            //    var svalue = XmlGeneralProperties[propertyName];
            //    var sprop = new XmlTreeStructure(propertyName, svalue.Value);
            //    sprop.AddValue(svalue.Key);
            //    return sprop;
            //}

            ////this must be fixed later because not all properties are of type string.
            //var realPropName = propertyName.ToLower();
            //realPropName = realPropName[0].ToString().ToUpper() + realPropName.Substring(1);
            //realPropName = realPropName.Replace("-", "");
            //string value;
            //try
            //{
            //    value = (string)resource.GetType().GetProperty(realPropName).GetValue(resource);
            //}
            //catch (Exception)
            //{
            //    value = null;
            //}

            //XmlTreeStructure prop;
            //try
            //{
            //    if (value != null)
            //        prop = (XmlTreeStructure)XmlTreeStructure.Parse(value);
            //    else
            //        prop = new XmlTreeStructure(propertyName, mainNS);
            //}
            //catch (Exception)
            //{
            //    prop = null;
            //    throw new Exception("The Property Value Could Not Be Parsed");
            //}
            var property = resource.Properties.SingleOrDefault(p => p.Name == propertyName && p.Namespace == mainNs);
            IXMLTreeStructure prop;
            if (property != null)
                prop = property.Value == null
                    ? new XmlTreeStructure(propertyName, mainNs)
                    : XmlTreeStructure.Parse(property.Value);
            else
            {
                prop = new XmlTreeStructure(propertyName, mainNs);
            }
            //var prop = new XmlTreeStructure(propertyName, mainNS);
            //prop.Value = property?.Value;
            return (XmlTreeStructure)prop;
        }

        /// <summary>
        /// Contains all the properties that are common for all Calendar Collection Resources.
        /// </summary>
        /// <summary>
        /// Returns all the properties of a resource that must be returned for
        /// an "allprop" property method of Propfind. 
        /// </summary>
        /// <param name="calendarResource"></param>
        /// <param name="errorStack"></param>
        /// <returns></returns>
        public static List<XmlTreeStructure> GetAllVisibleProperties(this CalendarResource calendarResource, Stack<string> errorStack )
        {
            List<XmlTreeStructure> list = new List<XmlTreeStructure>();
            foreach (var property in calendarResource.Properties.Where(prop => prop.IsVisible))
            {
                var tempTree = property.Value == null ? new XmlTreeStructure(property.Name, property.Namespace) : XmlTreeStructure.Parse(property.Value);

                list.Add((XmlTreeStructure)tempTree);
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
            //foreach (var property in XmlGeneralProperties)
            //{
            //    list.Add(new XmlTreeStructure(property.Key, property.Value.Value));
            //}

            //TODO: annadir el ns a los de abajo
            //Display Name
            //var displayName = new XmlTreeStructure("displayname", DavNs);
            //list.Add(displayName);

            ////creation date
            //var creationDate = new XmlTreeStructure("creationdate", DavNs);
            //list.Add(creationDate);

            ////getcontent length
            //var getcontentlegth = new XmlTreeStructure("getcontentlenght", DavNs);
            //list.Add(getcontentlegth);

            ////getetag
            //var getEtag = new XmlTreeStructure("getetag", DavNs);
            //list.Add(getEtag);

            ////getLastModified
            //var getLastModified = new XmlTreeStructure("getlastmodified", DavNs);
            //list.Add(getLastModified);

            ////getContentLanguage
            //var getContentLanguage = new XmlTreeStructure("getcontentlanguage", DavNs);
            //list.Add(getContentLanguage);

            //supported lock
            return calendarResource.Properties.Select(property => (XmlTreeStructure)XmlTreeStructure.Parse(property.Value)).ToList();
        }
    }
}
