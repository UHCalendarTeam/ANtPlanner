using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CalDAV.Core;
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

        //Constructor
        public CalDavController(ICalDav repoCalDav)
        {
            CalDavRepository = repoCalDav;
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

        //PROPFIND
        [AcceptVerbs("PropFind", Route = "{user}/calendars/{collection}")]
        public string PropFind(string user, string collection)
        {
            var propertiesAndHeaders = new Dictionary<string, string>();
            propertiesAndHeaders.Add("userEmail", user);
            propertiesAndHeaders.Add("collectionName", collection);
            

            return CalDavRepository.PropFind(propertiesAndHeaders, StreamToString(Request.Body));
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
        [HttpPut("{user}/calendars/{collection}/{resourceId}")]
        public void Put(string user, string collection, string resourceId)
        {
            var propertiesAndHeaders = new Dictionary<string, string>();
            propertiesAndHeaders.Add("userEmail",user);
            propertiesAndHeaders.Add("collectionName", collection);
            propertiesAndHeaders.Add("resourceId", resourceId);

            StringValues IfMatch;

            if (Request.Headers.TryGetValue("If-Match", out IfMatch) && IfMatch.Count==1)
            {
                propertiesAndHeaders.Add("If-Match", IfMatch.FirstOrDefault());
            }
            else if(Request.Headers.ContainsKey("If-None-Match"))
            {
                propertiesAndHeaders.Add("If-Match", "*");
            }
            

            CalDavRepository.AddCalendarObjectResource(propertiesAndHeaders, StreamToString(Request.Body));
        }

        // GET api/caldav/user_name/calendars/collection_name/object_resource_file_name
        [HttpGet("{user}/calendars/{collection}/{resourceId}")]
        public string Get(string user, string collection, string resourceId)
        {
            var propertiesAndHeaders = new Dictionary<string, string>();
            propertiesAndHeaders.Add("userEmail", user);
            propertiesAndHeaders.Add("collectionName", collection);
            propertiesAndHeaders.Add("resourceId", resourceId);
            //if auth the this
            string etag;
            var result =  CalDavRepository.ReadCalendarObjectResource(propertiesAndHeaders, out etag);
            Response.Headers.Add("Etag", etag);
            return result;
        }

        // DELETE api/values/5
        [HttpDelete("{user}/calendars/{collection}/{resourceId}")]
        public void Delete(string user, string collection, string resourceId)
        {
            var propertiesAndHeaders = new Dictionary<string, string>();
            propertiesAndHeaders.Add("userEmail", user);
            propertiesAndHeaders.Add("collectionName", collection);
            propertiesAndHeaders.Add("resourceId", resourceId);

            CalDavRepository.DeleteCalendarObjectResource(propertiesAndHeaders);
        }

        //REPORT api/values/5
        [AcceptVerbs("Report", Route = "{user}/calendars/{collection}/{resourceId}")]
        public string Report(string user, string collection, string resourceId)
        {
            var propertiesAndHeaders = new Dictionary<string, string>();
            propertiesAndHeaders.Add("userEmail", user);
            propertiesAndHeaders.Add("collectionName", collection);
            propertiesAndHeaders.Add("resourceId", resourceId);

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
