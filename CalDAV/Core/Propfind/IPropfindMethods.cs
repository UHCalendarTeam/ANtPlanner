using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalDAV.Utils.XML_Processors;
using CalDAV.XML_Processors;
using TreeForXml;


namespace CalDAV.Core.Propfind
{
    public interface IPropfindMethods
    {
        /// <summary>
        /// Returns all dead properties and some live properties.
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="collectionName"></param>
        /// <param name="calendarResourceId"></param>
        /// <param name="depth"></param>
        /// <param name="aditionalProperties"></param>
        /// <param name="multistatusTree"></param>
        /// <returns></returns>
        void AllPropMethod(string userEmail, string collectionName, string calendarResourceId, int? depth, List<KeyValuePair<string,string>> aditionalProperties ,XmlTreeStructure multistatusTree);

        /// <summary>
        /// Returns the value of the specified properties.
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="collectionName"></param>
        /// <param name="calendarResourceId"></param>
        /// <param name="depth"></param>
        /// <param name="propertiesReq"></param>
        /// <param name="multistatusTree"></param>
        /// <returns></returns>
        void PropMethod(string userEmail, string collectionName, string calendarResourceId, int? depth, List<KeyValuePair<string, string>> propertiesReq , XmlTreeStructure multistatusTree);

        /// <summary>
        ///  Returns the name of all the properties of a collection.
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="collectionName"></param>
        /// <param name="calendarResourceId"></param>
        /// <param name="depth"></param>
        /// <param name="multistatusTree"></param>
        /// <returns></returns>
        void PropNameMethod(string userEmail, string collectionName, string calendarResourceId, int? depth, XmlTreeStructure multistatusTree);
    }
}
