using System.ComponentModel.DataAnnotations;

namespace DataLayer.Entities
{
    /// <summary>
    ///     Define a set of properties for the
    ///     collections.
    /// </summary>
    public class CollectionProperty
    {
        public CollectionProperty()
        {
            
        }
        public CollectionProperty(string name, string nameSpace)
        {
            Name = name;
            Namespace = nameSpace;
        }
        [ScaffoldColumn(false)]
        public int CollectionPropertyId { get; set; }

        public CalendarCollection Collection { get; set; }

        public int CollectionId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Namespace { get; set; }

        public string Value { get; set; }

        public bool IsDestroyable { get; set; }

        public bool IsMutable { get; set; }

        public bool IsVisible { get; set; }
    }
}
