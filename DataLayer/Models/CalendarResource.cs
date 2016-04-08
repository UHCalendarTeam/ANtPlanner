using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DataLayer;
using TreeForXml;

namespace DataLayer
{
    /// <summary>
    /// to store the main properties of a cal resource.
    /// </summary>
    public class CalendarResource
    {
        [ScaffoldColumn(false)]
        public int CalendarResourceId { get; set; }

        [Required]
        public string Href { get; set; }

        //public string Getetag { get; set; }

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

        //public int CollectionId { get; set; }

        public ICollection<ResourceProperty> Properties { get; set; }

        //public string DtStart { get; set; }

        /// <summary>
        /// The endDateTime of the resource if defined.
        /// Default value = DateTime.Max
        /// </summary>
        //public string DtEnd { get; set; }

        //public string Recurrence { get; set; }



        /// <summary>
        /// The duration of the resource if defined
        /// Default value ="".
        /// </summary>
        //public string Duration { get; set; }

        /// <summary>
        /// Returns the datetime value of when was created the resource.
        /// </summary>
        //public string CreationDate { get; set; }
        //public string Creationdate { get; set; }

        //public string Displayname { get; set; }

        //public string Getcontentlength { get; set; }

        //public string Getlastmodified { get; set; }

        //public string Getcontentlanguage { get; set; }

        ////public string Lockdiscovery { get; set; }

        //public string Supportedlock { get; set; }

    }
}