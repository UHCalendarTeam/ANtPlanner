using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Models.Interfaces;

namespace DataLayer.Models.Entities
{
    public class Entity:IEntity<string>
    {
        [Key]
        public string Id { get; set; }

        public Entity()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
