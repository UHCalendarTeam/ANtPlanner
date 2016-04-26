using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CalDAV.Core;
using DataLayer;
using Microsoft.AspNet.Http.Extensions;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Routing;
using Microsoft.Extensions.Primitives;
using DataLayer;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Abstractions;
using Microsoft.AspNet.Identity;
using Microsoft.Data.Entity.Metadata.Internal;
using Microsoft.Net.Http.Headers;

namespace CalDav_Services.Controllers
{
    [Route("api/v1/[controller]")]
    public class CalDavController : Controller
    {
        //dependency injection
        [FromServices]
        ICalDav CalDavRepository { get; set; }

        CalDavContext _context;

        //Constructor
        public CalDavController(ICalDav repoCalDav, CalDavContext context)
        {
            CalDavRepository = repoCalDav;
            _context = context;
        }

        #region

        /// <summary>
        /// Accepts the PROFIND calls for the principals.
        /// </summary>
        /// <param name="groupOrUser">Says if the principal represents a user or a group</param>
        /// <param name="principalId">If the principal represents a group then it has the name
        /// of the group. Otherwise has the email of the user that the principal represents.</param>
        /// <returns></returns>
        [AcceptVerbs("PROPFIND", Route = "principals/{groupOrUser}/{principalId}")]
        public async Task ACLPropFind(string groupOrUser, string principalId)
        {
            
            var dict = new Dictionary<string, string>()
            {
                {"principalId", principalId },
                {"groupOrUser", groupOrUser }
            };
            Response.StatusCode = 207;
            await CalDavRepository.ACLProfind(Request, Response, dict);
           
            
        }

        /// <summary>
        /// If a PROFIND is perform to an empty URL
        /// means that the client is discovering the
        /// service.
        /// So create the principal and return a cookie
        /// to remember the user.
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs("PROPFIND", Route = "")]
        public async Task PropFind()
        {
            Response.StatusCode = 207;
            await CalDavRepository.ACLProfind(Request, Response, null);
           
            
        }

        #endregion


        #region Collection Methods
        /// <summary>
        /// THis action is called when the client perfoms a PROFIND
        /// on a collection to take the calendars.
        /// </summary>
        /// <param name="groupOrUser">If the principals represents a user or a group</param>
        /// <param name="principalId">If user then the email, otherwise the name of the group.</param>
        [AcceptVerbs("PropFind", Route = "collections/{groupOrUser}/{principalId}/")]
        public void CollectionRootProfind(string groupOrUser, string principalId)
        {
            var url = GetRealUrl(Request);
            var propertiesAndHeaders = new Dictionary<string, string>();
            propertiesAndHeaders.Add("url", url);
            var body = StreamToString(Request.Body);

            StringValues depth;
            if (Request.Headers.TryGetValue("Depth", out depth))
                propertiesAndHeaders.Add("depth", depth);

            CalDavRepository.PropFind(propertiesAndHeaders, body, Response);
        }

        //MKCAL api\v1\caldav\{username}\calendars\{collection_name}\
        [AcceptVerbs("MkCalendar", Route = "collections/{groupOrUser}/{principalId}/{collectionName}/")]
        public async Task MkCalendar(string groupOrUser, string principalId, string collectionName)
        {
            var url = GetRealUrl(Request);

            var propertiesAndHeaders = new Dictionary<string, string>();
            propertiesAndHeaders.Add("principalId", principalId);
            propertiesAndHeaders.Add("collectionName", collectionName);

            propertiesAndHeaders.Add("url", url);

            await CalDavRepository.MkCalendar(propertiesAndHeaders, StreamToString(Request.Body), Response);
        }

        //PROPFIND COLLECTIONS
        [AcceptVerbs("PropFind", Route = "collections/{groupOrUser}/{principalId}/{collectionName}/")]
        public void PropFind(string groupOrUser, string principalId, string collectionName)
        {
            var url = GetRealUrl(Request);
            var propertiesAndHeaders = new Dictionary<string, string>();
            propertiesAndHeaders.Add("url", url);

            StringValues depth;
            if (Request.Headers.TryGetValue("Depth", out depth))
                propertiesAndHeaders.Add("depth", depth);

            CalDavRepository.PropFind(propertiesAndHeaders, StreamToString(Request.Body), Response);
        }
        ///TODO: annadir un PROFIND q tenga la ruta "collections/{usersOrGroups}/principalId"
        /// por aki es donde el cliente realiza el primer PROFIND sobre una coleccion para coger los 
        /// calendarios del usuario.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupOrUser"></param>
        /// <param name="principalId"></param>
        /// <param name="collectionName"></param>
        /// <param name="calendarResource"></param>

        //PROPFIND RESOURCES
        [AcceptVerbs("PropFind", Route = "collections/{groupOrUser}/{principalId}/{collectionName}/{calendarResource}")]
        public void PropFind(string groupOrUser, string principalId, string collectionName, string calendarResource)
        {
            var url = GetRealUrl(Request);
            var propertiesAndHeaders = new Dictionary<string, string>();

            propertiesAndHeaders.Add("url", url);

            propertiesAndHeaders.Add("calendarResourceId", calendarResource);

            StringValues depth;
            if (Request.Headers.TryGetValue("Depth", out depth))
                propertiesAndHeaders.Add("depth", depth);

            CalDavRepository.PropFind(propertiesAndHeaders, StreamToString(Request.Body), Response);
        }

        [AcceptVerbs("Proppatch", Route = "collections/{groupOrUser}/{principalId}/{collectionName}/")]
        public void PropPatch(string groupOrUser, string principalId, string collectionName)
        {
            var url = GetRealUrl(Request);
            var propertiesAndHeaders = new Dictionary<string, string>();
            propertiesAndHeaders.Add("url", url);

            CalDavRepository.PropPatch(propertiesAndHeaders, StreamToString(Request.Body), Response);
        }

        [AcceptVerbs("Proppatch",
            Route = "collections/{groupOrUser}/{principalId}/{collectionName}/{calendarResourceId}")]
        public void PropPatch(string groupOrUser, string principalId, string collectionName, string calendarResourceId)
        {
            var url = GetRealUrl(Request);
            var propertiesAndHeaders = new Dictionary<string, string>();
            propertiesAndHeaders.Add("url", url);
            propertiesAndHeaders.Add("calendarResourceId", calendarResourceId);

            CalDavRepository.PropPatch(propertiesAndHeaders, StreamToString(Request.Body), Response);
        }

        //REPORT
        [AcceptVerbs("Report", Route = "collections/{groupOrUser}/{principalId}/{collectionName}/")]
        public async Task Report(string user, string collection)
        {
            var url = GetRealUrl(Request);
            var propertiesAndHeaders = new Dictionary<string, string>();
            propertiesAndHeaders.Add("userEmail", user);
            propertiesAndHeaders.Add("collectionName", collection);
            propertiesAndHeaders.Add("url", GetRealUrl(Request));
            await CalDavRepository.Report(HttpContext);
        }

        //OPTIONS
        [AcceptVerbs("Options", Route = "collections/{groupOrUser}/{principalId}/{collectionName}/")]
        public void Options(string groupOrUser, string principalId, string collectionName)
        {
            Response.Headers["Allow"] = "GET, PUT, PROPFIND";
        }

        #endregion


        #region Calendar Object Resource Methods

        // PUT api/caldav/user_name/calendars/collection_name/object_resource_file_name
        [HttpPut("collections/{groupOrUser}/{principalId}/{collectionName}/{calendarResourceId}")]
        public async Task Put(string groupOrUser, string principalId, string collectionName, string calendarResourceId)
        {
            var url = GetRealUrl(Request);
            var propertiesAndHeaders = new Dictionary<string, string>();
            propertiesAndHeaders.Add("principalId", principalId);
            propertiesAndHeaders.Add("url", url);
            propertiesAndHeaders.Add("calendarResourceId", calendarResourceId);
            propertiesAndHeaders.Add("body", StreamToString(Request.Body));

            var headers = Request.GetTypedHeaders();



            if (headers.ContentType != null && !string.IsNullOrEmpty(headers.ContentType.MediaType) &&
                headers.ContentType.MediaType != "text/calendar")
            {
                Response.StatusCode = (int)HttpStatusCode.ExpectationFailed;
            }
            else
            {
                var ifmatch = Request.Headers["If-Match"];
                if (ifmatch.Count > 0)
                {
                    propertiesAndHeaders.Add("If-Match", ifmatch.ToString());
                }

                var ifnonematch = Request.Headers["If-None-Match"];
                if (ifnonematch.Count > 0)
                {
                    propertiesAndHeaders.Add("If-None-Match", ifnonematch.ToString());
                }
                var contentSize = Request.Headers["Content-Length"];
                if (contentSize.Count > 0)
                {
                    propertiesAndHeaders.Add("content-length", contentSize);
                }

                await CalDavRepository.AddCalendarObjectResource(propertiesAndHeaders, Response);
            }


        }

        private string EtagAsString(IList<EntityTagHeaderValue> etags)
        {
            var res = "";
            foreach (var etag in etags)
            {
                res += etag.Tag + ",";
            }
            return res.Remove(res.Length - 1);
        }

        [HttpGet]
        public void test()
        {

            var body = @"BEGIN:VCALENDAR
VERSION:2.0
PRODID:-//Example Corp.//CalDAV Client//EN
BEGIN:VTODO
DTSTAMP:20060205T235300Z
DUE;TZID=US/Eastern:20060106T120000
LAST-MODIFIED:20060205T235308Z
SEQUENCE:1
STATUS:NEEDS-ACTION
SUMMARY:Task #2
UID:E10BA47467C5C69BB74E8720@example.com
BEGIN:VALARM
ACTION:AUDIO
TRIGGER;RELATED=START:-PT10M
END:VALARM
END:VTODO
END:VCALENDAR";
            //var arr = UTF8Encoding.UTF8.GetBytes(body.ToArray());
            //Response.Body.Write(arr,0,arr.Length);

            Response.StatusCode = 207;
            Response.Headers["test"] = "test";
            var headers = Response.GetTypedHeaders();
            headers.ContentLength = 300;
            Response.Headers["ContentLength"] = "300";

        }

        // GET api/caldav/user_name/calendars/collection_name/object_resource_file_name
        [HttpGet("collections/{groupOrUser}/{principalId}/{collectionName}/{calendarResourceId}")]
        public async Task Get(string groupOrUser, string principalId, string collectionName, string calendarResourceId)
        {
            var url = GetRealUrl(Request);
            var propertiesAndHeaders = new Dictionary<string, string>();
            propertiesAndHeaders.Add("url", url);

            await CalDavRepository.ReadCalendarObjectResource(propertiesAndHeaders, Response);
        }

        // DELETE api/values/5
        [HttpDelete("collections/{groupOrUser}/{principalId}/{collectionName}/{calendarResourceId}")]
        public void Delete(string groupOrUser, string principalId, string collectionName, string calendarResourceId)
        {
            var url = GetRealUrl(Request);
            var propertiesAndHeaders = new Dictionary<string, string>();
            propertiesAndHeaders.Add("principalId", principalId);
            propertiesAndHeaders.Add("url", url);

            CalDavRepository.DeleteCalendarObjectResource(propertiesAndHeaders, Response);
        }

        // DELETE api/values/
        [HttpDelete("collections/{groupOrUser}/{principalId}/{collectionName}/")]
        public void Delete(string groupOrUser, string principalId, string collectionName)
        {
            var url = GetRealUrl(Request);
            var propertiesAndHeaders = new Dictionary<string, string>();
            propertiesAndHeaders.Add("principalId", principalId);
            propertiesAndHeaders.Add("url", url);

            CalDavRepository.DeleteCalendarCollection(propertiesAndHeaders, Response);
        }

        //REPORT api/values/5
        [AcceptVerbs("Report", Route = "collections/{groupOrUser}/{principalId}/{collectionName}/{calendarResourceId}")]
        public async Task Report(string groupOrUser, string principalId, string collectionName, string calendarResourceId)
        {
            var url = GetRealUrl(Request);
            var propertiesAndHeaders = new Dictionary<string, string>();
            propertiesAndHeaders.Add("principalUrl", GetPrincipalUrlFromUrl(url, collectionName));
            propertiesAndHeaders.Add("collectionName", collectionName);
            propertiesAndHeaders.Add("url", url);
            propertiesAndHeaders.Add("calendarResourceId", calendarResourceId);

            CollectionReport collectionReport = new CollectionReport();
            await collectionReport.ProcessRequest(HttpContext);

        }

        [AcceptVerbs("Options", Route = "collections/{groupOrUser}/{principalId}/{collectionName}/{calendarResourceId}")]
        public void Options(string groupOrUser, string principalId, string collectionName, string calendarResourceId)
        {
            Response.Headers["Allow"] = "GET, PUT, PROPFIND";
        }

        #endregion

        private string StreamToString(Stream stream)
        {
            StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        [AcceptVerbs("Initialize")]
        public void InitialiseDb()
        {
            Response.StatusCode = (int)HttpStatusCode.NoContent;
            var fs = new FileSystemManagement();
            SqlMock.RecreateDb();

            SqlMock.SeedDb_Fs();
        }

        [AcceptVerbs("Destroy")]
        public void DestroyDb()
        {
            Response.StatusCode = (int)HttpStatusCode.NoContent;
            SqlMock.RecreateDb();
        }

        private string GetPrincipalUrlFromUrl(string url, string collectionName)
        {
            return url.Remove(url.IndexOf(collectionName));
        }

        private string GetRealUrl(HttpRequest request)
        {
            var url = Request.GetEncodedUrl();
            var host = "http://" + Request.Host.Value + "/api/v1/caldav/";
            url = url.Replace(host, "");
            return url;
        }
    }
}

