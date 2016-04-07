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
        public static string DavNs => "DAV:";


        /// <summary>
        /// Returns the value of a collection property given its name.
        /// If error returns the property without value and puts the error in the
        /// error stack.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="propertyName"></param>
        /// <param name="mainNs"></param>
        /// <param name="errorStack">Stores the stack of errors</param>
        /// <returns></returns>
        public static XmlTreeStructure ResolveProperty(this CalendarCollection collection, string propertyName, string mainNs, Stack<string> errorStack)
        {
            ////First I look to see if is one of the static ones.
            //if (XmlGeneralProperties.ContainsKey(propertyName))
            //{
            //    var svalue = XmlGeneralProperties[propertyName];
            //    var sprop = new XmlTreeStructure(propertyName, svalue.Value);
            //    sprop.AddValue(svalue.Key);
            //    return sprop;
            //}
            ////TODO: Check all property names
            //var realPropName = propertyName.ToLower();
            //string value;
            //realPropName = realPropName[0].ToString().ToUpper() + realPropName.Substring(1);
            //realPropName = realPropName.Replace("-", "");
            //try
            //{
            //    value = (string)collection.GetType().GetProperty(realPropName).GetValue(collection);
            //}
            //catch (Exception)
            //{
            //    value = null;
            //    //throw new Exception("The value could not be retrieved");
            //}
            //XmlTreeStructure prop;
            //try
            //{
            //    if (value != null)
            //        prop = (XmlTreeStructure)XmlTreeStructure.Parse(value);
            //    else
            //        prop = new XmlTreeStructure(propertyName, mainNS);
            //}
            //catch (Exception e)
            //{
            //    prop = null;
            //    //throw new Exception("The Property Value Could Not Be Parsed");
            //    throw e;
            //}
            var property = collection.Properties.SingleOrDefault(p => p.Name == propertyName && p.Namespace == mainNs);
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
        /// Returns all the properties of a collection that must be returned for
        /// an "allprop" property method of Propfind.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="errorStack"></param>
        /// <returns></returns>
        public static List<XmlTreeStructure> GetAllVisibleProperties(this CalendarCollection collection, Stack<string> errorStack)
        {
            var list = new List<XmlTreeStructure>();
            //foreach (var property in VisibleGeneralProperties)
            //{
            //    list.Add(ResolveProperty(collection, property));
            //}
            foreach (var property in collection.Properties.Where(prop => prop.IsVisible))
            {
                var tempTree = property.Value == null ? new XmlTreeStructure(property.Name, property.Namespace) : XmlTreeStructure.Parse(property.Value);

                list.Add((XmlTreeStructure)tempTree);
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
            //foreach (var property in XmlGeneralProperties)
            //{
            //    //I took the namespace from the last letter in general properties
            //    list.Add(new XmlTreeStructure(property.Key, property.Value.Value));
            //}

            ////calendar desription
            //var description = new XmlTreeStructure("calendar-description", CaldavNs);
            //list.Add(description);
            ////a todos los que estan abajo le tienes q pasar el MainNs
            ////Display Name
            //var displayName = new XmlTreeStructure("displayname", DavNs);
            //list.Add(displayName);

            ////CreationDate
            //var creationDate = new XmlTreeStructure("creationdate", DavNs);
            //list.Add(creationDate);

            ////resource type
            //var resourceType = new XmlTreeStructure("resourcetype", DavNs);
            //list.Add(resourceType);

            return collection.Properties.Select(property => new XmlTreeStructure(property.Name, property.Namespace)).ToList();
        }

        /// <summary>
        /// Try to remove the specified property in the collection.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="propertyName"></param>
        /// <param name="errorStack"></param>
        /// <returns></returns>
        public static bool RemoveProperty(this CalendarCollection collection, string propertyName, Stack<string> errorStack)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Try to create the specified property in the collection.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        /// <param name="errorStack"></param>
        /// <returns></returns>
        public static bool CreateProperty(this CalendarCollection collection, string propertyName, string propertyValue,Stack<string> errorStack)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Try to modify the specified property in the collection.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        /// <param name="errorStack"></param>
        /// <returns></returns>
        public static bool ModifyProperty(this CalendarCollection collection, string propertyName, string propertyValue, Stack<string> errorStack)
        {
            throw new NotImplementedException();
        }
    }
}
