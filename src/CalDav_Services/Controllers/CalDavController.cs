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

        [AcceptVerbs("PROPFIND", Route = "{user}")]
        public string PropFind(string user)
        {
            return "test";
        }

        [AcceptVerbs("propfind")]
        public async Task PropFind()
        {

            await Response.WriteAsync("sdf");
            // return "test";
        }
        #endregion


        #region Collection Methods

        //MKCAL api\v1\caldav\{username}\calendars\{collection_name}\
        [AcceptVerbs("MkCalendar", Route = "{user}/calendars/{collection}/")]
        public async Task MkCalendar(string user, string collection)
        {
            var propertiesAndHeaders = new Dictionary<string, string>();
            propertiesAndHeaders.Add("userEmail", user);
            propertiesAndHeaders.Add("collectionName", collection);
            propertiesAndHeaders.Add("url", Request.GetEncodedUrl());
            //TODO: I have to fix this the status is in the first element.
            //Response.StatusCode=GetHashCode() 

            await CalDavRepository.MkCalendar(propertiesAndHeaders, StreamToString(Request.Body), Response);
        }

        //PROPFIND COLLECTIONS
        [AcceptVerbs("PropFind", Route = "{user}/calendars/{collection}/")]
        public void PropFind(string user, string collection)
        {
            var propertiesAndHeaders = new Dictionary<string, string>();
            propertiesAndHeaders.Add("userEmail", user);
            propertiesAndHeaders.Add("collectionName", collection);

            StringValues depth;
            if (Request.Headers.TryGetValue("Depth", out depth))
                propertiesAndHeaders.Add("depth", depth);

            CalDavRepository.PropFind(propertiesAndHeaders, StreamToString(Request.Body), Response);
        }

        //PROPFIND RESOURCES
        [AcceptVerbs("PropFind", Route = "{user}/calendars/{collection}/{calendarResource}")]
        public void PropFind(string user, string collection, string calendarResource)
        {
            var propertiesAndHeaders = new Dictionary<string, string>();
            propertiesAndHeaders.Add("userEmail", user);
            propertiesAndHeaders.Add("collectionName", collection);
            propertiesAndHeaders.Add("calendarResourceId", calendarResource);
            
            StringValues depth;
            if (Request.Headers.TryGetValue("Depth", out depth))
                propertiesAndHeaders.Add("depth", depth);

            CalDavRepository.PropFind(propertiesAndHeaders, StreamToString(Request.Body), Response);
        }

        [AcceptVerbs("Proppatch", Route = "{userEmail}/calendars/{collectionName}/")]
        public void PropPatch(string userEmail, string collectionName)
        {
            var propertiesAndHeaders = new Dictionary<string, string>();
            propertiesAndHeaders.Add("userEmail", userEmail);
            propertiesAndHeaders.Add("collectionName", collectionName);

            CalDavRepository.PropPatch(propertiesAndHeaders, StreamToString(Request.Body), Response);
        }

        [AcceptVerbs("Proppatch", Route = "{userEmail}/calendars/{collectionName}/{calendarResourceId}")]
        public void PropPatch(string userEmail, string collectionName, string calendarResourceId)
        {
            var propertiesAndHeaders = new Dictionary<string, string>();
            propertiesAndHeaders.Add("userEmail", userEmail);
            propertiesAndHeaders.Add("collectionName", collectionName);
            propertiesAndHeaders.Add("calendarResourceId", calendarResourceId);

            CalDavRepository.PropPatch(propertiesAndHeaders, StreamToString(Request.Body), Response);
        }

        //REPORT
        [AcceptVerbs("Report", Route = "{user}/calendars/{collection}")]
        public string Report(string user, string collection)
        {
            var propertiesAndHeaders = new Dictionary<string, string>();
            propertiesAndHeaders.Add("userEmail", user);
            propertiesAndHeaders.Add("collectionName", collection);

            return CalDavRepository.Report(propertiesAndHeaders, StreamToString(Request.Body));
        }
        #endregion


        #region Calendar Object Resource Methods
        // PUT api/caldav/user_name/calendars/collection_name/object_resource_file_name
        [HttpPut("{user}/calendars/{collection}/{calendarResourceId}")]
        public async Task Put(string user, string collection, string calendarResourceId)
        {
            var propertiesAndHeaders = new Dictionary<string, string>();
            propertiesAndHeaders.Add("userEmail", user);
            propertiesAndHeaders.Add("collectionName", collection);
            propertiesAndHeaders.Add("calendarResourceId", calendarResourceId);
            propertiesAndHeaders.Add("body", StreamToString(Request.Body));
            propertiesAndHeaders.Add("url", Request.GetEncodedUrl());

            var headers = Request.GetTypedHeaders();



            if (!string.IsNullOrEmpty(headers.ContentType.MediaType) && headers.ContentType.MediaType != "text/calendar")
            {
                Response.StatusCode = (int)HttpStatusCode.ExpectationFailed;
            }
            else
            {
                if (headers.IfMatch.Count > 0)
                {
                    propertiesAndHeaders.Add("If-Match", EtagAsString(headers.IfMatch));
                }
                else if (headers.IfNoneMatch.Count > 0)
                {
                    propertiesAndHeaders.Add("If-None-Match", EtagAsString(headers.IfNoneMatch));
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
            return res.Remove(res.Length - 2);
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
        [HttpGet("{user}/calendars/{collection}/{calendarResourceId}")]
        public async Task Get(string user, string collection, string calendarResourceId)
        {
            var propertiesAndHeaders = new Dictionary<string, string>();
            propertiesAndHeaders.Add("userEmail", user);
            propertiesAndHeaders.Add("collectionName", collection);
            propertiesAndHeaders.Add("calendarResourceId", calendarResourceId);

            await CalDavRepository.ReadCalendarObjectResource(propertiesAndHeaders, Response);
        }

        // DELETE api/values/5
        [HttpDelete("{user}/calendars/{collection}/{calendarResourceId}")]
        public void Delete(string user, string collection, string calendarResourceId)
        {
            var propertiesAndHeaders = new Dictionary<string, string>();
            propertiesAndHeaders.Add("userEmail", user);
            propertiesAndHeaders.Add("collectionName", collection);
            propertiesAndHeaders.Add("calendarResourceId", calendarResourceId);

            CalDavRepository.DeleteCalendarObjectResource(propertiesAndHeaders, Response);
        }

        // DELETE api/values/
        [HttpDelete("{user}/calendars/{collection}")]
        public void Delete(string user, string collection)
        {
            var propertiesAndHeaders = new Dictionary<string, string>();
            propertiesAndHeaders.Add("userEmail", user);
            propertiesAndHeaders.Add("collectionName", collection);

            CalDavRepository.DeleteCalendarCollection(propertiesAndHeaders, Response);
        }

        //REPORT api/values/5
        [AcceptVerbs("Report", Route = "{user}/calendars/{collection}/{calendarResourceId}")]
        public string Report(string user, string collection, string calendarResourceId)
        {
            var propertiesAndHeaders = new Dictionary<string, string>();
            propertiesAndHeaders.Add("userEmail", user);
            propertiesAndHeaders.Add("collectionName", collection);
            propertiesAndHeaders.Add("calendarResourceId", calendarResourceId);

            return CalDavRepository.Report(propertiesAndHeaders, StreamToString(Request.Body));

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


    }
}
