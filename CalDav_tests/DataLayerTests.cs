using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using ACL.Core;
using DataLayer;
using DataLayer.ExtensionMethods;
using DataLayer.Models.ACL;
using DataLayer.Models.Entities;
using DataLayer.Repositories;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace CalDav_tests
{
    public class DataLayerTests
    {
        readonly string _email = "foo@gmail.com";
        readonly string _fullName = "John Doe";
        readonly string _password = "password1";
        readonly string _group = "C511";
        readonly string _career = "Computer Science";
        readonly string _faculty = "Computer Science";
        readonly int _year = 5;
       
        /// <summary>a
        /// Create a user in the DB
        /// </summary>
        [Fact]
        public async  Task UnitTest1()
        {
            var context = new CalDavContext(new DbContextOptions<CalDavContext>());

            var prinRepository = new PrincipalRepository(context);

            var user = prinRepository.CreateUserInSystem(_email, _fullName, _password);

            Assert.NotNull(user);
            
            context.SaveChanges();
            Assert.True(context.Users.Any());

            var dbUser = context.Users.FirstOrDefault(x => x.Email == _email);
            
            Assert.NotNull(dbUser);

            Assert.True(await prinRepository.ExistByStringIs(_email));

        }

     

        /// <summary>
        /// Testing the password verification
        /// </summary>
        [Fact]
        public async Task UnitTest3()
        {
            using (var context = new CalDavContext())
            {
                var prinRepository = new PrincipalRepository(context);
           

                var result = await prinRepository.VerifyPassword(_email, _password);

                Assert.True(result);
            }
        }

        /// <summary>
        /// Testing the student creation.
        /// </summary>
        [Fact]
        public void UnitTest4()
        {

            using (var context = new CalDavContext())
            {
                var prinRepository = new PrincipalRepository(context);
                prinRepository.CreateStudentInSystem("unittest4@gmail.com", "Student1", _password, _career, _group, _year);
                context.SaveChanges();
                var user = context.Students.FirstOrDefaultAsync(x => x.Email == _email && x.Career == _career);
               
                Assert.NotNull(user);
            }
        }

        /// <summary>
        /// Deleting entries from the DB.
        /// </summary>
        [Fact]
        public void UnitTest5()
        {
            var context = new CalDavContext();

            var user1 = context.Users.First();
            var id = user1.UserId;
            context.Users.Remove(user1);
            context.SaveChanges();
            Assert.Null(context.Users.FirstOrDefault(x=>x.UserId == id));

        }




        /// <summary>
        /// Taking the principal from the user.
        /// </summary>
        //[Fact]
        //public void UnitTest6()
        //{
        //    using (var _ctx = new CalDavContext())
        //    {

        //        var principal = _ctx.Principals.First(x=>x.Email == _email);

        //        var pri = _ctx.Users.First(x=>x.PrincipalId == principal.PrincipalId).Principal;

        //        Assert.Equal(pri.PrincipalId, principal.PrincipalId);
        //    }
        //}


        /// <summary>
        /// Creation of principal.
        /// </summary>
        [Fact]
        public void UnitTest7()
        {
            using (var _ctx = new CalDavContext())
            {

                var principal = new Principal()
                {
                    PrincipalURL = DataLayer.SystemProperties._userPrincipalUrl+_email
                };

                _ctx.Principals.Add(principal);

                _ctx.SaveChanges();

                //Assert.NotNull(_ctx.Principals.FirstOrDefault(p=>p.Displayname == "Adriano Flechilla"));
            }
        }

        /// <summary>
        /// Testing the CalendarCollection creation.
        /// </summary>
        [Fact]
        public void UnitTest8()
        {
            using (var cont = new CalDavContext())
            {
                var col = new CalendarCollection(DataLayer.SystemProperties._groupCollectionUrl+_group+"/", 
                    $"Group {_group} Calendar")
                {
                    //Group = _group
                };

                cont.CalendarCollections.Add(col);
                cont.SaveChanges();

               // Assert.NotNull(cont.CalendarCollections.FirstOrDefault(x=>x.Group == _group));
            }
        }

        /// <summary>
        /// testing the worker creation.
        /// </summary>
        [Fact]
        public void UnitTest9()
        {
            var context = new CalDavContext(new DbContextOptions<CalDavContext>());
            var prinRepository = new PrincipalRepository(context);

            var user = prinRepository.CreateWorkerInSystem("worker1@gmail.com", "worker", "UnitTest9 worker", _faculty,"Weboo");

            context.SaveChanges();
            Assert.True(context.Workers.Count() > 0);

            var worker = context.Workers.FirstOrDefault(x => x.Email == "worker1@gmail.com");

            Assert.NotNull(worker);
            Assert.NotNull(context.Principals.FirstOrDefault(x => x.PrincipalId == worker.PrincipalId.Value));
            Assert.NotNull(context.CalendarCollections.FirstOrDefault(x => x.PrincipalId == worker.PrincipalId.Value));
        }


        //[Fact]
        //public void UnitTest10()
        //{
        //    DataLayer.SqlMock.SeedDb_Fs();
        //}

       
    }
}
