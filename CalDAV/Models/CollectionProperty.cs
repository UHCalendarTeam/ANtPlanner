using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CalDAV.Models
{
    public class CollectionProperty
    {
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
