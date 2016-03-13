using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CalDAV.Models
{
    /// <summary>
    /// To store the data related to the calendar collections of the user.
    /// </summary>
    public class CalendarCollection
    {
        public int CalendarCollectionId { get; set; }

        public string Name { get; set; }

        [Required]
        public string Url { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public string DisplayName { get; set; }
        public string CalendarDescription { get; set; }

        public string CalendarTimeZone { get; set; }

        public ICollection<string> SupportedCalendarComponentSet { get; set; }

        public List<string> ResourceType { get; set; } 

        public int MaxResourceSize { get; set; }

        public DateTime MinDateTime { get; set; }

        public DateTime MaxDateTime { get; set; }

        public int MaxIntences { get; set; }

        public ICollection<CalendarResource> CalendarResources { get; set; }



    }
}