using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CalDAV;
using Microsoft.AspNet.Http.Extensions;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Routing;


namespace CalDav_Services.Controllers
{
    [Route("api/[controller]")]
    public class CalDavController : Controller
    {
        [FromServices]
        ICalDav CalDavRepository { get; set; }

        public CalDavController(ICalDav repoCalDav)
        {
            CalDavRepository = repoCalDav;
        }
        
        
        //MKCAL api\caldav\{username}\calendars\{collection_name}
        [AcceptVerbs("MkCalendar",Route = "{user}/calendars/{collection}")]
        public string MkCalendar(string user, string collection)
        {
            var bodyStream = HttpContext.Request.Body;
            var stringReader = new StreamReader(bodyStream);
            var body = stringReader.ReadToEnd();
            return CalDavRepository.MkCalendar(user, collection, body );

        }

        [AcceptVerbs("PropFind")]
        public string PropFind()
        {
            var bodyStream = HttpContext.Request.Body;
            var stringReader = new StreamReader(bodyStream);
            var body = stringReader.ReadToEnd();

            return CalDavRepository.PropFind(body);
        }

        [AcceptVerbs("Request")]
        public string Request()
        {
            var bodyStream = HttpContext.Request.Body;
            var stringReader = new StreamReader(bodyStream);
            var body = stringReader.ReadToEnd();

            return CalDavRepository.Request(body);
        }

        // PUT api/caldav/user_name/calendars/collection_name/object_resource_file_name
        [HttpPut("{user}/calendars/{collection}/{resourceId}")]
        public void Put(string user, string collection, string resourceId)
        {
            var bodyStream = HttpContext.Request.Body;
            var stringReader = new StreamReader(bodyStream);
            var body = stringReader.ReadToEnd();

            CalDavRepository.AddCOR(user, collection, resourceId,body);
        }

        // GET api/caldav/user_name/calendars/collection_name/object_resource_file_name
        [HttpGet("{user}/calendars/{collection}/{resourceId}")]
        public string Get(string  resourceId)
        {
            return CalDavRepository.ReadCOR();
        }


        #region Default Methods
        //// GET: api/values
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}



        //// POST api/values
        //[HttpPost]
        //public void Post([FromBody]string value)
        //{
        //}



        //// DELETE api/values/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
        #endregion
    }
}
