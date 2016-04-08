using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CalDAV.XML_Processors;
using CalDAV.Models;
using CalDAV.Utils.XML_Processors;
using TreeForXml;
using System.Reflection;

namespace CalDAV.CALDAV_Properties
{
    public static class CollectionPropertyMethods
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
            return collection.Properties.Select(property => new XmlTreeStructure(property.Name, property.Namespace)).ToList();
        }

        /// <summary>
        /// Try to remove the specified property in the collection.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="propertyName"></param>
        /// <param name="nameSpace"></param>
        /// <param name="errorStack"></param>
        /// <returns></returns>
        public static bool RemoveProperty(this CalendarCollection collection, string propertyName, string nameSpace, Stack<string> errorStack)
        {
            var property = collection.Properties.SingleOrDefault(x => x.Name == propertyName && x.Namespace == nameSpace);
            if (property==null)
            {
                return true;
            }
            if(!property.IsDestroyable)
            {
                errorStack.Push(HttpStatusCode.Forbidden.ToString());
                return false;
            }
            collection.Properties.Remove(property);
            return true;
        }
       

        /// <summary>
        /// Try to modify the specified property if it exist in the collection.
        /// If the property does not exist it is try to create the property in the collection.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="propertyName"></param>
        /// <param name="nameSpace"></param>
        /// <param name="propertyValue"></param>
        /// <param name="errorStack"></param>
        /// <returns></returns>
        public static bool CreateOrModifyProperty(this CalendarCollection collection, string propertyName, string nameSpace, string propertyValue, Stack<string> errorStack)
        {
            //get the property
            var property =
                collection.Properties
                    .SingleOrDefault(prop => prop.Name == propertyName && prop.Namespace == nameSpace);
            //if the property did not exist it is created.
            if (property == null)
            {
                collection.Properties.Add(new CollectionProperty() {Name = propertyName, Namespace = nameSpace,
                    IsDestroyable = true, IsVisible = false, IsMutable = true, Value = XmlTreeStructure.Parse(propertyValue).ToString()});
                return true;
            }
            //if this property belongs to the fix system properties, it can not be changed.
            if (!property.IsMutable)
                return false;

            //if all previous conditions don't pass then the value of the property is changed.
            property.Value = XmlTreeStructure.Parse(propertyValue).ToString();
            return true;
        }
    }
}
