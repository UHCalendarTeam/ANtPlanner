using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataLayer.Models.ACL
{
    /// <summary>
    /// Defines new properties for the WebDav resources.
    /// All this properties applies on any given resource.
    /// The properties are stores in xml format, so need to be decoded
    /// and enceded for the working with them.
    /// </summary>
    public class AccessControlProperties
    {
        /// <summary>
        /// This property identifies a particular principal 
        /// as being the "owner" of the resource.
        /// </summary>
        public string Owner { get; set; }

        /// <summary>
        /// This property identifies a particular principal 
        /// as being the "group" of the resource.
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// This is a protected property that identifies 
        /// the privileges defined for the resource.
        /// Each privilege appears as an XML element, 
        /// where aggregate privileges list as sub-elements 
        /// all of the privileges that they aggregate.
        /// </summary>
        public string SupportedPrivilegeSet { get; set; }

        /// <summary>
        /// This is a protected property that specifies the list of access 
        /// control entries(ACEs), which define what principals are to get what 
        /// privileges for this resource.
        /// </summary>
        public string Acl { get; set; }
    }
}
