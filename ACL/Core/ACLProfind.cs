using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;

namespace ACL.Core
{
    /// <summary>
    /// Contains the PROFIND method for 
    /// the principals
    /// </summary>
    public class ACLProfind
    {
        /// <summary>
        /// Call the method to perform a PROFIND over a 
        /// principal.
        /// Initially the client could do a PROFIND over
        /// the server to discover all the user calendars
        /// or could PORFIND directly over a calendar URL. 
        /// </summary>
        /// <param name="headerDict">Pass through this the:
        /// authentnication, the url of the principal if any</param>
        /// <param name="body">The request's body</param>
        /// <param name="response">The HttpResponse property from the controller.</param>
        /// <returns>The request</returns>
        public async Task ACLProfind(Dictionary<string, string> headerDict, string body, HttpResponse response)
        {
            //take the authentication from the header if any
            string authentication = 
        }
    }
}
