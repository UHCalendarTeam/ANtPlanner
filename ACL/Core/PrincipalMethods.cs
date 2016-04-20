using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ACL.Interfaces;
using DataLayer.Models.ACL;

namespace ACL.Core
{
    /// <summary>
    /// Implementation of the IPrincipalMethods.
    /// 
    /// </summary>
    public class PrincipalMethods : IPrincipalMethods
    {
        /// <summary>
        /// Return the DAV:displayname property of 
        /// the given principal
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        public  string GetDispplayName(Principal principal)
        {
            return principal.Displayname;
        }

        public string GetAlternateUriSet(Principal principal)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the DAV:principal-url property
        /// of the given principal.
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        public string GetPrincipalURL(Principal principal)
        {
            return principal.PrincipalURL;
        }

        public string GetGroupMemberSet(Principal principal)
        {
            throw new NotImplementedException();
        }

        public string GetGroupMembership(Principal principal)
        {
            throw new NotImplementedException();
        }

        public bool SetGroupMemberSet(ICollection<string> members)
        {
            throw new NotImplementedException();
        }

        public bool AddGroupMember(Principal principal, string member)
        {
            throw new NotImplementedException();
        }
    }
}
