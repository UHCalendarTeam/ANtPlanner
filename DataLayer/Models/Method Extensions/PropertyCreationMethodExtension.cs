using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using DataLayer.Models.Entities;

namespace DataLayer
{
    /// <summary>
    /// Contains useful methods for the creation 
    /// of different properties
    /// </summary>
    public static class PropertyCreation
    {

        private static readonly Dictionary<string, string> Namespaces = new Dictionary<string, string>
        {
            {"D", @"xmlns:D=""DAV:"""},
            {"C", @"xmlns:C=""urn:ietf:params:xml:ns:caldav"""}
        };

        private static readonly Dictionary<string, string> NamespacesSimple = new Dictionary<string, string>
        {
            {"D", "DAV:"},
            {"C", "urn:ietf:params:xml:ns:caldav"}
        };

        /// <summary>
        ///     Create and return a DAV:group-membership property.
        ///  This protected property identifies the groups in 
        ///     which the principal is directly a member.
        /// </summary>
        /// <param name="groupHref"></param>
        /// <returns></returns>
        public static Property CreateGroupMembership(string groupHref)
        {
            var hrefNode = groupHref == "" ? "" : $"<D:href>{groupHref}</D:href>";
            var property = new Property("group-membership", "DAV:")
            {
                Value = $"<D:group-membership xmlns:D=\"DAV:\">{hrefNode}</D:group-membership>"
            };
            return property;
        }

        /// <summary>
        ///     Create and return a  DAV:group-member-set property.
        ///     This property of a group principal identifies the principals that are
        ///     direct members of a group.
        /// </summary>
        /// <returns></returns>
        public static Property CreateGroupMemberSet()
        {
            var property = new Property("group-member-set", "DAV:")
            {
                Value = $"<D:group-member-set xmlns:D=\"DAV:\"></D:group-member-set>"
            };
            return property;
        }

        /// <summary>
        ///  Create the c:calendar-home-set property
        ///     THis property says where to find the principal's calendars.
        /// </summary>
        /// <param name="pType">Says if the principal is gonna represents
        /// a user or a group </param>
        /// <param name="principalId">If the principal represents a group then send
        /// the name of the group, otherwise send the email of the user</param>
        /// <returns></returns>
        public static Property CreateCalendarHomeSet(SystemProperties.PrincipalType pType, string principalId)
        {
            var property = new Property("calendar-home-set", "DAV:")
            {
                Value = $"<C:calendar-home-set xmlns:C=\"urn:ietf:params:xml:ns:caldav\" xmlns:D=\"DAV:\">" +
                        $"<d:href>{SystemProperties.BuildHomeSetUrl(pType, principalId)}</d:href></C:calendar-home-set>"
            };
            return property;
        }

        /// <summary>
        /// Create a property instance with the given values
        /// </summary>
        /// <param name="nodeName">The name for the node. ej:displayname</param>
        /// <param name="nodePrefix">The ns prefix of the node. ej: the prefix of DAV: is D, so pass D</param>
        /// <param name="nodeValue">The value for the node. Clean value, dont construct the xml.</param>
        /// <returns></returns>
        public static Property CreateProperty(string nodeName, string nodePrefix, string nodeValue)
        {
            var xmlValue = $"<{nodePrefix}:{nodeName} xmlns:d=\"DAV:\">{nodeValue}</{nodePrefix}:{nodeName}>";
            return new Property(nodeName, NamespacesSimple[nodePrefix])
            {
                Value = xmlValue
            };
        }

        /// <summary>
        /// Add a single node to this property
        /// </summary>
        /// <param name="property"></param>
        /// <param name="nodeValue"></param>
        /// <param name="nodeName"></param>
        /// <param name="ns"></param>
        /// <returns></returns>
        //public static bool AddNodeToProperty(this Property property, string nodeValue,string nodeName, string ns)
        //{
        //    var xml = XDocument.Parse(property.Value);
        //}
    }
}
