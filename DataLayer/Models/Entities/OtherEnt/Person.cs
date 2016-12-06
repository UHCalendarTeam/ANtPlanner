using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataLayer.Models.Entities.OtherEnt.RelationsEnt;
using DataLayer.Models.Entities.ResourcesAndCollections;
using DataLayer.Models.Interfaces;

namespace DataLayer.Models.Entities.OtherEnt
{
    public class Person : Entity,IPropertyContainer
    {

        [Required]
        public string DisplayName
        { get; set; }

        public int Age
        { get; set; }

        [Required]
        public string Description
        { get; set; }

        public string Phone
        { get; set; }

        public string Email
        { get; set; }

        public ICollection<Property> Properties
        { get; set; }

        public ICollection<RCalendarResourcePerson> RCalendarResourcePerson { get; set; }

        public ICollection<RPersonResource> RPersonResource
        { get; set; }

        public ICollection<RPersonLocation> RPersonLocations
        { get; set; }

        public ICollection<RImagenFilesPersons> RImagenFilesPersons { get; set; }

        //ver Roles con ASP.Core
        public PersonRoles Role
        { get; set; }

        [NotMapped]//todo<ver como lo resuelvo
        public ICollection<Languages> Languageses
        { get; set; }

        public Person()
        {

        }

        public Person(string displayName, int age, string description, params Property[] properties)
        {
            this.DisplayName = displayName;
            this.Age = age;
            this.Description = description;
            this.Properties = new List<Property>(properties);
        }

        public Person(string displayName, int age, string description, PersonRoles role, ICollection<Languages> languageses, params Property[] properties)
            : this(displayName, age, description, properties)
        {
            this.Role = role;
            this.Languageses = languageses;
        }


    }

    public enum PersonRoles
    {
        Worker,
        User,
        Estudent,
        Client,
        ClientPotential

    }

    public enum Languages
    {
        Spanish,
        English,
        Germany,
        French,
        Russian
    }
}
