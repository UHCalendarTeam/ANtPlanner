using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Models.Entities.ResourcesAndCollections;

namespace UHCalendarUI.Models
{
    public class PrincipalViewModel
    {
        public string Email { get; set; }

        public string Url { get; set; }

        public string CalendarHomeName { get; set; }

        public string Password { get; set; }

        public IEnumerable<CalendarCollection> CalendarCollections { get; set; }

        public List<CalendarCollection> CurrentCallendars { get; set; }
    }
}
