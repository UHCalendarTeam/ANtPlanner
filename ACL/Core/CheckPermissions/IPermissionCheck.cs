using DataLayer;
using Microsoft.AspNetCore.Http;

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