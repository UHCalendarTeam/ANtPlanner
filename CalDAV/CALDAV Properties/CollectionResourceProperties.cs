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
            
            //TODO: getcontentlenght
            return list;
        }

        /// <summary>
        /// Returns all property names of the resource
        /// </summary>
        /// <param name="calendarResource"></param>
        /// <returns></returns>
        public static List<XMLTreeStructure> GetAllPropertyNames(this CalendarResource calendarResource)
        {
            throw new NotImplementedException();
        }
    }
}
