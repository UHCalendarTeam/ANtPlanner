using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DataLayer.Models.ACL;

namespace DataLayer.Models.Entities
{
    /// <summary>
    ///     To store the data related to the calendar collections of the user.
    /// </summary>
    public class CalendarCollection
    {
        private readonly Dictionary<string, string> _namespaces = new Dictionary<string, string>
        {
            {"D", @"xmlns:D=""DAV:"""},
            {"C", @"xmlns:C=""urn:ietf:params:xml:ns:caldav"""},
            {"S", @"xmlns:S=""http://calendarserver.org/ns/"""}
        };

        private readonly Dictionary<string, string> _namespacesSimple = new Dictionary<string, string>
        {
            {"D", "DAV:"},
            {"C", "urn:ietf:params:xml:ns:caldav"},
            {"S", "http://calendarserver.org/ns/"}
        };

        public CalendarCollection()
        {
        }

        public CalendarCollection(string url, string name, params Property[] properties)
        {
            Url = url;
            Name = name;
            CalendarResources = new List<CalendarResource>();
            Properties = new List<Property>();
            if (properties != null && properties.Length > 0)
            {
                Properties = properties;
            }
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
        ///     Contains the properties of the collection.
        /// </summary>
        public ICollection<Property> Properties { get; set; }


        private void InitializeStandardCollectionProperties(string name)
        {
            Properties.Add(new Property("calendar-timezone", _namespacesSimple["C"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = false,
                Value = $"<C:calendar-timezone {_namespaces["C"]}>LaHabana/Cuba</C:calendar-timezone>"
            });

            Properties.Add(new Property("max-resource-size", _namespacesSimple["C"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = false,
                Value = $"<C:max-resource-size {_namespaces["C"]}>102400</C:max-resource-size>"
            });


            Properties.Add(new Property("min-date-time", _namespacesSimple["C"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = false,
                Value = $"<C:min-date-time {_namespaces["C"]}>{SystemProperties.MinDateTime()}</C:min-date-time>"
            });

            Properties.Add(new Property("max-date-time", _namespacesSimple["C"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = false,
                Value = $"<C:max-date-time {_namespaces["C"]}>{SystemProperties.MaxDateTime()}</C:max-date-time>"
            });


            Properties.Add(new Property("max-instances", _namespacesSimple["C"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = false,
                Value = $"<C:max-instances {_namespaces["C"]}>10</C:max-instances>"
            });


            Properties.Add(new Property("getcontentlength", _namespacesSimple["D"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = false,
                Value = $"<D:getcontentlength {_namespaces["D"]}>0</D:getcontentlength>"
            });


            Properties.Add(new Property("supported-calendar-component-set", _namespacesSimple["C"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = true,
                Value =
                    $@"<C:supported-calendar-component-set {_namespaces["C"]
                        }><C:comp name=""VEVENT""/><C:comp name=""VTODO""/></C:supported-calendar-component-set>"
            });

            Properties.Add(new Property("supported-calendar-data", _namespacesSimple["C"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = false,
                Value =
                    $@"<C:supported-calendar-data {_namespaces["C"]
                        }><C:comp name=""VEVENT""/><C:comp name=""VTODO""/></C:supported-calendar-data>"
            });

            Properties.Add(new Property("calendar-description", _namespacesSimple["C"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = true,
                Value = $"<C:calendar-description {_namespaces["C"]}>No Description Available</C:calendar-description>"
            });

            Properties.Add(new Property("resourcetype", _namespacesSimple["D"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = false,
                Value =
                    $"<D:resourcetype {_namespaces["D"]}><D:collection/><C:calendar xmlns:C=\"urn:ietf:params:xml:ns:caldav\"/></D:resourcetype>"
            });

            Properties.Add(new Property("displayname", _namespacesSimple["D"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = true,
                Value = string.IsNullOrEmpty(name) ? null : $"<D:displayname {_namespaces["D"]}>{name}</D:displayname>"
            });

            Properties.Add(new Property("creationdate", _namespacesSimple["D"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = true,
                Value = $"<D:creationdate {_namespaces["D"]}>{DateTime.Now}</D:creationdate>"
            });
            Properties.Add(new Property("getctag", _namespacesSimple["S"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = false,
                Value = $@"<S:getctag {_namespaces["S"]} >{Guid.NewGuid()}</S:getctag>"
            });
            Properties.Add(PropertyCreation.CreateSupportedPrivilegeSetForResources());
        }
    }
}