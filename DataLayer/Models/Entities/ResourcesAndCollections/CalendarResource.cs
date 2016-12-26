using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DataLayer.Models.Entities.OtherEnt;
using DataLayer.Models.Entities.OtherEnt.RelationsEnt;
using DataLayer.Models.Entities.Users;

namespace DataLayer.Models.Entities.ResourcesAndCollections
{
    /// <summary>
    ///     to store the main properties of a cal resource.
    /// </summary>
    public class CalendarResource : AbstractCalendar
    {
        public CalendarResource()
        {
        }

        public CalendarResource(string href, string name) : base(href, name, new Property[0])
        {
            Href = href;
        }

        public CalendarResource(string url, string name, params Property[] properties) : this(url, name)
        {

        }

        /// <summary>
        ///     The url where is the resource.
        /// </summary>
  
        public string Href { get; set; }

        //public string Getetag { get; set; }

        public string Uid { get; set; }


        /// <summary>
        ///     The collection where the resource is defined.
        /// </summary>
        public CalendarCollection CalendarCollection { get; set; }

        /// <summary>
        /// The CalendarResource have others CalendarResource related. 
        /// </summary>
        public ICollection<CalendarResource> RelatedCalendarResources { get; set; }

        ///// <summary>
        ///// The ACL properties of the resource.
        ///// </summary>
        //public AccessControlProperties AccessControlProperties { get; set; }

        //public int AccessControlPropertiesId { get; set; }

        /// <summary>
        /// The CalendarResourse content (.ics).
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        ///     The type of the calendarResouorse.
        /// </summary>
        public int CalendarResourseType { get; set; }


        #region auxiliary properties

        public readonly Dictionary<string, string> Namespaces = new Dictionary<string, string>
        {
            {"D", @"xmlns:D=""DAV:"""},
            {"C", @"xmlns:C=""urn:ietf:params:xml:ns:caldav"""}
        };

        public readonly Dictionary<string, string> NamespacesSimple = new Dictionary<string, string>
        {
            {"D", "DAV:"},
            {"C", "urn:ietf:params:xml:ns:caldav"}
        };

        #endregion


        //TODO: add the time properties and other useful properties for the queries

        public ICollection<User> Users { get; set; }

        public ICollection<RCalendarResourcesImagenFiles> RImageFilesResources { get; set; }

        public ICollection<RCalendarResourceLocation> RCalendarResourceLocations { get; set; }

        public ICollection<RCalendarResourcePerson> RCalendarResourcePersons { get; set; }

        public ICollection<RResourceCalendarResource> RResourceCalendarResources { get; set; }

        //public ICollection<RCalendarResourcesCalendarResource> RCalendarResourcesCalendarResources { get; set; }
    }

    public enum CalendarResourseType
    {
        VEvent,
        VTodo,
        VA
    }
}