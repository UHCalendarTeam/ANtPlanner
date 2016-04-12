using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataLayer.Entities
{
    /// <summary>
    ///     To store the data related to the calendar collections of the user.
    /// </summary>
    public class CalendarCollection
    {
        [ScaffoldColumn(false)]
        public int CalendarCollectionId { get; set; }

        /// <summary>
        ///     The na,e of the collection.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     The url that identifies this collection.
        /// </summary>
        [Required]
        public string Url { get; set; }

        public int UserId { get; set; }

        /// <summary>
        ///     The owner of the collection.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        ///     Contains the resources of this collection.
        /// </summary>
        public ICollection<CalendarResource> Calendarresources { get; set; }

        /// <summary>
        ///     Contains the properties of the collection.
        /// </summary>
        public ICollection<CollectionProperty> Properties { get; set; }

        //public string Displayname { get; set; }

        //public string Calendardescription { get; set; }


        //public string Creationdate { get; set; }

        //public string Getetag { get; set; }

        //public string Lockdiscovery { get; set; }

        //public string Resourcetype { get; set; }

        //public string Supportedlock { get; set; }
    }
}