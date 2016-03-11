using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CalDAV.Core;
using Microsoft.AspNet.Http.Extensions;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Routing;

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
            return CalDavRepository.MkCalendar(user, collection, Request.Body);
        }

        //PROPFIND
        [AcceptVerbs("PropFind", Route = "{user}/calendars/{collection}")]
        public string PropFind(string user, string collection)
        {

            return CalDavRepository.PropFind(user, collection, Response.Body);
        }

        //REPORT
        [AcceptVerbs("Report", Route = "{user}/calendars/{collection}")]
        public string Report(string user, string collection)
        {
            return CalDavRepository.Report(user, collection, Request.Body);
        }
        #endregion


        #region Calendar Object Resource Methods
        // PUT api/caldav/user_name/calendars/collection_name/object_resource_file_name
        [HttpPut("{user}/calendars/{collection}/{resourceId}")]
        public void Put(string user, string collection, string resourceId)
        {
            
            CalDavRepository.AddCalendarObjectResource(user, collection, resourceId, Request.Body);
        }

        // GET api/caldav/user_name/calendars/collection_name/object_resource_file_name
        [HttpGet("{user}/calendars/{collection}/{resourceId}")]
        public string Get(string user, string collection, string resourceId)
        {
            Response.Headers.Add("Etag", "testTag");
            //if auth the this
            return CalDavRepository.ReadCalendarObjectResource(user, collection, resourceId);
        }

        // DELETE api/values/5
        [HttpDelete("{user}/calendars/{collection}/{resourceId}")]
        public void Delete(string user, string collection, string resourceId)
        {
            CalDavRepository.DeleteCalendarObjectResource(user, collection, resourceId);
        }

        #endregion




        

       


    }
}
