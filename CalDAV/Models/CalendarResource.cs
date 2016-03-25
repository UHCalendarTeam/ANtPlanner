using System;
using System.ComponentModel.DataAnnotations;
using CalDAV.CALDAV_Properties.Interfaces;
using CalDAV.Core;
using CalDAV.Utils.XML_Processors;
using TreeForXml;

namespace CalDAV.Models
{
    /// <summary>
    /// to store the main properties of a cal resource.
    /// </summary>
    public class CalendarResource
    {
        [ScaffoldColumn(false)]
        public int CalendarResourceId { get; set; }

        [Required]
        public string FileName { get; set; }

        public string GetEtag { get; set; }

        //public string DtStart { get; set; }

        /// <summary>
        /// The endDateTime of the resource if defined.
        /// Default value = DateTime.Max
        /// </summary>
        //public string DtEnd { get; set; }

        //public string Recurrence { get; set; }

        public string Uid { get; set; }

        public int UserId { get; set; }

        /// <summary>
        /// The owner of the resource
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// The collection where the resource is defined.
        /// </summary>
        public CalendarCollection Collection { get; set; }

        /// <summary>
        /// The duration of the resource if defined
        /// Default value ="".
        /// </summary>
        //public string Duration { get; set; }

        /// <summary>
        /// Returns the datetime value of when was created the resource.
        /// </summary>
        //public string CreationDate { get; set; }
        public string CreationDate { get; set; }

        public string DisplayName { get; set; }
        
        public string GetContentLength { get; set; }

        public string GetContentType { get; set; }

        public string GetLastModified { get; set; }

        public string GetContentLanguage { get; set; }

        public string LockDiscovery { get; set; }

        public string SupportedLock { get; set; }

    }
}