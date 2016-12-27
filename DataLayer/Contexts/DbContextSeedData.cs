using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DataLayer.Models.Entities;
using DataLayer.Models.Entities.OtherEnt;
using DataLayer.Models.Entities.OtherEnt.Resource;
using DataLayer.Models.Entities.ResourcesAndCollections;
using DataLayer.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Remotion.Linq.Clauses;

namespace DataLayer.Contexts
{
    public class DbContextSeedData
    {
        private readonly Random _r;
        private CalDavContext _context;
        private Dictionary<Type, Func<Entity>> _factories;

        public DbContextSeedData(CalDavContext calDavContext)
        {
            _context = calDavContext;
            _r = new Random();

            _factories = new Dictionary<Type, Func<Entity>>()
            {
                {typeof(Person),CreateRandomPerson},
                {typeof(Location),CreateRandomLocation},
                {typeof(ResourceType) ,CreateRandomResourceType},
                {typeof(Resource) ,CreateRandomResource},
                {typeof(FileImage) ,CreateRandomFileImage},
                {typeof(CalendarHome) ,CreateRandomCalendarHome},
                {typeof(CalendarCollection) ,CreateRandomCalendarCollection},
                {typeof(CalendarResource) ,CreateRandomCalendarResource},
            };
        }

        /// <summary>
        /// Insert new funtion in order to create seed entity.
        /// </summary>
        /// <param name="type">type of the entity to resolve seed funtion</param>
        /// <param name="func">funtion to create seed</param>
        public void InsertEntityFactory(Type type, Func<Entity> func)
        {
            _factories.Add(type, func);
        }

        //todo:recordar pedir el tipo generico que estoy llamando con un tipo incorrecto
        public void InsertEntityFactory(DbSet<Entity> dbEntity, Func<Entity> func)
        {
            InsertEntityFactory(dbEntity.GetType().GenericTypeArguments[0], func);
        }

        //return true if factory contain type,false in others case.
        public bool ModifyEntityFactory(Type type, Func<Entity> func)
        {
            if (_factories.ContainsKey(type))
            {
                _factories[type] = func;
                return true;
            }
            InsertEntityFactory(type, func);
            return false;
        }

        //todo:ver si cuando retorna el argumento generico retorna el verdaredo y no el tipo Entity.
        //todo:en caso que ocura dejar solo modifuEntityFactory con el primer parametro de tipo type.
        public bool ModifyEntityFactory(DbSet<Entity> dbEntity, Func<Entity> func)
        {
            return ModifyEntityFactory(dbEntity.GetType().GenericTypeArguments[0], func);
        }

        private Person CreateRandomPerson()
        {
            var displayName = "The person name is" + Guid.NewGuid();
            var age = _r.Next(5, 70);
            var description = "Person description :" + Guid.NewGuid();

            return new Person(displayName, age, description);
        }

        private Location CreateRandomLocation()
        {
            string displayName = "The location name is" + Guid.NewGuid();
            int price = _r.Next(5, 70);
            string description = "Location description :" + Guid.NewGuid();
            int clasification = _r.Next(0, 5);
            string phone = _r.Next(1000000, 9999999).ToString();
            string email = Guid.NewGuid() + "@domain";
            return new Location(displayName, description, phone, email, clasification, price);
        }

        private static ResourceType CreateRandomResourceType()
        {
            return new ResourceType(Guid.NewGuid().ToString());
        }

        private static Resource CreateRandomResource()
        {
            return new Resource(CreateRandomResourceType());
        }

        private static FileImage CreateRandomFileImage()
        {
            return new FileImage(Guid.NewGuid().ToString());
        }

        private static CalendarHome CreateRandomCalendarHome()
        {
            string name = "The Calendar Home name is " + Guid.NewGuid();
            string uri = "The Calendar Home uri is" + name + '@' + Guid.NewGuid();
            return new CalendarHome(uri, name);
        }

        private static CalendarCollection CreateRandomCalendarCollection()
        {
            string name = "The Calendar Collection name is " + Guid.NewGuid();
            string uri = "The Calendar Collection uri is" + name + '@' + Guid.NewGuid();

            return new CalendarCollection(uri, name);
        }

        private static CalendarResource CreateRandomCalendarResource()
        {
            string name = "The Calendar Resource name is " + Guid.NewGuid();
            string uri = "The Calendar Resource uri is" + name + '@' + Guid.NewGuid();
            return new CalendarResource(uri, name);
        }

        public void Seed(int entitiesAmmount = 30)
        {
            if (!_context.Persons.Any())
            {
                StanadardEntitySeed(_context.Persons, entitiesAmmount);
            }
            else
            {
                #region 
                //Console.WriteLine("element");
                //foreach (var VARIABLE in _context.Persons)
                //{
                 //   Console.WriteLine(VARIABLE.DisplayName);
                //}
                #endregion
            }
            if (!_context.Locations.Any())
                StanadardEntitySeed(_context.Locations, entitiesAmmount);
            if (!_context.ResourceTypes.Any())
                StanadardEntitySeed(_context.ResourceTypes, entitiesAmmount);
            if (!_context.Resources.Any())
                StanadardEntitySeed(_context.Resources, entitiesAmmount);
            if (!_context.ImageFile.Any())
                StanadardEntitySeed(_context.ImageFile, entitiesAmmount);
        }

        public void StanadardEntitySeed<TEnt>(DbSet<TEnt> dbSet, int entityAmmout) where TEnt : Entity
        {
            Type aux = typeof(TEnt);
            EntitySeed(dbSet, _factories[aux], entityAmmout);
        }

        public void EntitySeed<TEnt>(DbSet<TEnt> dbSet, Func<Entity> entityFactory, int entityAmmout) where TEnt : Entity
        {
            List<TEnt> result = new List<TEnt>(entityAmmout);
            for (int i = 0; i < entityAmmout; i++)
            {
                TEnt newEntity = entityFactory() as TEnt;
                if (newEntity != null)
                    result.Add(newEntity);
            }

            dbSet.AddRange(result);
            _context.SaveChanges();
        }
    }
}

