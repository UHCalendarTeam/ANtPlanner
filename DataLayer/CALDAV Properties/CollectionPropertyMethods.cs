using System;
using System.Collections.Generic;
using System.Linq;
using DataLayer.Models.Entities;
using TreeForXml;

namespace DataLayer
{
    public static class CollectionPropertyMethods
    {
        public static string CaldavNs => "urn:ietf:params:xml:ns:caldav";
        public static string DavNs => "DAV:";

        //This two methods will give me a month up and down from NOW
        public static string MinDateTime(this CalendarCollection collection)
        {
            var thisMonth = DateTime.Now.Month;
            var thisDay = DateTime.Now.Day;
            return
                new DateTime(DateTime.Now.Year, thisMonth - 1 == 0 ? 12 : thisMonth - 1, thisDay > 28 ? 28 : thisDay)
                    .ToUniversalTime()
                    .ToString("yyyyMMddTHHmmssZ");
        }

        public static string MaxDateTime(this CalendarCollection collection)
        {
            var thisMonth = DateTime.Now.Month;
            var thisDay = DateTime.Now.Day;
            return
                new DateTime(DateTime.Now.Year, thisMonth + 1 == 13 ? 1 : thisMonth + 1, thisDay > 28 ? 28 : thisDay)
                    .ToUniversalTime()
                    .ToString("yyyyMMddTHHmmssZ");
        }

        /// <summary>
        ///     Returns the value of a collection property given its name.
        ///     If error returns the property without value and puts the error in the
        ///     error stack.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="propertyName">Name of the property</param>
        /// <param name="mainNs">Main Namespace</param>
        /// <param name="errorStack">Stores the stack of errors</param>
        /// <returns></returns>
        public static XmlTreeStructure ResolveProperty(this CalendarCollection collection, string propertyName,
            string mainNs, Stack<string> errorStack)
        {
            



            var property = string.IsNullOrEmpty(mainNs)?collection.Properties.FirstOrDefault(p => p.Name == propertyName): collection.Properties.FirstOrDefault(p => p.Name == propertyName && p.Namespace == mainNs);
            IXMLTreeStructure prop;
            if (property != null)
                prop = property.Value == null
                    ? new XmlTreeStructure(property.Name, property.Namespace) { Value = ""}
                    : XmlTreeStructure.Parse(property.Value);
            else
            {
                prop = new XmlTreeStructure(propertyName, mainNs);
            }

            return (XmlTreeStructure) prop;
        }

        /// <summary>
        ///     Returns all the properties of a collection that must be returned for
        ///     an "allprop" property method of Propfind.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="errorStack">Stores the stack of errors</param>
        /// <returns></returns>
        public static List<XmlTreeStructure> GetAllVisibleProperties(this CalendarCollection collection,
            Stack<string> errorStack)
        {
            var list = new List<XmlTreeStructure>();

            foreach (var property in collection.Properties.Where(prop => prop.IsVisible))
            {
                //TODO: Check that the property is accessible beyond its visibility.
                var tempTree = property.Value == null
                    ? new XmlTreeStructure(property.Name, property.Namespace) { Value = ""}
                    : XmlTreeStructure.Parse(property.Value);

                list.Add((XmlTreeStructure) tempTree);
            }

            return list;
        }

        /// <summary>
        ///     Returns the Name of all collection properties.
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static List<XmlTreeStructure> GetAllPropertyNames(this CalendarCollection collection)
        {
            return
                collection.Properties.Select(property => new XmlTreeStructure(property.Name, property.Namespace))
                    .ToList();
        }

        /// <summary>
        ///     Try to remove the specified property in the collection.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="propertyName"></param>
        /// <param name="nameSpace"></param>
        /// <param name="errorStack"></param>
        /// <returns></returns>
        public static bool RemoveProperty(this CalendarCollection collection, string propertyName, string nameSpace,
            Stack<string> errorStack)
        {
            var property = collection.Properties.SingleOrDefault(x => x.Name == propertyName && x.Namespace == nameSpace);
            if (property == null)
            {
                return true;
            }
            if (!property.IsDestroyable)
            {
                errorStack.Push("HTTP/1.1 403 Forbidden");
                return false;
            }
            collection.Properties.Remove(property);
            return true;
        }


        /// <summary>
        ///     Try to modify the specified property if it exist in the collection.
        ///     If the property does not exist it is try to create the property in the collection.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="propertyName"></param>
        /// <param name="nameSpace"></param>
        /// <param name="propertyValue"></param>
        /// <param name="errorStack"></param>
        /// <returns></returns>
        public static bool CreateOrModifyProperty(this CalendarCollection collection, string propertyName,
            string nameSpace, string propertyValue, Stack<string> errorStack)
        {
            //get the property
            var property =
                collection.Properties
                    .SingleOrDefault(prop => prop.Name == propertyName && prop.Namespace == nameSpace);
            //if the property did not exist it is created.
            if (property == null)
            {
                collection.Properties.Add(new Property
                {
                    Name = propertyName,
                    Namespace = nameSpace,
                    IsDestroyable = true,
                    IsVisible = false,
                    IsMutable = true,
                    Value = propertyValue
                });
                return true;
            }
            //if this property belongs to the fix system properties, it can not be changed.
            if (!property.IsMutable)
            {
                errorStack.Push("HTTP/1.1 403 Forbidden");
                return false;
            }


            //if all previous conditions don't pass then the value of the property is changed.
            property.Value = propertyValue;
            return true;
        }


        public static bool CreateOrModifyPropertyAdmin(this CalendarCollection collection, string propertyName,
            string nameSpace, string propertyValue, Stack<string> errorStack)
        {
            //get the property
            var property =
                collection.Properties
                    .SingleOrDefault(prop => prop.Name == propertyName && prop.Namespace == nameSpace);
            //if the property did not exist it is created.
            if (property == null)
            {
                collection.Properties.Add(new Property
                {
                    Name = propertyName,
                    Namespace = nameSpace,
                    IsDestroyable = true,
                    IsVisible = false,
                    IsMutable = true,
                    Value = propertyValue
                });
                return true;
            }
            //if this property belongs to the fix system properties, it can not be changed.
            //if (!property.IsMutable)
            //{
            //    errorStack.Push("HTTP/1.1 403 Forbidden");
            //    return false;
            //}


            //if all previous conditions don't pass then the value of the property is changed.
            property.Value = propertyValue;
            return true;
        }
    }
}