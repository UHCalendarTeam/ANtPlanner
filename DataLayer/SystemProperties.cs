using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataLayer
{
    /// <summary>
    /// Contains some useful properties for the system.
    /// Declare here the appSettings.
    /// </summary>
    public class SystemProperties
    {
        /// <summary>
        /// Contains the url for the user's collections
        /// Add the email of the user
        /// </summary>
        public static readonly string _userCollectionUrl = "collections/users/";

        /// <summary>
        /// Contains the url for the groups collection.
        /// Add the name of the group
        /// </summary>
        public static readonly string _groupCollectionUrl = "collections/group/";

        /// <summary>
        /// Contains the default name for the user collections
        /// </summary>
        public static readonly string _defualtInitialCollectionName = "DefualCalendar";

        public static readonly string _principalUrl = "principals";

        /// <summary>
        /// Contains the url that has to be assigned the the priciapl that 
        /// represent a normal user.
        /// The email of the user has to be added to the end of the url
        /// </summary>
        public static readonly string _userPrincipalUrl = "principals/users/"; 
        
        /// <summary>
        /// Contains the url that has to be assigned the the priciapl that 
        /// represent a group.
        /// The name of the group has to be added.
        /// </summary>
        public static readonly string _groupPrincipalUrl = "principals/groups/";
    }
}
