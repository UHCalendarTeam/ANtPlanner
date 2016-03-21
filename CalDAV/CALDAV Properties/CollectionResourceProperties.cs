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
        public static List<XMLTreeStructure> GetAllVisibleProperties(this CalendarResource calendarResource)
        {
            List<XMLTreeStructure> list = new List<XMLTreeStructure>();
            //getetag
            var etag = new XMLTreeStructure("getetag", new List<string>() {"D"});
            etag.AddValue(calendarResource.GetEtag);
            list.Add(etag);

            //displayname
            var displayName = new XMLTreeStructure("displayname", new List<string>() {"D"});
            displayName.AddValue(calendarResource.FileName.Replace(".ics", ""));
            list.Add(displayName);

            //TODO: creationdate
            var creationDate = new XMLTreeStructure("creationdate", new List<string>() {"D"});
            creationDate.AddValue(calendarResource.CreationDate.ToString());
            list.Add(creationDate);
            
            //TODO: getcontentlenght
            var getContentLenght = new XMLTreeStructure("getcontentlenght", new List<string>() {"D"});
            getContentLenght.AddValue(calendarResource.GetContentLength);
            list.Add(getContentLenght);

            //getcontenttype
            var getContentType = new XMLTreeStructure("getcontenttype", new List<string>() { "D" });
            getContentType.AddValue(calendarResource.GetContentType);
            list.Add(getContentType);

            //getlastmodified
            var getLastModified = new XMLTreeStructure("getlastmodified", new List<string>() {"D"});
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
        public static List<XMLTreeStructure> GetAllPropertyNames(this CalendarResource calendarResource)
        {
            var list = new List<XMLTreeStructure>();
            
            //Display Name
            var displayName = new XMLTreeStructure("displayname");
            list.Add(displayName);

            //resource type
            var resourceType = new XMLTreeStructure("resourcetype");
            list.Add(resourceType);
            
            //creation date
            var creationDate = new XMLTreeStructure("creationdate");
            list.Add(creationDate);

            //getcontent length
            var getcontentlegth = new XMLTreeStructure("getcontentlenght");
            list.Add(getcontentlegth);

            //getcontenttype
            var getContentType = new XMLTreeStructure("getcontenttype");
            list.Add(getContentType);

            //getetag
            var getEtag = new XMLTreeStructure("getetag");
            list.Add(getEtag);

            //getLastModified
            var getLastModified = new XMLTreeStructure("getlastmodified");
            list.Add(getLastModified);

            //supported lock

            return list;
        }
    }
}
