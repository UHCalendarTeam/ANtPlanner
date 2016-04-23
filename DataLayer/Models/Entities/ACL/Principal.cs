using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        ///     Main cont for this obj
        /// </summary>
        /// <param name="dispName">The DAV:displayname property value of the principal.</param>
        /// <param name="pIdentifier">If the principal represents a group then send
        /// the name of the group, otherwise send the email of the user</param>
        /// <param name="userOrGroup">If the principal represents a group or a user.</param>
        /// <param name="properties">The initial properties.</param>
        public Principal(string dispName, string pIdentifier, SystemProperties.PrinicpalType userOrGroup, 
            params Property[] properties)
        {
            Displayname = dispName;

            //build the principalUrl depending if the principal represents a user
            //or a group
            PrincipalURL = userOrGroup != SystemProperties.PrinicpalType.Group?
                SystemProperties._userPrincipalUrl+pIdentifier+"/":
                SystemProperties._groupPrincipalUrl+pIdentifier+"/";

            Properties = new List<Property>(properties);


            ///create some properties
            /// 
            ///if represents a group then add the DAV:group-membership property
            //if (userOrGroup == SystemProperties.PrinicpalType.User)
            //    Properties.Add(CreateGroupMemberSet());

            /////if the principal represents a user then add the DAV:group-membership property
            //else
            //{
            //    //if empty is not a student the user
            //    var url = groupName == "" ? "" : SystemProperties._groupPrincipalUrl + groupName;
            //    Properties.Add(CreateGroupMembership(url));
            //}
            ////if the principal represents a group take the name
            /////of the group, otherwise take the email of the 
            ///// user.
            //var pId = userOrGroup == SystemProperties.PrinicpalType.User?
            //    em

            /////create and add the calendar-home-set property
            //Properties.Add(CreateCalendarHomeSet(userOrGroup,));
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
        ///     Contains the collection of the principal.
        /// </summary>
        public ICollection<CalendarCollection> CalendarCollections { get; set; }

        /// <summary>
        ///     If the principal represents a user then
        ///     this is.
        /// </summary>
        public User User { get; set; }

        
    }
}