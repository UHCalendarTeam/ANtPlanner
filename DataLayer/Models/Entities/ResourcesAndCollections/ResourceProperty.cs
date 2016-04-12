using System.ComponentModel.DataAnnotations;

namespace DataLayer.Entities
{
    /// <summary>
    ///     Define a set of properties for the
    ///     resources.
    /// </summary>
    public class ResourceProperty
    {
        [ScaffoldColumn(false)]
        public int ResourcePropertyId { get; set; }

        public CalendarResource Resource { get; set; }

        public int ResourceId { get; set; }

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