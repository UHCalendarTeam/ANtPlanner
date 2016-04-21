using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataLayer.Models.Entities
{
    /// <summary>
    ///     To store the data related to the calendar collections of the user.
    /// </summary>
    public class CalendarCollection
    {
        private readonly Dictionary<string, string> Namespaces = new Dictionary<string, string>
        {
            {"D", @"xmlns:D=""DAV:"""},
            {"C", @"xmlns:C=""urn:ietf:params:xml:ns:caldav"""}
        };

        private readonly Dictionary<string, string> NamespacesSimple = new Dictionary<string, string>
        {
            {"D", "DAV:"},
            {"C", "urn:ietf:params:xml:ns:caldav"}
        };

        public CalendarCollection() { }

        public CalendarCollection(string url, string name)
        {
            Url = url;
            Name = name;
            CalendarResources = new List<CalendarResource>();
            Properties = new List<Property>();
            InitializeStandardCollectionProperties(Name);
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

        public int UserId { get; set; }

        /// <summary>
        ///     The owner of the collection.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        ///     Contains the resources that are defined in this collection.
        /// </summary>
        public ICollection<CalendarResource> CalendarResources { get; set; }

        /// <summary>
        ///     Contains the properties of the collection.
        /// </summary>
        public ICollection<Property> Properties { get; set; }

        private void InitializeStandardCollectionProperties(string name)
        {
            Properties.Add(new Property("calendar-timezone", NamespacesSimple["C"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = false,
                Value = $"<C:calendar-timezone {Namespaces["C"]}>LaHabana/Cuba</C:calendar-timezone>"
            });

            Properties.Add(new Property("max-resource-size", NamespacesSimple["C"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = false,
                Value = $"<C:max-resource-size {Namespaces["C"]}>102400</C:max-resource-size>"
            });


            Properties.Add(new Property("min-date-time", NamespacesSimple["C"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = false,
                Value = $"<C:min-date-time {Namespaces["C"]}>{this.MinDateTime()}</C:min-date-time>"
            });

            Properties.Add(new Property("max-date-time", NamespacesSimple["C"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = false,
                Value = $"<C:max-date-time {Namespaces["C"]}>{this.MaxDateTime()}</C:max-date-time>"
            });


            Properties.Add(new Property("max-instances", NamespacesSimple["C"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = false,
                Value = $"<C:max-instances {Namespaces["C"]}>10</C:max-instances>"
            });


            Properties.Add(new Property("getcontentlength", NamespacesSimple["D"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = false,
                Value = $"<D:getcontentlength {Namespaces["D"]}>0</D:getcontentlength>"
            });


            Properties.Add(new Property("supported-calendar-component-set", NamespacesSimple["C"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = true,
                Value =
                    $@"<C:supported-calendar-component-set {Namespaces["C"]
                        }>&lt;C:comp name=""VEVENT""/&gt;&lt;C:comp name=""VTODO""/&gt;</C:supported-calendar-component-set>"
            });

            Properties.Add(new Property("supported-calendar-data", NamespacesSimple["C"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = false,
                Value =
                    $@"<C:supported-calendar-data {Namespaces["C"]
                        }><C:comp name=""VEVENT""/><C:comp name=""VTODO""/></C:supported-calendar-data>"
            });

            Properties.Add(new Property("calendar-description", NamespacesSimple["C"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = true,
                Value = $"<C:calendar-description {NamespacesSimple["C"]}>No Description Available</C:calendar-description>"
            });

            Properties.Add(new Property("resourcetype", NamespacesSimple["D"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = false,
                Value =
                    $"<D:resourcetype {Namespaces["D"]}><D:collection/><C:calendar xmlns:C=\"urn:ietf:params:xml:ns:caldav\"/></D:resourcetype>"
            });

            Properties.Add(new Property("displayname", NamespacesSimple["D"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = true,
                Value = string.IsNullOrEmpty(name) ? null : $"<D:displayname {Namespaces["D"]}>{name}</D:displayname>"
            });

            Properties.Add(new Property("creationdate", NamespacesSimple["D"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = true,
                Value = $"<D:creationdate {Namespaces["D"]}>{DateTime.Now}</D:creationdate>"
            });
            Properties.Add(new Property("getctag", NamespacesSimple["C"])
            {
                IsVisible = false,
                IsDestroyable = false,
                IsMutable = true,
                Value = $"<C:getctag {Namespaces["C"]}>{Guid.NewGuid()}</C:getctag>"
            });
        }
    }
}