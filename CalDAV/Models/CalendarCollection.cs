using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using CalDAV.CALDAV_Properties.Interfaces;
using CalDAV.Utils.XML_Processors;
using TreeForXml;

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

        public string Displayname { get; set; }

        public string Calendardescription { get; set; }

        public ICollection<CalendarResource> Calendarresources { get; set; }

        public string Creationdate { get; set; }

        public string Getcontenttype { get; set; }

        public string Getetag { get; set; }

        public string Getlastmodified { get; set; }

        public string Getcontentlanguage { get; set; }

        public string Lockdiscovery { get; set; }

        public string Resourcetype { get; set; }

        public string Supportedlock { get; set; }


    }
}