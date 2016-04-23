using DataLayer;
using DataLayer.Models.Entities;
using Xunit;

namespace CalDav_tests
{
    public class CalendarCollectionAndResourcesTest
    {
        [Fact]
        public void GetAllPropertiesName()
        {
            var user = new User
            {
                Email = "hohn@noname.com",
          
            };
            var colecction = new CalendarCollection();

            var treeNames = colecction.GetAllPropertyNames();
            Assert.NotNull(treeNames);
        }

        [Fact]
        public void GetAllVisibleProperties()
        {
        }
    }
}