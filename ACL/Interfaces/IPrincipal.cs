using System.Collections.Generic;

namespace ACL.Interfaces
{
    /// <summary>
    ///     Defines the behavior of the principals
    ///     for the system.
    ///     Implement this interface in order to have
    ///     full functionalities of the system.
    /// </summary>
    public interface IPrincipal
    {
        /// <summary>
        ///     This should be a human readable name for the principal.
        ///     If none is available, return the nodename.
        /// </summary>
        /// <returns>Returns the displayname property for the principal</returns>
        string GetDispplayName();

        /// <summary>
        ///     This protected property, if non-empty, contains the URIs of network
        ///     resources with additional descriptive information about the
        ///     principal.
        /// </summary>
        /// <returns>The  DAV:alternate-URI-set property</returns>
        string GetAlternateUriSet();

        /// <summary>
        ///     This protected property contains the URL that MUST be used
        ///     to identify this principal in an ACL request.
        /// </summary>
        /// <returns>The principal-URL property</returns>
        string GetPrincipalURL();

        /// <summary>
        ///     This protected property contains the URL that MUST be used to identify
        ///     this principal in an ACL request.
        /// </summary>
        /// <returns>The group-member-set property.</returns>
        string GetGroupMemberSet();

        /// <summary>
        ///     This protected property identifies the groups in which the principal
        ///     is directly a member.
        /// </summary>
        /// <returns></returns>
        string GetGroupMembership();

        /// <summary>
        ///     Set the principals of this group.
        /// </summary>
        /// <param name="members">
        ///     The members that will belong t
        ///     to this group.
        /// </param>
        /// <returns>True is was a successful operation, false otherwise.</returns>
        bool SetGroupMemberSet(ICollection<string> members);

        /// <summary>
        ///     Add a member to the group.
        /// </summary>
        /// <param name="member">The member to be add.</param>
        /// <returns>True if successful operation, false otherwise.</returns>
        bool AddGroupMember(string member);
    }
}