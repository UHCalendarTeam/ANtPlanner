using System;
using System.Collections.Generic;
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
        
        
        //MKCAL \\api\caldav\{username}\calendars\{collection_name}
        [AcceptVerbs("MkCal",Route = "{user}/calendars/{collection}")]
        public string MkCal(string user, string collection)
        {
            var body = HttpContext.Request.Body.ToString();
            return CalDavRepository.MkCalendar(user, collection, body );

        }

        [AcceptVerbs("PropFind")]
        public string PropFind()
        {
            var url = HttpContext.Request.GetDisplayUrl();
            var body = HttpContext.Request.Body.ToString();

            return CalDavRepository.PropFind(url, body);
        }

        [AcceptVerbs("Request")]
        public string Request()
        {
            var url = HttpContext.Request.GetDisplayUrl();
            var body = HttpContext.Request.Body.ToString();

            return CalDavRepository.Request(url, body);
        }


        #region Default Methods
        //// GET: api/values
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/values/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/values
        //[HttpPost]
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/values/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody]string value)
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
