using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACL.Core.Authentication;
using CalDAV.Core.Interfaces;
using CalDAV.Core.Method_Extensions;
using CalDAV.Method_Extensions;
using CalDAV.Utils;
using DataLayer;
using DataLayer.Models.NonMappedEntities;
using ICalendar.Calendar;
using ICalendar.CalendarComponents;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using TreeForXml;

namespace CalDavServices.Controllers
{
    [Produces("application/json")]
    [Route("api/EasyCalendar")]
    public class EasyCalendarController : Controller
    {
        private IAuthenticate _authenticate;
        private readonly IEasyCalendarService _easyCalendarService;

        public EasyCalendarController([FromServices] IAuthenticate authenticate,
            IEasyCalendarService easyCalendarService)
        {
            _authenticate = authenticate;
            _easyCalendarService = easyCalendarService;
        }

        // GET: api/EasyCalendar
        [HttpGet("collections/{groupOrUser}/{principalId}/collectionName")]
        public async Task<IEnumerable<EasyCalendarEvent>> Get()
        {
            return await _easyCalendarService.GetEasyCalendarEventsWithinMonthRangeAsync(2, HttpContext);
        }

        // GET: api/EasyCalendar/5
        [HttpGet("{id}", Name = "Get")]
        public EasyCalendarEvent Get(int id)
        {
            return null;
        }

        // POST: api/EasyCalendar
        [HttpPost]
        public void Post([FromBody] EasyCalendarEvent value)
        {
        }

        // PUT: api/EasyCalendar/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] EasyCalendarEvent value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}