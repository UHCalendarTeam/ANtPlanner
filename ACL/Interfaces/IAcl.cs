namespace ACL.Interfaces
{
    /// <summary>
    ///     This specification defines a number of new properties for WebDAV
    ///     resources.Access control properties may be retrieved just like
    ///     other WebDAV properties, using the PROPFIND method.
    ///     Define the necessary properties for the ACL.
    /// </summary>
    public interface IAcl
    {
        /// <summary>
        ///     This property identifies a particular principal as being the "owner"
        ///     of the resource.
        /// </summary>
        /// <returns>
        ///     Should return the URL of the principal,
        ///     or string.Empty if there is no owner.
        /// </returns>
        string GetOwner();

        /// <summary>
        ///     This property identifies a particular principal as being the "group"
        ///     of the resource.
        /// </summary>
        /// <returns>
        ///     The URL of a principal that identify a group.
        ///     string.Empty if not exist.
        /// </returns>
        string GetGroup();

        /// <summary>
        ///     This is a protected property that identifies the privileges defined
        ///     for the resource.
        /// </summary>
        /// <returns>
        ///     The supported-privilege-set or string.Empty for
        ///     the default privilege.
        /// </returns>
        string GetSupportedPriviledSet();

        /// <summary>
        ///     DAV:current-user-privilege-set is a protected property containing the
        ///     exact set of privileges(as computed by the server) granted to the
        ///     currently authenticated HTTP user.
        /// </summary>
        /// <returns> The DAV:current-user-privilege-set property.</returns>
        string GetCurrentUserPrivilegeSet();

        /// <summary>
        ///     Specifies the list of access control entries(ACEs),
        ///     which define what principals are to get what
        ///     privileges for this resource.
        /// </summary>
        /// <returns>The  DAV:acl property.</returns>
        string GetACL();

        /// <summary>
        ///     Defines the types of ACLs supported by this
        ///     server, to avoid clients needlessly getting errors.
        /// </summary>
        /// <returns>The acl-restricion property.</returns>
        string GetAclRestriction();


        /// <summary>
        ///     Contains a set of URLs that identify the root
        ///     collections that contain the principals that are
        ///     available on the server that implements this resource.
        /// </summary>
        /// <returns>The principal-collection-set property.</returns>
        string GetPrincipalCollectionSet();
    }
}