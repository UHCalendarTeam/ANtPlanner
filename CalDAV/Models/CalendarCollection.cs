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

        public string DisplayName { get; set; }

        public string CalendarDescription { get; set; }

        public ICollection<CalendarResource> CalendarResources { get; set; }

        public string CreationDate { get; set; }

        public string GetContentType { get; set; }

        public string GetEtag { get; set; }

        public string GetLastModified { get; set; }

        public string GetContentLanguage { get; set; }

        public string LockDiscovery { get; set; }

        public string ResourceType { get; set; }

        public string SupportedLock { get; set; }


    }
}