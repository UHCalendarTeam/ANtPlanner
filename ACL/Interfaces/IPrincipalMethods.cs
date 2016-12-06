using System.Collections.Generic;
using DataLayer.Models.Entities.ACL;

namespace ACL.Interfaces
{
    /// <summary>
    ///     Defines the behavior of the principals
    ///     for the system.
    ///     Implement this interface in order to have
    ///     full functionalities of the system.
    /// </summary>
    public interface IPrincipalMethods
    {
        /// <summary>
        ///     This should be a human readable name for the principal.
        ///     If none is available, return the nodename.
        /// </summary>
        /// <returns>Returns the displayname property for the principal</returns>
        string GetDispplayName(Principal principal);

        /// <summary>
        ///     This protected property, if non-empty, contains the URIs of network
        ///     resources with additional descriptive information about the
        ///     principal.
        /// </summary>
        /// <returns>The  DAV:alternate-URI-set property</returns>
        string GetAlternateUriSet(Principal principal);

        /// <summary>
        ///     This protected property contains the URL that MUST be used
        ///     to identify this principal in an ACL request.
        /// </summary>
        /// <returns>The principal-URL property</returns>
        string GetPrincipalURL(Principal principal);

        /// <summary>
        ///     This protected property contains the URL that MUST be used to identify
        ///     this principal in an ACL request.
        /// </summary>
        /// <returns>The group-member-set property.</returns>
        string GetGroupMemberSet(Principal principal);

        /// <summary>
        ///     This protected property identifies the groups in which the principal
        ///     is directly a member.
        /// </summary>
        /// <returns></returns>
        string GetGroupMembership(Principal principal);

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
        /// <param name="principal"></param>
        /// <param name="member">The member to be add.</param>
        /// <returns>True if successful operation, false otherwise.</returns>
        bool AddGroupMember(Principal principal, string member);
    }
}