using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CalDAV.Core;
using CalDAV.Models;
using Microsoft.AspNet.Http.Extensions;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Routing;
using Microsoft.Extensions.Primitives;

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


        #region Collection Methods

        //MKCAL api\caldav\{username}\calendars\{collection_name}
        [AcceptVerbs("MkCalendar", Route = "{user}/calendars/{collection}")]
        public string MkCalendar(string user, string collection)
        {
            var propertiesAndHeaders = new Dictionary<string, string>();
            propertiesAndHeaders.Add("userEmail", user);
            propertiesAndHeaders.Add("collectionName", collection);
            
            return CalDavRepository.MkCalendar(propertiesAndHeaders, StreamToString(Request.Body));
        }

        //PROPFIND COLLECTIONS
        [AcceptVerbs("PropFind", Route = "{user}/calendars/{collection}")]
        public string PropFind(string user, string collection)
        {
            var propertiesAndHeaders = new Dictionary<string, string>();
            propertiesAndHeaders.Add("userEmail", user);
            propertiesAndHeaders.Add("collectionName", collection);
            

            return CalDavRepository.PropFind(propertiesAndHeaders, StreamToString(Request.Body)).ToString();
        }

        //PROPFIND RESOURCES
        [AcceptVerbs("PropFind", Route = "{user}/calendars/{collection}/{calendarResource}")]
        public string PropFind(string user, string collection, string calendarResource)
        {
            var propertiesAndHeaders = new Dictionary<string, string>();
            propertiesAndHeaders.Add("userEmail", user);
            propertiesAndHeaders.Add("collectionName", collection);
            propertiesAndHeaders.Add("calendarResourceId", calendarResource);

            StringValues depth;
            if(Request.Headers.TryGetValue("Depth", out depth)) 
                propertiesAndHeaders.Add("depth", depth);

            var res = CalDavRepository.PropFind(propertiesAndHeaders, StreamToString(Request.Body)).ToString();

            return res;
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
        public void Put(string user, string collection, string calendarResourceId)
        {
            var propertiesAndHeaders = new Dictionary<string, string>();
            propertiesAndHeaders.Add("userEmail",user);
            propertiesAndHeaders.Add("collectionName", collection);
            propertiesAndHeaders.Add("calendarResourceId", calendarResourceId);
            propertiesAndHeaders.Add("body", StreamToString(Request.Body));

            StringValues ifMatch;
            StringValues etags;

            string etag;

            if (Request.Headers.TryGetValue("If-Match", out ifMatch) && ifMatch.Count==1)
            {
                propertiesAndHeaders.Add("If-Match", ifMatch.FirstOrDefault());
            }
            else if(Request.Headers.ContainsKey("If-None-Match"))
            {
                propertiesAndHeaders.Add("If-Match", "*");
            }
            if (Request.Headers.TryGetValue("Etag", out etags))
            {
                propertiesAndHeaders.Add("Etag", etags.FirstOrDefault());
            }
            

            CalDavRepository.AddCalendarObjectResource(propertiesAndHeaders, out etag);
        }

        [HttpGet]
        public string test()
        {
            return "Test";
        }

        // GET api/caldav/user_name/calendars/collection_name/object_resource_file_name
        [HttpGet("{user}/calendars/{collection}/{calendarResourceId}")]
        public string Get(string user, string collection, string calendarResourceId)
        {
            var propertiesAndHeaders = new Dictionary<string, string>();
            propertiesAndHeaders.Add("userEmail", user);
            propertiesAndHeaders.Add("collectionName", collection);
            propertiesAndHeaders.Add("calendarResourceId", calendarResourceId);
            //if auth the this
            string etag;
            var result =  CalDavRepository.ReadCalendarObjectResource(propertiesAndHeaders, out etag);
            Response.Headers.Add("Etag", etag);
            return result;
        }

        // DELETE api/values/5
        [HttpDelete("{user}/calendars/{collection}/{calendarResourceId}")]
        public void Delete(string user, string collection, string calendarResourceId)
        {
            var propertiesAndHeaders = new Dictionary<string, string>();
            propertiesAndHeaders.Add("userEmail", user);
            propertiesAndHeaders.Add("collectionName", collection);
            propertiesAndHeaders.Add("calendarResourceId", calendarResourceId);

            CalDavRepository.DeleteCalendarObjectResource(propertiesAndHeaders);
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

        

       


    }
}
