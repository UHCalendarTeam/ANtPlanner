using System.IO;
using System.Text;
using DataLayer;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Extensions;

namespace CalDAV.Core.Method_Extensions
{
    public static class MiscellaneousExtensions
    {
        public static void Write(this Stream stream, string content)
        {
            var data = Encoding.UTF8.GetBytes(content);
            stream.Write(data, 0, data.Length);
        }

        /// <summary>
        /// Remove the base url from the requested url. (i.e:http://...com/api/v1/caldav/)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetRealUrl(this HttpRequest request)
        {
            var url = request.GetEncodedUrl();
            var host = "http://" + request.Host.Value ;
            url = url.Replace(host, "");
            return url;
        }

        /// <summary>
        /// FOr the ACL PROFINDs the principalId is included
        /// in the url. Takes the principalId from it.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>Returns the PrincipalStringIdentifier. If the principal represents
        /// a user then returns the name of the groups, otherwise returns the email.</returns>
        public static string TakePrincipalIdFromUrl(this HttpContext context)
        {
            var url = context.Request.GetRealUrl();
            var index = url.LastIndexOf("/");

            var output = url.Substring(index + 1);
            return output;

        }
    }
}