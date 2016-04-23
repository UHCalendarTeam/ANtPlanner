using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DataLayer.Models.Entities;

namespace DataLayer.Models.ACL
{
    /// <summary>
    ///     Entity for the properties of the principals.
    ///     A principal may be a user or a group that contains many users.
    ///     All the groups and users contains a Principal because contains
    ///     useful properties for the ACL.
    /// </summary>
    public class Principal
    {
        public Principal()
        {
            
        }

        /// <summary>
        /// Main cont for this obj
        /// </summary>
        /// <param name="dispName">The DAV:displayname property value of the principal.</param>
        /// <param name="pUrl">The url that uniquely identifies this principal.</param>
        /// <param name="userOrGroup">True if the principal represent a user, false otherwise</param>
        /// <param name="groupName">If this principal represents a student
        /// then this param is the name of the group that the student belongs</param>
        /// <param name="properties">The initial properties.</param>
        public Principal(string dispName, string pUrl,bool userOrGroup,string groupName="", params Property[] properties)
        {
            Displayname = dispName;
            PrincipalURL = pUrl;
            Properties = new List<Property>(properties);


            //create some properties
            //if is a user then add the DAV:group-membership property
            if (userOrGroup)
                Properties.Add(CreateGroupMemberSet());
            else
            {
                //if empty is not a student the user
                var url = groupName==""?"": DataLayer.SystemProperties._groupPrincipalUrl + groupName;
                Properties.Add(CreateGroupMembership(url));
            }

        }

        /// <summary>
        ///     Identify uniquely a principal.
        ///     A principal may have many URLs, but there must be one "principal URL"
        ///     that clients can use to uniquely identify a principal.This protected
        ///     property contains the URL that MUST be used to identify this principal
        ///     in an ACL request.
        /// </summary>
        public string PrincipalURL { get; set; }

        /// <summary>
        ///     This property of a group principal identifies the principals that are
        ///     direct members of this group.
        /// </summary>
        public string GroupMemberSet { get; set; }

        /// <summary>
        ///     This protected property identifies the groups in which the principal
        ///     is directly a member.
        /// </summary>
        public string GroupMembership { get; set; }

        /// <summary>
        ///     The readable name of the principal.
        /// </summary>
        public string Displayname { get; set; }

        /// <summary>
        ///     Cotnains the email of the principal if the
        ///     principal is a user.
        /// </summary>
        public string Email { get; set; }

        [ScaffoldColumn(false)]
        public int PrincipalId { get; set; }

  
        /// <summary>
        ///     Contains the properties of
        ///     principal.
        /// </summary>
        public List<Property> Properties { get; set; }

        /// <summary>
        /// Contains the collection of the principal.
        /// </summary>
        public ICollection<CalendarCollection> CalendarCollections { get; set; }

        /// <summary>
        /// If the principal represents a user then 
        /// this is.
        /// </summary>
        public User User { get; set; }


        #region useful methods
        /// <summary>
        /// Create and return a DAV:group-membership property.
        /// </summary>
        /// <param name="groupHref"></param>
        /// <returns></returns>
        private static Property CreateGroupMembership(string groupHref)
        {
            var hrefNode = groupHref == "" ? "" : $"<D:href>{groupHref}</D:href>";
            var property = new Property("group-membership", "DAV:")
            {
                Value = $"<D:group-membership xmlns:D=\"DAV:\">{hrefNode}</D:group-membership>"
            };
            return property;
        }

        /// <summary>
        /// Create and return a  DAV:group-member-set property.
        /// </summary>
        /// <returns></returns>
        private static Property CreateGroupMemberSet()
        {
            var property = new Property("group-member-set", "DAV:")
            {
                Value = $"<D:group-member-set xmlns:D=\"DAV:\"></D:group-member-set>"
            };
            return property;
        }
        #endregion
    }
}