using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Models.ACL;

namespace DataLayer.Models.Entities.ResourcesAndCollections
{
    /// <summary>
    /// Represents the a principal's calendar home.
    /// This collections contains the principal's calendar collections.
    /// </summary>
    public class CalendarHome
    {
        public CalendarHome()
        {
            
        }

        public CalendarHome(string url, string name, params Property[] properties)
        {
            Url = url;
            Name = name;
            CalendarCollections = new List<CalendarCollection>();
            Properties = new List<Property>();
            if (properties != null && properties.Length > 0)
            {
                Properties.AddRange(properties);
            }
            InitializeStandardCollectionProperties(name);
        }


        [ScaffoldColumn(false)]
        public int CalendarHomeId { get; set; }

        /// <summary>
        ///     The name of the calendar home.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Identified uniquely the calendar.
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
        ///     Contains the calendar collections that are defined in this collection.
        /// </summary>
        public ICollection<CalendarCollection> CalendarCollections { get; set; }

        /// <summary>
        ///     Contains the properties of the collection.
        /// </summary>
        public List<Property> Properties { get; set; }



        private void InitializeStandardCollectionProperties(string name)
        {
            Properties.Add(new Property("calendar-timezone",SystemProperties.NamespacesValues["C"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = false,
                Value = $"<C:calendar-timezone {SystemProperties.Namespaces["C"]}>LaHabana/Cuba</C:calendar-timezone>"
            });

            Properties.Add(new Property("getcontentlength", SystemProperties.NamespacesValues["D"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = false,
                Value = $"<D:getcontentlength {SystemProperties.Namespaces["D"]}>0</D:getcontentlength>"
            });

            Properties.Add(new Property("resourcetype", SystemProperties.NamespacesValues["D"])
            {
                IsVisible = true,
                IsDestroyable = false,
                IsMutable = false,
                Value =
                    $"<D:resourcetype {SystemProperties.Namespaces["D"]}><D:collection/></D:resourcetype>"
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
