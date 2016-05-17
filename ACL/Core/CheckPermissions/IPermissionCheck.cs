using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;

namespace ACL.Core.CheckPermissions
{
    /// <summary>
    /// This interface defines some useful methods
    /// that allow to check the permission that a 
    /// principal has to perform certain action on a
    /// resource.
    /// </summary>
    public interface IPermissionChecker
    {
        /// <summary>
        /// The HTTP methods GET, REPORT need to pass
        /// this permission in order to perform an action
        /// on a resource.
        /// </summary>
        /// <param name="resourceUrl">The URL that identifies the resource.</param>
        /// <param name="principalUrl">The URL that identifies the principal
        /// that is making the request.</param>
        /// <param name="response">The response for the client</param>
        /// <returns>True if the principal has the read permission on the resource
        /// False otherwise</returns>
        bool CheckReadPermission(string resourceUrl, string principalUrl, HttpResponse response);


        /// <summary>
        /// The HTTP methods PUT, DELETE need to pass
        /// this permission in order to perform an action
        /// on a resource.
        /// </summary>
        /// <param name="resourceUrl">The URL that identifies the resource.</param>
        /// <param name="principalUrl">The URL that identifies the principal
        /// that is making the request.</param>
        /// <param name="response">The response for the client</param>
        /// <returns>True if the principal has the write permission on the resource
        /// False otherwise</returns>
        bool CheckWritePermission(string resourceUrl, string principalUrl, HttpResponse response);


    }
}
