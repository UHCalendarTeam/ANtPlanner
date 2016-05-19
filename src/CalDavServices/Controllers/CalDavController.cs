using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using CalDAV.Core;
using DataLayer;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Extensions;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using ACL.Core.Authentication;

namespace CalDavServices.Controllers
{
    //[Route("api/v1/[controller]")]
    public class CalDavController : Controller
    {
        //Constructor
        public CalDavController(ICalDav repoCalDav, IAuthenticate authenticate)
        {
            CalDavRepository = repoCalDav;
            _authenticate = authenticate;
        }

        //dependency injection
        [FromServices]
        private ICalDav CalDavRepository { get; }

        private string StreamToString(Stream stream)
        {
            var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        [FromServices]
        private  IAuthenticate _authenticate { get; }

        private string GetPrincipalUrlFromUrl(string url, string collectionName)
        {
            return url.Remove(url.IndexOf(collectionName));
        }

        private string GetRealUrl(HttpRequest request)
        {
            var url = Request.GetEncodedUrl();
            var host = "http://" + Request.Host.Value + SystemProperties._baseUrl;
            url = url.Replace(host, "");
            return url;
        }

        #region

        /// <summary>
        ///     Accepts the PROFIND calls for the principals.
        /// </summary>
        /// <param name="groupOrUser">Says if the principal represents a user or a group</param>
        /// <param name="principalId">
        ///     If the principal represents a group then it has the name
        ///     of the group. Otherwise has the email of the user that the principal represents.
        /// </param>
        /// <returns></returns>
        [AcceptVerbs("PROPFIND", Route = "principals/{groupOrUser}/{principalId}")]
        public async Task ACLPropFind(string groupOrUser, string principalId)
        {
            Response.ContentType = @"application/xml; charset=""utf-8""";

            var dict = new Dictionary<string, string>
            {
                {"principalId", principalId},
                {"groupOrUser", groupOrUser}
            };
            Response.StatusCode = 207;
            //HttpContext.Session.SetString("principalId", principalId);
            //HttpContext.Session.SetString("groupOrUser", groupOrUser);
            await CalDavRepository.ACLProfind(HttpContext);
        }

        /// <summary>
        ///     If a PROFIND is perform to an empty URL
        ///     means that the client is discovering the
        ///     service.
        ///     So create the principal and return a cookie
        ///     to remember the user.
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs("PROPFIND", Route = "/")]
        public async Task PropFind()
        {
            Response.ContentType = @"application/xml; charset=""utf-8""";
            Response.StatusCode = 207;

            await CalDavRepository.ACLProfind(HttpContext);
        }

        #endregion

        #region Collection Methods

        /// <summary>
        ///     THis action is called when the client perfoms a PROFIND
        ///     on a collection to take the calendars.
        /// </summary>
        /// <param name="groupOrUser">If the principals represents a user or a group</param>
        /// <param name="principalId">If user then the email, otherwise the name of the group.</param>
        [AcceptVerbs("PropFind", Route = "collections/{groupOrUser}/{principalId}/")]
        public async Task CollectionRootProfind(string groupOrUser, string principalId)
        {
            Response.ContentType = @"application/xml; charset=""utf-8""";
            var url = GetRealUrl(Request);
            var principal = await _authenticate.AuthenticateRequestAsync(HttpContext);
            var propertiesAndHeaders = new Dictionary<string, string>();
            propertiesAndHeaders.Add("principalUrl", principal.PrincipalURL);
            propertiesAndHeaders.Add("url", url);
            var body = StreamToString(Request.Body);

            StringValues depth;
            if (Request.Headers.TryGetValue("Depth", out depth))
                propertiesAndHeaders.Add("depth", depth);

            await CalDavRepository.CHSetPropfind(propertiesAndHeaders, body, Response);
        }

        //MKCAL api\v1\caldav\{username}\calendars\{collection_name}\
        [AcceptVerbs("MkCalendar", Route = "collections/{groupOrUser}/{calendarHome}/{collectionName}/")]
        public async Task MkCalendar(string groupOrUser, string calendarHome, string collectionName)
        {
            var url = GetRealUrl(Request);
            var principal = await _authenticate.AuthenticateRequestAsync(HttpContext);
            var propertiesAndHeaders = new Dictionary<string, string>();
            propertiesAndHeaders.Add("principalUrl", principal.PrincipalURL);
            propertiesAndHeaders.Add("collectionName", collectionName);
            propertiesAndHeaders.Add("url", url);

            await CalDavRepository.MkCalendar(propertiesAndHeaders, StreamToString(Request.Body), Response);
        }

        //PROPFIND COLLECTIONS
        [AcceptVerbs("PropFind", Route = "collections/{groupOrUser}/{principalId}/{collectionName}/")]
        public async Task PropFind(string groupOrUser, string principalId, string collectionName)
        {
            var principal = await _authenticate.AuthenticateRequestAsync(HttpContext);
            Response.ContentType = @"application/xml; charset=""utf-8""";
            var url = GetRealUrl(Request);
            var propertiesAndHeaders = new Dictionary<string, string>();
            propertiesAndHeaders.Add("url", url);
            propertiesAndHeaders.Add("principalUrl", principal.PrincipalURL); 

            StringValues depth;
            if (Request.Headers.TryGetValue("Depth", out depth))
                propertiesAndHeaders.Add("depth", depth);

            await CalDavRepository.PropFind(propertiesAndHeaders, StreamToString(Request.Body), Response);
        }

        /// TODO: annadir un PROFIND q tenga la ruta "collections/{usersOrGroups}/principalId"
        /// por aki es donde el cliente realiza el primer PROFIND sobre una coleccion para coger los 
        /// calendarios del usuario.
        /// <summary>
        /// </summary>
        /// <param name="groupOrUser"></param>
        /// <param name="principalId"></param>
        /// <param name="collectionName"></param>
        /// <param name="calendarResource"></param>

        //PROPFIND RESOURCES
        [AcceptVerbs("PropFind", Route = "collections/{groupOrUser}/{principalId}/{collectionName}/{calendarResource}")]
        public async Task PropFind(string groupOrUser, string principalId, string collectionName,
            string calendarResource)
        {
            var principal = await _authenticate.AuthenticateRequestAsync(HttpContext);
            Response.ContentType = @"application/xml; charset=""utf-8""";
            var url = GetRealUrl(Request);
            var propertiesAndHeaders = new Dictionary<string, string>();

            propertiesAndHeaders.Add("url", url);

            propertiesAndHeaders.Add("calendarResourceId", calendarResource);

            propertiesAndHeaders["principalUrl"] = principal.PrincipalURL;

            StringValues depth;
            if (Request.Headers.TryGetValue("Depth", out depth))
                propertiesAndHeaders.Add("depth", depth);

            await CalDavRepository.PropFind(propertiesAndHeaders, StreamToString(Request.Body), Response);
        }

        [AcceptVerbs("Proppatch", Route = "collections/{groupOrUser}/{principalId}/{collectionName}/")]
        public async Task PropPatch(string groupOrUser, string principalId, string collectionName)
        {
            Response.ContentType = @"application/xml; charset=""utf-8""";
            var url = GetRealUrl(Request);
            var propertiesAndHeaders = new Dictionary<string, string>();
            propertiesAndHeaders.Add("url", url);
            var principal = await _authenticate.AuthenticateRequestAsync(HttpContext);
            propertiesAndHeaders["principalUrl"] = principal.PrincipalURL;

            await CalDavRepository.PropPatch(propertiesAndHeaders, StreamToString(Request.Body), Response);
        }

        [AcceptVerbs("Proppatch",
            Route = "collections/{groupOrUser}/{principalId}/{collectionName}/{calendarResourceId}")]
        public async Task PropPatch(string groupOrUser, string principalId, string collectionName,
            string calendarResourceId)
        {
            Response.ContentType = @"application/xml; charset=""utf-8""";
            var url = GetRealUrl(Request);
            var propertiesAndHeaders = new Dictionary<string, string>();
            propertiesAndHeaders.Add("url", url);
            propertiesAndHeaders.Add("calendarResourceId", calendarResourceId);
            var principal = await _authenticate.AuthenticateRequestAsync(HttpContext);
            propertiesAndHeaders["principalUrl"] = principal.PrincipalURL;

            await CalDavRepository.PropPatch(propertiesAndHeaders, StreamToString(Request.Body), Response);
        }

        [AcceptVerbs("Options", Route = "/")]
        public void Options()
        {
            Response.Headers["Allow"] = "GET, PUT, PROPFIND, REPORT, PROPATCH";
        }

        //OPTIONS
        [AcceptVerbs("Options", Route = "collections/{groupOrUser}/{principalId}/{collectionName}/")]
        public void Options(string groupOrUser, string principalId, string collectionName)
        {
            Response.Headers["Allow"] = "GET, PUT, PROPFIND, REPORT, PROPATCH";
        }

        #endregion

        #region Calendar Object Resource Methods

        // PUT api/caldav/user_name/calendars/collection_name/object_resource_file_name
        [HttpPut("collections/{groupOrUser}/{principalId}/{collectionName}/{calendarResourceId}")]
        public async Task Put(string groupOrUser, string principalId, string collectionName, string calendarResourceId)
        {
            var url = GetRealUrl(Request);
            var principal = await _authenticate.AuthenticateRequestAsync(HttpContext);
            var propertiesAndHeaders = new Dictionary<string, string>
            {
                {"principalUrl", principal.PrincipalURL},
                {"url", url},
                {"calendarResourceId", calendarResourceId},
                {"body", StreamToString(Request.Body)}
            };

            var headers = Request.GetTypedHeaders();


            if (headers.ContentType != null && !string.IsNullOrEmpty(headers.ContentType.MediaType) &&
                headers.ContentType.MediaType != "text/calendar")
            {
                Response.StatusCode = (int) HttpStatusCode.ExpectationFailed;
            }
            else
            {
                var ifmatch = Request.Headers["If-Match"];
                foreach (var value in ifmatch)
                {
                    propertiesAndHeaders.Add("If-Match", value);
                }

                var ifnonematch = Request.Headers["If-None-Match"];
                foreach (var value in ifnonematch)
                {
                    propertiesAndHeaders.Add("If-None-Match", value);
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


        // GET api/caldav/user_name/calendars/collection_name/object_resource_file_name
        [HttpGet("collections/{groupOrUser}/{principalId}/{collectionName}/{calendarResourceId}")]
        public async Task Get(string groupOrUser, string principalId, string collectionName, string calendarResourceId)
        {
            var url = GetRealUrl(Request);
            var propertiesAndHeaders = new Dictionary<string, string>();
            propertiesAndHeaders.Add("url", url);
            var principal = await _authenticate.AuthenticateRequestAsync(HttpContext);
            propertiesAndHeaders.Add("principalUrl", principal.PrincipalURL);

            await CalDavRepository.ReadCalendarObjectResource(propertiesAndHeaders, Response);
        }

        // DELETE api/values/5
        [HttpDelete("collections/{groupOrUser}/{principalId}/{collectionName}/{calendarResourceId}")]
        public async Task Delete(string groupOrUser, string principalId, string collectionName,
            string calendarResourceId)
        {
            var url = GetRealUrl(Request);
            var propertiesAndHeaders = new Dictionary<string, string>();
            var principal = await _authenticate.AuthenticateRequestAsync(HttpContext);

            propertiesAndHeaders.Add("principalUrl", principal.PrincipalURL);
            propertiesAndHeaders.Add("url", url);

            var ifmatch = Request.Headers["If-Match"];
            if (ifmatch.Count > 0)
            {
                propertiesAndHeaders.Add("If-Match", ifmatch.ToString());
            }

            await CalDavRepository.DeleteCalendarObjectResource(propertiesAndHeaders, Response);
        }

        // DELETE api/values/
        [HttpDelete("collections/{groupOrUser}/{principalId}/{collectionName}/")]
        public async Task Delete(string groupOrUser, string principalId, string collectionName)
        {
            var url = GetRealUrl(Request);
            var principal = await _authenticate.AuthenticateRequestAsync(HttpContext);
            var propertiesAndHeaders = new Dictionary<string, string>();
            propertiesAndHeaders.Add("principalUrl", principal.PrincipalURL);
            propertiesAndHeaders.Add("url", url);

            await CalDavRepository.DeleteCalendarCollection(propertiesAndHeaders, Response);
        }

        #region Collections Reports

        //REPORT api/values/5
        [AcceptVerbs("Report", Route = "collections/{groupOrUser}/{principalId}/{collectionName}/{calendarResourceId}")]
        public async Task Report(string groupOrUser, string principalId, string collectionName,
            string calendarResourceId)
        {
            Response.StatusCode = 207;
            Response.ContentType = @"application/xml; charset=""utf-8""";
            await CalDavRepository.Report(HttpContext);
        }

        //REPORT
        [AcceptVerbs("Report", Route = "collections/{groupOrUser}/{principalId}/{collectionName}/")]
        public async Task Report(string user, string collection)
        {
            Response.StatusCode = 207;
            Response.ContentType = @"application/xml; charset=""utf-8""";
            await CalDavRepository.Report(HttpContext);
        }

        #endregion

        [AcceptVerbs("Options", Route = "collections/{groupOrUser}/{principalId}/{collectionName}/{calendarResourceId}")
        ]
        public void Options(string groupOrUser, string principalId, string collectionName, string calendarResourceId)
        {
            Response.Headers["Allow"] = "GET, PUT, PROPFIND";
        }

        #endregion
    }
}