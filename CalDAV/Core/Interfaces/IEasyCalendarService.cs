using System.Collections.Generic;
using System.Threading.Tasks;
using DataLayer.Models;
using Microsoft.AspNetCore.Http;

namespace CalDAV.Core.Interfaces
{
    public interface IEasyCalendarService
    {
         Task<IEnumerable<EasyCalendarEvent>> GetEasyCalendarEventsWithinMonthRangeAsync(int monthRange, HttpContext context);
    }
}
