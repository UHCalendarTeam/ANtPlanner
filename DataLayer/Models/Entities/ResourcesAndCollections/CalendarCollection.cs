using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DataLayer.Models.ACL;

namespace DataLayer.Models.Entities
{
    /// <summary>
    ///     To store the data related to the calendar collections of the user.
    /// </summary>
    public class CalendarCollection
    {
        public CalendarCollection()
        {
        }

        public CalendarCollection(string url, string name, params Property[] properties)
        {
            Url = url;
            Name = name;
            CalendarResources = new List<CalendarResource>();
            Properties = new List<Property>(properties);
          
            InitializeStandardCollectionProperties(name);
        }

        [ScaffoldColumn(false)]
        public int CalendarCollectionId { get; set; }

        /// <summary>
        ///     The name of the collection.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Identified uniquely the collection.
        /// </summary>
        [Required]
        public string Url { get; set; }

        /// <summary>
        ///     The collection can belongs to a
        /// </summary>
        public int? PrincipalId { get; set; }

        /// <summary>
        ///     The principal can represent either a
        ///     user or a group. Both have a collection.
        /// </summary>
        public Principal Principal { get; set; }

        /// <summary>
        ///     Contains the resources that are defined in this collection.
        /// </summary>
        public ICollection<CalendarResource> CalendarResources { get; set; }

        /// <summary>
        ///     The FK to the calendarHome where the calendarCollection
        ///     belongs.
        /// </summary>
        public int CalendarHomeId { get; set; }

        /// <summary>
        ///     Reference to the calendar home of this collection.
        /// </summary>
        public CalendarHome CalendarHome { get; set; }

        /// <summary>
        ///     Contains the properties of the collection.
        /// </summary>
        public ICollection<Property> Properties { get; set; }


        private void InitializeStandardCollectionProperties(string name)
        {
            Properties.Add(new Property("calendar-timezone", SystemProperties.NamespacesValues["C"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = false,
                Value = $"<C:calendar-timezone {SystemProperties.Namespaces["C"]}>LaHabana/Cuba</C:calendar-timezone>"
            });

            Properties.Add(new Property("max-resource-size", SystemProperties.NamespacesValues["C"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = false,
                Value = $"<C:max-resource-size {SystemProperties.Namespaces["C"]}>102400</C:max-resource-size>"
            });


            Properties.Add(new Property("min-date-time", SystemProperties.NamespacesValues["C"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = false,
                Value = $"<C:min-date-time {SystemProperties.Namespaces["C"]}>{SystemProperties.MinDateTime()}</C:min-date-time>"
            });

            Properties.Add(new Property("max-date-time", SystemProperties.NamespacesValues["C"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = false,
                Value = $"<C:max-date-time {SystemProperties.Namespaces["C"]}>{SystemProperties.MaxDateTime()}</C:max-date-time>"
            });


            Properties.Add(new Property("max-instances", SystemProperties.NamespacesValues["C"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = false,
                Value = $"<C:max-instances {SystemProperties.Namespaces["C"]}>10</C:max-instances>"
            });


            Properties.Add(new Property("getcontentlength", SystemProperties.NamespacesValues["D"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = false,
                Value = $"<D:getcontentlength {SystemProperties.Namespaces["D"]}>0</D:getcontentlength>"
            });


            Properties.Add(new Property("supported-calendar-component-set", SystemProperties.NamespacesValues["C"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = true,
                Value =
                    $@"<C:supported-calendar-component-set {SystemProperties.Namespaces["C"]
                        }><C:comp name=""VEVENT""/><C:comp name=""VTODO""/></C:supported-calendar-component-set>"
            });

            Properties.Add(new Property("supported-calendar-data", SystemProperties.NamespacesValues["C"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = false,
                Value =
                    $@"<C:supported-calendar-data {SystemProperties.Namespaces["C"]
                        }><C:comp name=""VEVENT""/><C:comp name=""VTODO""/></C:supported-calendar-data>"
            });

            Properties.Add(new Property("calendar-description", SystemProperties.NamespacesValues["C"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = true,
                Value = $"<C:calendar-description {SystemProperties.Namespaces["C"]}>No Description Available</C:calendar-description>"
            });

            Properties.Add(new Property("resourcetype", SystemProperties.NamespacesValues["D"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = false,
                Value =
                    $"<D:resourcetype {SystemProperties.Namespaces["D"]}><D:collection/><C:calendar xmlns:C=\"urn:ietf:params:xml:ns:caldav\"/></D:resourcetype>"
            });

            Properties.Add(new Property("displayname", SystemProperties.NamespacesValues["D"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = true,
                Value = string.IsNullOrEmpty(name) ? null : $"<D:displayname {SystemProperties.Namespaces["D"]}>{name}</D:displayname>"
            });

            Properties.Add(new Property("creationdate", SystemProperties.NamespacesValues["D"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = true,
                Value = $"<D:creationdate {SystemProperties.Namespaces["D"]}>{DateTime.Now}</D:creationdate>"
            });
            Properties.Add(new Property("getctag", SystemProperties.NamespacesValues["CS"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = false,
                Value = $@"<CS:getctag {SystemProperties.Namespaces["CS"]} >{Guid.NewGuid()}</CS:getctag>"
            });
            Properties.Add(PropertyCreation.CreateSupportedPrivilegeSetForResources());
        }
    }
}