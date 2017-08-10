using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Models.Entities.ACL;

namespace DataLayer.Models.Entities.ResourcesAndCollections
{
    /// <summary>
    /// Represents the a principal's calendar home.
    /// This collections contains the principal's calendar collections.
    /// </summary>
    public class CalendarHome : AbstractCalendar
    {
        /// <summary>
        ///     Contains the calendar collections that are defined in this collection.
        /// </summary>
        public ICollection<CalendarCollection> CalendarCollections { get; set; }

        public CalendarHome()
        {

        }

        public CalendarHome(string url, string name, params Property[] properties) : base(url, name, properties)
        {
            CalendarCollections = new List<CalendarCollection>();
        }

        public Principal Principal { get; set; }

        public string PrincipalId {get;set;}
    }
}
