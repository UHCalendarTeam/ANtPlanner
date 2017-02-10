using System.Collections.Generic;

namespace DataLayer.Models.Entities.ResourcesAndCollections
{
    /// <summary>
    ///     To store the data related to the calendar collections of the user.
    /// </summary>
    public class CalendarCollection : AbstractCalendar
    {
        /// <summary>
        ///     Contains the resources that are defined in this collection.
        /// </summary>
        public ICollection<CalendarResource> CalendarResources { get; set; }

        /// <summary>
        ///     The FK to the calendarHome where the calendarCollection
        ///     belongs.
        /// </summary>

        /// <summary>
        ///     Reference to the calendar home of this collection.
        /// </summary>

        public string CalendarHomeId { get; set; }

        public CalendarHome CalendarHome { get; set; }

        public CalendarCollection()
        {
        }

        public CalendarCollection(string url, string name, params Property[] properties) : base(url, name, properties)
        {
            CalendarResources = new List<CalendarResource>();
        }
    }
}