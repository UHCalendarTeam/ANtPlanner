using System;
using System.Collections.Generic;
using DataLayer;
using DataLayer.Models.Entities;
using Microsoft.Data.Entity;
using Xunit;

namespace CalDav_tests
{
    /// <summary>
    ///     This class contains the test for the work
    ///     with the DB.
    /// </summary>
    public class DBTests
    {
        [Fact]
        public void UnitTest1()
        {
            var optionsBuilder = new DbContextOptionsBuilder<CalDavContext>();

            // This is the magic line
            optionsBuilder.UseInMemoryDatabase();

            var db = new CalDavContext(optionsBuilder.Options);
            var user = new User
            {
                Email = "foo@gmail.com",
               
            };
            var resources = new List<CalendarResource>
            {
                new CalendarResource
                {
                    //TODO: Adriano ver esto
                    //DtStart = DateTime.Now,
                    //DtEnd = DateTime.Now,
                    Name = "test.ics",
                    Href = ".......",
                    //Recurrence = "test",
                    // User = user,
                    Properties = new List<Property>
                    {
                        new Property
                        {
                            Name = "getetag",
                            Namespace = "DAV:",
                            Value = "12345",
                            IsVisible = true,
                            IsMutable = true,
                            IsDestroyable = false
                        },
                        new Property
                        {
                            Namespace = "DAV:",
                            Name = "creationdate",
                            Value = DateTime.Now.ToString(),
                            IsVisible = true,
                            IsMutable = true,
                            IsDestroyable = false
                        }
                    }

                    //Creationdate = DateTime.Now.ToString(),
                }
            };
            var collection = new List<CalendarCollection>
            {
                new CalendarCollection
                {
                    //Calendardescription = "Foo description",
                    Name = "Foo collection",
                    //User = user,
                    CalendarResources = resources /*,
                    SupportedCalendarComponentSet = new List<string>()*/,
                    //ResourceType = new List<string>(),

                    //TODO: Adriano ver esto ahora es xml hecho string
                    //ResourceType = new XmlTreeStructure("resourcetype", "DAV"),
                    //Displayname = "Display name",
                    Url = "url",
                    Properties = new List<Property>
                    {
                        new Property
                        {
                            Name = "calendar-description",
                            Namespace = @"xmlns:C=""urn:ietf:params:xml:ns:caldav""",
                            Value = "Foo description",
                            IsVisible = true,
                            IsMutable = true,
                            IsDestroyable = false
                        },
                        new Property
                        {
                            Name = "displayname",
                            Namespace = "DAV:",
                            Value = "Foo",
                            IsVisible = true,
                            IsMutable = true,
                            IsDestroyable = false
                        }
                    }
                }
            };
           
            // user.Resources = resources;
            db.Users.Add(user);
            db.SaveChanges();

            var principla = db.GetPrincipal("...");
            Assert.NotNull(user);
        }
    }
}