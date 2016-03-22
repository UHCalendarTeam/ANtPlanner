using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalDAV.Models;
using CalDAV.Utils.XML_Processors;

namespace CalDAV.CALDAV_Properties
{
    public static class CollectionResourceProperties
    {
        public static string NameSpace => "urn:ietf:params:xml:ns:caldav";

        /// <summary>
        /// Returns all the properties of a resource that must be returned for
        /// an "allprop" property method of Propfind. 
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static List<XmlTreeStructure> GetAllVisibleProperties(this CalendarResource calendarResource)
        {
            List<XmlTreeStructure> list = new List<XmlTreeStructure>();
            //getetag
            var etag = new XmlTreeStructure("getetag", "D");
            etag.AddValue(calendarResource.GetEtag);
            list.Add(etag);

            //displayname
            var displayName = new XmlTreeStructure("displayname", "D");
            displayName.AddValue(calendarResource.FileName.Replace(".ics", ""));
            list.Add(displayName);

            //TODO: creationdate
            var creationDate = new XmlTreeStructure("creationdate", "D");
            creationDate.AddValue(calendarResource.CreationDate.ToString());
            list.Add(creationDate);
            
            //TODO: getcontentlenght
            var getContentLenght = new XmlTreeStructure("getcontentlenght", "D");
            getContentLenght.AddValue(calendarResource.GetContentLength);
            list.Add(getContentLenght);

            //getcontenttype
            var getContentType = new XmlTreeStructure("getcontenttype", "D");
            getContentType.AddValue(calendarResource.GetContentType);
            list.Add(getContentType);

            //getlastmodified
            var getLastModified = new XmlTreeStructure("getlastmodified", "D");
            getLastModified.AddValue(calendarResource.GetLastModified.ToString());
            list.Add(getLastModified);

            //resourcetype
            list.Add(calendarResource.ResourceType);

            //TODO: supported lock




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
