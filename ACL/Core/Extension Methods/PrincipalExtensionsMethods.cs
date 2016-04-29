using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Models.ACL;
using DataLayer.Models.Entities;
using TreeForXml;

namespace ACL.Core.Extension_Method
{
    public static class PrincipalExtensionsMethods
    {
        /// <summary>
        /// Get the permission for the given principal
        /// in some resource.
        /// </summary>
        /// <param name="principal">The principal that wants to know his permissions.</param>
        /// <param name="property">The resource or collection's DAV:acl property</param>
        /// <returns>Return an I</returns>
        public static IXMLTreeStructure GetCurrentUserPermissions(this Principal principal, Property property)
        {
            return null;
        }


        
    }
}
