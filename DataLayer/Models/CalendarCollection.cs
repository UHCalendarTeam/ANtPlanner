using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataLayer
{
    /// <summary>
    /// To store the data related to the calendar collections of the user.
    /// </summary>
    public class CalendarCollection
    {
        private Dictionary<string, string> Namespaces = new Dictionary<string, string> { { "D", @"xmlns:D=""DAV:""" }, { "C", @"xmlns:C=""urn:ietf:params:xml:ns:caldav""" } };
        private Dictionary<string, string> NamespacesSimple = new Dictionary<string, string> { { "D", "DAV:" }, { "C", "urn:ietf:params:xml:ns:caldav" } };

        private void InitializeStandardCollectionProperties(string name)
        {
            Properties.Add(new CollectionProperty("calendar-timezone", NamespacesSimple["C"])
            {IsVisible = true, IsDestroyable = false, IsMutable = false, Value = $"<C:calendar-timezone {Namespaces["C"]}>LaHabana/Cuba</C:calendar-timezone>" });

            Properties.Add(new CollectionProperty("max-resource-size", NamespacesSimple["C"])
            { IsVisible = true, IsDestroyable = false, IsMutable = false, Value = $"<C:max-resource-size {Namespaces["C"]}>102400</C:max-resource-size>" });

            Properties.Add(new CollectionProperty("min-date-time", NamespacesSimple["C"])
            { IsVisible = true, IsDestroyable = false, IsMutable = false, Value = $"<C:min-date-time {Namespaces["C"]}>{this.MinDateTime()}</C:min-date-time>" });

            Properties.Add(new CollectionProperty("max-date-time", NamespacesSimple["C"])
            { IsVisible = true, IsDestroyable = false, IsMutable = false, Value = $"<C:max-date-time {Namespaces["C"]}>{this.MaxDateTime()}</C:max-date-time>" });

            Properties.Add(new CollectionProperty("max-instances", NamespacesSimple["C"])
            { IsVisible = true, IsDestroyable = false, IsMutable = false, Value = $"<C:max-instances {Namespaces["C"]}>10</C:max-instances>" });

            Properties.Add(new CollectionProperty("getcontentlength", NamespacesSimple["D"])
            { IsVisible = true, IsDestroyable = false, IsMutable = false, Value = $"<D:getcontentlength {Namespaces["D"]}>0</D:getcontentlength>"});

            Properties.Add(new CollectionProperty("supported-calendar-component-set", NamespacesSimple["C"])
            { IsVisible = true, IsDestroyable = false, IsMutable = false, Value = $@"<C:supported-calendar-component-set {Namespaces["C"]}>&lt;C:comp name=""VEVENT""/&gt;&lt;C:comp name=""VTODO""/&gt;</C:supported-calendar-component-set>" });

            Properties.Add(new CollectionProperty("supported-calendar-data", NamespacesSimple["C"])
            { IsVisible = true, IsDestroyable = false, IsMutable = false, Value = $@"<C:supported-calendar-data {Namespaces["C"]}><C:comp name=""VEVENT""/><C:comp name=""VTODO""/></C:supported-calendar-data>" });

            Properties.Add(new CollectionProperty("calendar-description", NamespacesSimple["C"])
            { IsVisible = true, IsDestroyable = false, IsMutable = true });

            Properties.Add(new CollectionProperty("resourcetype", NamespacesSimple["D"])
            { IsVisible = true, IsDestroyable = false, IsMutable = false, Value = $"<D:resourcetype {Namespaces["D"]}><D:collection/><C:calendar xmlns:C=\"urn:ietf:params:xml:ns:caldav\"/></D:resourcetype>" });

            Properties.Add(new CollectionProperty("displayname", NamespacesSimple["D"])
            { IsVisible = true, IsDestroyable = false, IsMutable = true, Value = string.IsNullOrEmpty(name)?null: $"<D:displayname {Namespaces["D"]}>{name}</D:displayname>" });

            Properties.Add(new CollectionProperty("creationdate", NamespacesSimple["D"])
            { IsVisible = true, IsDestroyable = false, IsMutable = true, Value = $"<D:creationdate {Namespaces["D"]}>{DateTime.Now}</D:creationdate>" });
        }
        public CalendarCollection()
        {
            Calendarresources = new List<CalendarResource>();
            Properties = new List<CollectionProperty>();
            InitializeStandardCollectionProperties(null);
        }
        public CalendarCollection(string url, string name)
        {
            Url = url;
            Name = name;
            Calendarresources = new List<CalendarResource>();
            Properties = new List<CollectionProperty>();
            InitializeStandardCollectionProperties(Name);


        }

        [ScaffoldColumn(false)]
        public int CalendarCollectionId { get; set; }

        public string Name { get; set; }

        [Required]
        public string Url { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public ICollection<CalendarResource> Calendarresources { get; set; }

        public ICollection<CollectionProperty> Properties { get; set; }

    }
}