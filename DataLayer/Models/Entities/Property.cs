using System.ComponentModel.DataAnnotations;
using DataLayer.Models.Entities.ACL;
using DataLayer.Models.Entities.OtherEnt;
using DataLayer.Models.Entities.OtherEnt.Resource;
using DataLayer.Models.Entities.ResourcesAndCollections;

namespace DataLayer.Models.Entities
{
    /// <summary>
    ///     This entity represents the properties
    ///     of any other entity.
    /// </summary>
    public class Property
    {
        public Property(string name, string nameSpace)
        {
            Name = name;
            Namespace = nameSpace;
            IsMutable = true;
            IsDestroyable = true;
            IsVisible = true;
        }

        public Property()
        {
        }

        [ScaffoldColumn(false)]
        public int PropertyId { get; set; }

        /// <summary>
        ///     The name of the property.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        ///     The ns of the property in the xml.
        /// </summary>
        [Required]
        public string Namespace { get; set; }

        /// <summary>
        ///     The string representation of the
        ///     xml that contains the values of the
        ///     property.
        ///     Should contains the full ns of the node.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        ///     True if the property can be deleted.
        ///     False otherwise.
        /// </summary>
        public bool IsDestroyable { get; set; }

        /// <summary>
        ///     True if the property can be modified.
        ///     False otherwise.
        /// </summary>
        public bool IsMutable { get; set; }

        /// <summary>
        ///     True if the property can be send in a
        ///     PROFIND request with allprop.
        /// </summary>
        public bool IsVisible { get; set; }

        #region Foreign keys and navigators

        public CalendarResource CalendarResource { get; set; }

        public string CalendarResourceId { get; set; }

        public CalendarCollection CalendarCollection { get; set; }

        public string CalendarCollectionId { get; set; }
        public CalendarHome CalendarHome { get; set; }


        public int PersonId { get; set; }
        public Person Person { get; set; }

        public string ResourceTypeId { get; set; }

        public ResourceType ResourceType { get; set; }

        public int LocationId { get; set; }
        public Location Location { get; set; }

        public string CalendarHomeId { get; set; }

        public string PricipalId { get; set; }

        public Principal Principal { get; set; }

        public override bool Equals(object obj)
        {
            Property other = obj as Property;
            if (ReferenceEquals(other, null))
                return false;

            if (!other.Name.Equals(Name))
                return false;

            if (!other.Namespace.Equals(Namespace))
                return false;

            return other.Value.Equals(Value);
        }

        #endregion
    }
}