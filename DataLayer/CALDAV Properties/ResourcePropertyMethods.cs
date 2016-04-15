using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TreeForXml;

namespace DataLayer
{
    public static class ResourcePropertyMethods
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
            return calendarResource.Properties.Select(property => (XmlTreeStructure)XmlTreeStructure.Parse(property.Value)).ToList();
        }

        /// <summary>
        /// Try to remove the specified property in the resource.
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="propertyName"></param>
        /// <param name="propertyNamespace"></param>
        /// <param name="errorStack"></param>
        /// <returns></returns>
        public static bool RemoveProperty(this CalendarResource resource, string propertyName, string propertyNamespace, Stack<string> errorStack)
        {
            //try to gets the property for check if exists
            var property = resource.Properties.SingleOrDefault(x => x.Name == propertyName && x.Namespace == propertyNamespace);
            //if it does not exist then the method success!!
            if (property == null)
            {
                return true;
            }
            //If the property is not allowed to be destroyed it should return and error.
            if (!property.IsDestroyable)
            {
                errorStack.Push("HTTP/1.1 403 Forbidden");
                return false;
            }
            //If there are not problems and the property exists it should be deleted.
            resource.Properties.Remove(property);
            return true;
        }

        /// <summary>
        /// Try to modify the specified property if it exist in the resource.
        /// If the property does not exist it is try to create the property in the resource.
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="propertyName"></param>
        /// <param name="nameSpace"></param>
        /// <param name="propertyValue"></param>
        /// <param name="errorStack"></param>
        /// <returns></returns>
        public static bool CreateOrModifyProperty(this CalendarResource resource, string propertyName, string nameSpace, string propertyValue, Stack<string> errorStack)
        {
            //get the property
            var property =
                resource.Properties
                    .SingleOrDefault(prop => prop.Name == propertyName && prop.Namespace == nameSpace);
            //if the property did not exist it is created.
            if (property == null)
            {
                resource.Properties.Add(new ResourceProperty
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

    }
}
