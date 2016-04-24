using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
using Microsoft.Net.Http.Headers;
using ACL.Core.Authentication;


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
            //string body;
            //if (Request.Body != null)
            //      body = StreamToString(Request.Body);
            //string path = Request.PathBase.Value + Request.Path;
            //string mehod = Request.Method;
        }

        #region test action
        [HttpGet]
        public string test()
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
            Response.Cookies.Append("AuthId", Guid.NewGuid().ToString());
            return body;

        }
        #endregion

        #region

        [AcceptVerbs("PROPFIND", Route = "{user}")]
        public string PropFind(string groupOrUser)
        {
            return "test";
        }

        [AcceptVerbs("propfind")]
        public async Task PropFind()
        {
            string body = StreamToString(Request.Body);
            var auth = new UhCalendarAuthentication();
            await auth.AuthenticateRequest(Request, Response);
        }
        #endregion


        //#region Collection Methods

        ////MKCAL api\v1\caldav\{username}\calendars\{collection_name}\
        //[AcceptVerbs("MkCalendar", Route = "collections/{groupOrUser}/{principalId}/{collectionName}")]
        //public async Task MkCalendar(string groupOrUser, string principalId, string collectionName)
        //{
        //    var url = Request.GetEncodedUrl();

        //    var propertiesAndHeaders = new Dictionary<string, string>();
        //    propertiesAndHeaders.Add("principalUrl", GetPrincipalUrlFronUrl(url, collectionName));
        //    propertiesAndHeaders.Add("collectionName", collectionName);
        //    propertiesAndHeaders.Add("url", url);
        //    //TODO: I have to fix this the status is in the first element.
        //    //Response.StatusCode=GetHashCode() 

        //    await CalDavRepository.MkCalendar(propertiesAndHeaders, StreamToString(Request.Body), Response);
        //}

        ////PROPFIND COLLECTIONS
        //[AcceptVerbs("PropFind", Route = "collections/{groupOrUser}/{principalId}/{collectionName}")]
        //public void PropFind(string groupOrUser, string principalId, string collectionName)
        //{
        //    var url = Request.GetEncodedUrl();
        //    var propertiesAndHeaders = new Dictionary<string, string>();
        //    propertiesAndHeaders.Add("principalUrl", GetPrincipalUrlFronUrl(url, collectionName));
        //    propertiesAndHeaders.Add("collectionName", collectionName);
        //    propertiesAndHeaders.Add("url", url);

        //    StringValues depth;
        //    if (Request.Headers.TryGetValue("Depth", out depth))
        //        propertiesAndHeaders.Add("depth", depth);

        //    CalDavRepository.PropFind(propertiesAndHeaders, StreamToString(Request.Body), Response);
        //}

        ////PROPFIND RESOURCES
        //[AcceptVerbs("PropFind", Route = "{collections/{groupOrUser}/{principalId}/{collectionName}/{calendarResource}")]
        //public void PropFind(string groupOrUser, string principalId, string collectionName, string calendarResource)
        //{
        //    var url = Request.GetEncodedUrl();
        //    var propertiesAndHeaders = new Dictionary<string, string>();
        //    propertiesAndHeaders.Add("principalUrl", GetPrincipalUrlFronUrl(url, collectionName));
        //    propertiesAndHeaders.Add("collectionName", collectionName);
        //    propertiesAndHeaders.Add("url", url);

        //    propertiesAndHeaders.Add("calendarResourceId", calendarResource);

        //    StringValues depth;
        //    if (Request.Headers.TryGetValue("Depth", out depth))
        //        propertiesAndHeaders.Add("depth", depth);

        //    CalDavRepository.PropFind(propertiesAndHeaders, StreamToString(Request.Body), Response);
        //}

        //[AcceptVerbs("Proppatch", Route = "{collections/{groupOrUser}/{principalId}/{collectionName}/")]
        //public void PropPatch(string groupOrUser, string principalId, string collectionName)
        //{
        //    var url = Request.GetEncodedUrl();
        //    var propertiesAndHeaders = new Dictionary<string, string>();
        //    propertiesAndHeaders.Add("principalUrl", GetPrincipalUrlFronUrl(url, collectionName));
        //    propertiesAndHeaders.Add("collectionName", collectionName);
        //    propertiesAndHeaders.Add("url", url);

        //    CalDavRepository.PropPatch(propertiesAndHeaders, StreamToString(Request.Body), Response);
        //}

        //[AcceptVerbs("Proppatch", Route = "{collections/{groupOrUser}/{principalId}/{collectionName}/{calendarResourceId}")]
        //public void PropPatch(string groupOrUser, string principalId, string collectionName, string calendarResourceId)
        //{
        //    var url = Request.GetEncodedUrl();
        //    var propertiesAndHeaders = new Dictionary<string, string>();
        //    propertiesAndHeaders.Add("principalUrl", GetPrincipalUrlFronUrl(url, collectionName));
        //    propertiesAndHeaders.Add("collectionName", collectionName);
        //    propertiesAndHeaders.Add("url", url);
        //    propertiesAndHeaders.Add("calendarResourceId", calendarResourceId);

        //    CalDavRepository.PropPatch(propertiesAndHeaders, StreamToString(Request.Body), Response);
        //}

        ////REPORT
        //[AcceptVerbs("Report", Route = "{user}/calendars/{collection}")]
        //public string Report(string user, string collection)
        //{
        //    var propertiesAndHeaders = new Dictionary<string, string>();
        //    propertiesAndHeaders.Add("userEmail", user);
        //    propertiesAndHeaders.Add("collectionName", collection);

        //    return CalDavRepository.Report(propertiesAndHeaders, StreamToString(Request.Body));
        //}
        //#endregion


        //#region Calendar Object Resource Methods
        //// PUT api/caldav/user_name/calendars/collection_name/object_resource_file_name
        //[HttpPut("{collections/{groupOrUser}/{principalId}/{collectionName}/{calendarResourceId}")]
        //public async Task Put(string groupOrUser, string principalId, string collectionName, string calendarResourceId)
        //{
        //    var url = Request.GetEncodedUrl();
        //    var propertiesAndHeaders = new Dictionary<string, string>();
        //    propertiesAndHeaders.Add("principalUrl", GetPrincipalUrlFronUrl(url, collectionName));
        //    propertiesAndHeaders.Add("collectionName", collectionName);
        //    propertiesAndHeaders.Add("url", url);
        //    propertiesAndHeaders.Add("calendarResourceId", calendarResourceId);
        //    propertiesAndHeaders.Add("body", StreamToString(Request.Body));

        //    var headers = Request.GetTypedHeaders();



        //    if (!string.IsNullOrEmpty(headers.ContentType.MediaType) && headers.ContentType.MediaType != "text/calendar")
        //    {
        //        Response.StatusCode = (int)HttpStatusCode.ExpectationFailed;
        //    }
        //    else
        //    {
        //        if (headers.IfMatch.Count > 0)
        //        {
        //            propertiesAndHeaders.Add("If-Match", EtagAsString(headers.IfMatch));
        //        }
        //        else if (headers.IfNoneMatch.Count > 0)
        //        {
        //            propertiesAndHeaders.Add("If-None-Match", EtagAsString(headers.IfNoneMatch));
        //        }

        //        await CalDavRepository.AddCalendarObjectResource(propertiesAndHeaders, Response);
        //    }
        //}

        //private string EtagAsString(IList<EntityTagHeaderValue> etags)
        //{
        //    var res = "";
        //    foreach (var etag in etags)
        //    {
        //        res += etag.Tag + ",";
        //    }
        //    return res.Remove(res.Length - 2);
        //}



        //// GET api/caldav/user_name/calendars/collection_name/object_resource_file_name
        //[HttpGet("{collections/{groupOrUser}/{principalId}/{collectionName}/{calendarResourceId}")]
        //public async Task Get(string groupOrUser, string principalId, string collectionName, string calendarResourceId)
        //{
        //    var url = Request.GetEncodedUrl();
        //    var propertiesAndHeaders = new Dictionary<string, string>();
        //    propertiesAndHeaders.Add("principalUrl", GetPrincipalUrlFronUrl(url, collectionName));
        //    propertiesAndHeaders.Add("collectionName", collectionName);
        //    propertiesAndHeaders.Add("url", url); propertiesAndHeaders.Add("calendarResourceId", calendarResourceId);

        //    await CalDavRepository.ReadCalendarObjectResource(propertiesAndHeaders, Response);
        //}

        //// DELETE api/values/5
        //[HttpDelete("{collections/{groupOrUser}/{principalId}/{collectionName}/{calendarResourceId}")]
        //public void Delete(string groupOrUser, string principalId, string collectionName, string calendarResourceId)
        //{
        //    var url = Request.GetEncodedUrl();
        //    var propertiesAndHeaders = new Dictionary<string, string>();
        //    propertiesAndHeaders.Add("principalUrl", GetPrincipalUrlFronUrl(url, collectionName));
        //    propertiesAndHeaders.Add("collectionName", collectionName);
        //    propertiesAndHeaders.Add("url", url);
        //    propertiesAndHeaders.Add("calendarResourceId", calendarResourceId);

        //    CalDavRepository.DeleteCalendarObjectResource(propertiesAndHeaders, Response);
        //}

        //// DELETE api/values/
        //[HttpDelete("{collections/{groupOrUser}/{principalId}/{collectionName}")]
        //public void Delete(string groupOrUser, string principalId, string collectionName)
        //{
        //    var url = Request.GetEncodedUrl();
        //    var propertiesAndHeaders = new Dictionary<string, string>();
        //    propertiesAndHeaders.Add("principalUrl", GetPrincipalUrlFronUrl(url, collectionName));
        //    propertiesAndHeaders.Add("collectionName", collectionName);
        //    propertiesAndHeaders.Add("url", url);

        //    CalDavRepository.DeleteCalendarCollection(propertiesAndHeaders, Response);
        //}

        ////REPORT api/values/5
        //[AcceptVerbs("Report", Route = "{collections/{groupOrUser}/{principalId}/{collectionName}/{calendarResourceId}")]
        //public string Report(string groupOrUser, string principalId, string collectionName, string calendarResourceId)
        //{
        //    var url = Request.GetEncodedUrl();
        //    var propertiesAndHeaders = new Dictionary<string, string>();
        //    propertiesAndHeaders.Add("principalUrl", GetPrincipalUrlFronUrl(url, collectionName));
        //    propertiesAndHeaders.Add("collectionName", collectionName);
        //    propertiesAndHeaders.Add("url", url);
        //    propertiesAndHeaders.Add("calendarResourceId", calendarResourceId);

        //    return CalDavRepository.Report(propertiesAndHeaders, StreamToString(Request.Body));

        //}
        //#endregion

        private string StreamToString(Stream stream)
        {
            StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        //[AcceptVerbs("Initialize")]
        //public void InitialiseDb()
        //{
        //    Response.StatusCode = (int)HttpStatusCode.NoContent;
        //    var fs = new FileSystemManagement();
        //    SqlMock.RecreateDb();

        //    SqlMock.SeedDb_Fs();
        //}

        //[AcceptVerbs("Destroy")]
        //public void DestroyDb()
        //{
        //    Response.StatusCode = (int)HttpStatusCode.NoContent;
        //    SqlMock.RecreateDb();
        //}

        //private string GetPrincipalUrlFronUrl(string url, string collectionName)
        //{
        //    return url.Remove(url.IndexOf(collectionName));
        //}
    }
}
