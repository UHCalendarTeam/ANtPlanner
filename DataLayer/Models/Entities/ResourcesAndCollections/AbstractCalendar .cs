using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Models.Entities.ACL;
using DataLayer.Models.Interfaces;

namespace DataLayer.Models.Entities.ResourcesAndCollections
{
    public abstract class AbstractCalendar : Entity, IPropertyContainer
    {
        #region Properties

        /// <summary>
        ///     The name of the calendar.
        /// </summary>
        [Required]
        public string Name
        { get; set; }

        /// <summary>
        ///     Identified uniquely the calendar.
        /// </summary>
        [Required]
        public string Url
        { get; set; }

        /// <summary>
        ///     The collection can belongs to a
        /// </summary>
        public int? PrincipalId
        { get; set; }

        /// <summary>
        ///     The principal can represent either a
        ///     user or a group. Both have a collection.
        /// </summary>
        public Principal Principal
        { get; set; }

        /// <summary>
        ///     Contains the properties of the collection.
        /// </summary>
        public ICollection<Property> Properties
        { get; set; }

        #endregion

        protected AbstractCalendar()
        {

        }

        protected AbstractCalendar(string url, string name, params Property[] properties)
        {
            this.Url = url;
            this.Name = name;
            Properties = new List<Property>(properties);
        }
    }
}
