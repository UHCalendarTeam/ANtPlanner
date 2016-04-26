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
            var host = "http://" + request.Host.Value + SystemProperties._baseUrl;
            url = url.Replace(host, "");
            return url;
        }
    }
}