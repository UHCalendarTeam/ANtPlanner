using System.Collections.Generic;
using TreeForXml;

namespace CalDAV.Core.Propfind
{
    public interface IPropfindMethods
    {
        /// <summary>
        ///     Returns all dead properties and some live properties.
        /// </summary>
        /// <param name="principalUrl">User unique identification</param>
        /// <param name="collectionName">Target collection</param>
        /// <param name="calendarResourceId">Target resource</param>
        /// <param name="depth">Depth that method should hit</param>
        /// <param name="aditionalProperties">Properties contained in the include xml if any</param>
        /// <param name="multistatusTree">Response structure, element to be fill</param>
        /// <returns></returns>
        void AllPropMethod(string principalUrl, string collectionName, string calendarResourceId, int? depth,
            List<KeyValuePair<string, string>> aditionalProperties, XmlTreeStructure multistatusTree);

        /// <summary>
        ///     Returns the value of the specified properties.
        /// </summary>
        /// <param name="userEmail">User unique identification</param>
        /// <param name="collectionName">Target collection</param>
        /// <param name="calendarResourceId">Target resource</param>
        /// <param name="depth">Depth that method should hit</param>
        /// <param name="propertiesReq">List with all the properties to retrieve</param>
        /// <param name="multistatusTree">Response structure, element to be fill</param>
        /// <returns></returns>
        void PropMethod(string userEmail, string collectionName, string calendarResourceId, int? depth,
            List<KeyValuePair<string, string>> propertiesReq, XmlTreeStructure multistatusTree);

        /// <summary>
        ///     Returns the name of all the properties of a collection.
        /// </summary>
        /// <param name="userEmail">User unique identification</param>
        /// <param name="collectionName">Target collection</param>
        /// <param name="calendarResourceId">Target resource</param>
        /// <param name="depth">Depth that method should hit</param>
        /// <param name="multistatusTree">Response structure, element to be fill</param>
        /// <returns></returns>
        void PropNameMethod(string userEmail, string collectionName, string calendarResourceId, int? depth,
            XmlTreeStructure multistatusTree);
    }
}