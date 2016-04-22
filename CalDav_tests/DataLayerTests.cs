using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer;
using DataLayer.ExtensionMethods;
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
       
        /// <summary>
        /// Create a user in the DB
        /// </summary>
        [Fact]
        public void UnitTest1()
        {
            var context = new CalDavContext(new DbContextOptions<CalDavContext>());

            var user = context.CreateUserInSystem(_email, _fullName, _password);
            context.Users.Add(user);
           
            var dbUser = context.Users.FirstOrDefault(x => x.Email == _email);
            
            Assert.NotNull(dbUser);

        }
    }
}
