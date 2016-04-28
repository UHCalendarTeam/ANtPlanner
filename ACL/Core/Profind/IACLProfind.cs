using DataLayer.Models.ACL;
using Microsoft.AspNet.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ACL.Core
{
    public interface IACLProfind
    {
        /// <summary>
        /// Defines the main methos for the ACL profind.
        /// </summary>
        /// <param name="httpContext">This contains besides the HttpRequest and the HttpResponse
        /// useful data in the Session.</param>
        /// <returns>Modified the HttpResponse and put out content there.</returns>
        Task Profind(HttpContext httpContext);

        /// <summary>
        /// Build the body of the response.
        /// </summary>
        /// <param name="response">The response for the client.</param>
        /// <param name="requestedUrl">The requested url that identified the principal's url </param>
        /// <param name="reqProperties">The requested properties from the client.</param>
        /// <param name="principal">An instance of the principal that the PROFIND should be 
        /// applied to.</param>
        /// <returns></returns>
        Task BuildResponse(HttpResponse response, string requestedUrl,
            List<KeyValuePair<string, string>> reqProperties, Principal principal);
    }
}