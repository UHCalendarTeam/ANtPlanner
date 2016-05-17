using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DataLayer.Models.Entities;
using DataLayer.Models.Entities.ResourcesAndCollections;

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
        ///     Call this constructor for the creation.
        /// </summary>
        /// <param name="pIdentifier">
        ///     If the principal represents a group then send
        ///     the name of the group, otherwise send the email of the user
        /// </param>
        /// <param name="userOrGroup">If the principal represents a group or a user.</param>
        /// <param name="properties">The initial properties.</param>
        public Principal(string pIdentifier, SystemProperties.PrincipalType userOrGroup,
            params Property[] properties)
        {
            //build the principalUrl depending if the principal represents a user
            //or a group
            PrincipalURL = userOrGroup != SystemProperties.PrincipalType.Group
                ? SystemProperties._userPrincipalUrl + pIdentifier + "/"
                : SystemProperties._groupPrincipalUrl + pIdentifier + "/";


            Properties = new List<Property>(properties);

            CalendarCollections = new List<CalendarCollection>();

            PrincipalStringIdentifier = pIdentifier;
        }

        /// <summary>
        ///     Identify uniquely a principal.
        ///     A principal may have many URLs, but there must be one "principal URL"
        ///     that clients can use to uniquely identify a principal.This protected
        ///     property contains the URL that MUST be used to identify this principal
        ///     in an ACL request.
        /// </summary>
        [Required]
        public string PrincipalURL { get; set; }


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
        /// The calendar home associated to the principal.
        /// </summary>
        public CalendarHome CalendarHome { get; set; }

        /// <summary>
        ///     If the principal represents a user then
        ///     this is.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        ///     When the principal represents an User this property
        ///     will contain the userEmail. When the principal represents a group
        ///     this property will contain the group name.
        /// </summary>
        public string PrincipalStringIdentifier { get; set; }

        /// <summary>
        ///     The value of the this property will be
        ///     use to store the value of the cookie's
        ///     session so to check if the principal is
        ///     authenticated.
        /// </summary>
        public string SessionId { get; set; }
    }
}