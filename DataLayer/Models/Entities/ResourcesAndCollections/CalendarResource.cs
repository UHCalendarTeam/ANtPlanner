using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DataLayer.Models.ACL;

namespace DataLayer.Entities
{

    /// <summary>
    ///     to store the main properties of a cal resource.
    /// </summary>
    public class CalendarResource
    {
        private Dictionary<string, string> Namespaces = new Dictionary<string, string> { { "D", @"xmlns:D=""DAV:""" }, { "C", @"xmlns:C=""urn:ietf:params:xml:ns:caldav""" } };
        private Dictionary<string, string> NamespacesSimple = new Dictionary<string, string> { { "D", "DAV:" }, { "C", "urn:ietf:params:xml:ns:caldav" } };

        private void InitializeStandardResourceProperties()
        {
            Properties.Add(new ResourceProperty("getcontenttype", NamespacesSimple["D"])
            { IsVisible = true, IsMutable = false, IsDestroyable = false, Value = $"<D:getcontenttype {Namespaces["D"]}>text/icalendar</D:getcontenttype>" });

            Properties.Add(new ResourceProperty("resourcetype", NamespacesSimple["D"])
            { IsVisible = true, IsDestroyable = false, IsMutable = false, Value = $"<D:resourcetype {Namespaces["D"]}/>" });

            Properties.Add(new ResourceProperty("displayname", NamespacesSimple["D"])
            { IsVisible = true, IsDestroyable = false, IsMutable = true });

            //TODO: Generar Etag en creacion.
            Properties.Add(new ResourceProperty("getetag", NamespacesSimple["D"])
            { IsVisible = true, IsDestroyable = false, IsMutable = false });

            Properties.Add(new ResourceProperty("creationdate", NamespacesSimple["D"])
            { IsVisible = true, IsDestroyable = false, IsMutable = false, Value = $"<D:creationdate {Namespaces["D"]}>{DateTime.Now}</D:creationdate>" });

            Properties.Add(new ResourceProperty("getcontentlanguage", NamespacesSimple["D"])
            { IsVisible = true, IsDestroyable = false, IsMutable = true, Value = $"<D:getcontentlanguage {Namespaces["D"]}>en</D:getcontentlanguage>" });

            Properties.Add(new ResourceProperty("getcontentlength", NamespacesSimple["D"])
            { IsVisible = true, IsDestroyable = false, IsMutable = false, Value = $"<D:getcontentlength {Namespaces["D"]}>0</D:getcontentlength>" });

            Properties.Add(new ResourceProperty("getlastmodified", NamespacesSimple["D"])
            { IsVisible = true, IsDestroyable = false, IsMutable = false, Value = $"<D:getlastmodified {Namespaces["D"]}>{DateTime.Now}</D:getlastmodified>" });
        }
        public CalendarResource()
        {
            Properties = new List<ResourceProperty>();
            InitializeStandardResourceProperties();
        }
        public CalendarResource(string href)
        {
            Href = href;
            Properties = new List<ResourceProperty>();
            InitializeStandardResourceProperties();
        }

        [ScaffoldColumn(false)]
        public int CalendarResourceId { get; set; }

        /// <summary>
        /// The url where is the resource.
        /// </summary>
        [Required]
        public string Href { get; set; }

        //public string Getetag { get; set; }

        public string Uid { get; set; }

        public int CollectionId { get; set; }

        /// <summary>
        ///     The collection where the resource is defined.
        /// </summary>
        public CalendarCollection Collection { get; set; }
       

        public ICollection<ResourceProperty> Properties { get; set; }
        
        /// <summary>
        /// The ACL properties of the resource.
        public AccessControlProperties AccessControlProperties { get; set; }

        public int AccessControlPropertiesId { get; set; }

    }
}