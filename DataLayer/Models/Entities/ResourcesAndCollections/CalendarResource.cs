using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataLayer.Models.Entities
{
    /// <summary>
    ///     to store the main properties of a cal resource.
    /// </summary>
    public class CalendarResource
    {
       

        [ScaffoldColumn(false)]
        public int CalendarResourceId { get; set; }

        /// <summary>
        ///     The url where is the resource.
        /// </summary>
        [Required]
        public string Href { get; set; }

        [Required]
        public string Name { get; set; }

        //public string Getetag { get; set; }

        public string Uid { get; set; }

        public int CalendarCollectionId { get; set; }

        /// <summary>
        ///     The collection where the resource is defined.
        /// </summary>
        public CalendarCollection CalendarCollection { get; set; }


        public ICollection<Property> Properties { get; set; }

        ///// <summary>
        ///// The ACL properties of the resource.
        ///// </summary>
        //public AccessControlProperties AccessControlProperties { get; set; }

        //public int AccessControlPropertiesId { get; set; }

        #region auxiliary properties

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

        #endregion

        #region Initializers and constructors

        private void InitializeStandardResourceProperties()
        {
            Properties.Add(new Property("getcontenttype", NamespacesSimple["D"])
            {
                IsVisible = true,
                IsMutable = false,
                IsDestroyable = false,
                Value = $"<D:getcontenttype {Namespaces["D"]}>text/calendar</D:getcontenttype>"
            });

            Properties.Add(new Property("resourcetype", NamespacesSimple["D"])
            { IsVisible = true, IsDestroyable = false, IsMutable = false, Value = $"<D:resourcetype {Namespaces["D"]}/>" });

            Properties.Add(new Property("displayname", NamespacesSimple["D"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = true,
                Value = $"<D:displayname {Namespaces["D"]}>Test</D:displayname>"
            });

            //TODO: Generar Etag en creacion.
            Properties.Add(new Property("getetag", NamespacesSimple["D"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = false,
                Value = $"<D:getetag {Namespaces["D"]}>{Guid.NewGuid()}</D:getetag>"
            });

            Properties.Add(new Property("creationdate", NamespacesSimple["D"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = false,
                Value = $"<D:creationdate {Namespaces["D"]}>{DateTime.Now}</D:creationdate>"
            });

            Properties.Add(new Property("getcontentlanguage", NamespacesSimple["D"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = true,
                Value = $"<D:getcontentlanguage {Namespaces["D"]}>en</D:getcontentlanguage>"
            });

            Properties.Add(new Property("getcontentlength", NamespacesSimple["D"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = false,
                Value = $"<D:getcontentlength {Namespaces["D"]}>0</D:getcontentlength>"
            });

            Properties.Add(new Property("getlastmodified", NamespacesSimple["D"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = false,
                Value = $"<D:getlastmodified {Namespaces["D"]}>{DateTime.Now}</D:getlastmodified>"
            });
        }

        public CalendarResource()
        {

        }

        public CalendarResource(string href, string name)
        {
            Name = name;
            Href = href;
            Properties = new List<Property>();
            InitializeStandardResourceProperties();
        }

        #endregion
    }
}