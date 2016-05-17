using DataLayer;
using Microsoft.AspNet.Http;

namespace ACL.Core.CheckPermissions
{
    /// <summary>
    ///     This interface defines some useful methods
    ///     that allow to check the permission that a
    ///     principal has to perform certain action on a
    ///     resource.
    /// </summary>
    public interface IPermissionChecker
    {
        /// <summary>
        ///     The HTTP methods GET, REPORT need to pass
        ///     this permission in order to perform an action
        ///     on a resource.
        /// </summary>
        /// <param name="resourceUrl">The URL that identifies the resource.</param>
        /// <param name="principalUrl">
        ///     The URL that identifies the principal
        ///     that is making the request.
        /// </param>
        /// <param name="response">The response for the client</param>
        /// <returns>
        ///     True if the principal has the read permission on the resource
        ///     False otherwise
        /// </returns>
        bool CheckReadPermission(string resourceUrl, string principalUrl, HttpResponse response);


        /// <summary>
        ///     The HTTP methods PUT, DELETE need to pass
        ///     this permission in order to perform an action
        ///     on a resource.
        /// </summary>
        /// <param name="resourceUrl">The URL that identifies the resource.</param>
        /// <param name="principalUrl">
        ///     The URL that identifies the principal
        ///     that is making the request.
        /// </param>
        /// <param name="response">The response for the client</param>
        /// <returns>
        ///     True if the principal has the write permission on the resource
        ///     False otherwise
        /// </returns>
        bool CheckWritePermission(string resourceUrl, string principalUrl, HttpResponse response);

        /// <summary>
        ///     Check the permission needed to perform an action
        ///     by the given method.
        /// </summary>
        /// <param name="resourceUrl">The URL that identifies the resource.</param>
        /// <param name="principalUrl">
        ///     The URL that identifies the principal
        ///     that is making the request.
        /// </param>
        /// <param name="response">The response for the client</param>
        /// <param name="method">
        ///     The method is gonna be used to check what
        ///     permission has to the grant.
        /// </param>
        /// <returns>     
        ///         True if the principal has the permission granted on the resource
        ///         False otherwise
        ///</returns>
        bool CheckPermisionForMethod(string resourceUrl, string principalUrl, HttpResponse response,
            SystemProperties.HttpMethod method);
    }
}