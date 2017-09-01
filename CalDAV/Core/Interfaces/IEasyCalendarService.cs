using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
using DataLayer.Models.NonMappedEntities;
using Microsoft.AspNetCore.Http;

namespace CalDAV.Core.Interfaces
{
    public interface IEasyCalendarService
    {
         Task<IEnumerable<EasyCalendarEvent>> GetEasyCalendarEventsWithinMonthRangeAsync(int monthRange, HttpContext context);
    }
}
