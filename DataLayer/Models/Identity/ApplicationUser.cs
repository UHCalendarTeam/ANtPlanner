using DataLayer.Models.Entities.ACL;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace DataLayer.Models.Identity
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// Relationship one-to-one with the <see cref="Principal"/>
        /// that represent a "User" in the ACL protocol.
        /// </summary>
        public Principal Principal { get; set; }

        /// <summary>
        /// Inverse Navigation Key
        /// </summary>
        public string PrincipalId { get; set; }
    }
}
