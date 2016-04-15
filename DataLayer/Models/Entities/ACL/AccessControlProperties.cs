using System.ComponentModel.DataAnnotations;

namespace DataLayer.Models.ACL
{
    /// <summary>
    ///     Defines new properties for the WebDav resources.
    ///     All this properties applies on any given resource.
    ///     The properties are stores in xml format, so need to be decoded
    ///     and enceded for the working with them.
    /// </summary>
    public class AccessControlProperties
    {
        /// <summary>
        ///     This property identifies a particular principal
        ///     as being the "owner" of the resource.
        /// </summary>
        public string Owner { get; set; }

        /// <summary>
        ///     This property identifies a particular principal
        ///     as being the "group" of the resource.
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        ///     This is a protected property that identifies
        ///     the privileges defined for the resource.
        ///     Each privilege appears as an XML element,
        ///     where aggregate privileges list as sub-elements
        ///     all of the privileges that they aggregate.
        /// </summary>
        public string SupportedPrivilegeSet { get; set; }

        /// <summary>
        ///     This is a protected property that specifies the list of access
        ///     control entries(ACEs), which define what principals are to get what
        ///     privileges for this resource.
        /// </summary>
        public string Acl { get; set; }

        /// <summary>
        ///     This protected property defines the types of ACLs supported by this
        ///     server, to avoid clients needlessly getting errors.When a client
        ///     tries to set an ACL via the ACL method, the server may reject the
        ///     attempt to set the ACL as specified
        /// </summary>
        public string AclRestrictions { get; set; }

        /// <summary>
        ///     This protected property of a resource contains a set of URLs that
        ///     identify the root collections that contain the principals that are
        ///     available on the server that implements this resource.
        ///     A server MAY elect to report none of the principal collections
        ///     it knows about, in which case the property value would be empty.
        /// </summary>
        public string PrincipalCollectionSet { get; set; }

        [ScaffoldColumn(false)]
        public int AccessControlPropertiesId { get; set; }
    }
}