using System;
using System.ComponentModel.DataAnnotations;

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

        public string Etag { get; set; }

        public DateTime DtStart { get; set; }

        /// <summary>
        /// The endDateTime of the resource if defined.
        /// Default value = DateTime.Max
        /// </summary>
        public DateTime DtEnd { get; set; }

        public string Recurrence { get; set; }

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
        /// Define the comp type(i.e. VEVENT, VTODO)
        /// </summary>
        public string ResourceType { get; set; }

        /// <summary>
        /// The duration of the resource if defined
        /// Default value ="".
        /// </summary>
        public string Duration { get; set; }


    }
}