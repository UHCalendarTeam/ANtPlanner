using DataLayer;
using Xunit;

namespace CalDav_tests
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class Class1
    {
        [Fact]
        public void UniTest1()
        {
            var fsm = new FileSystemManagement();
            Assert.False(fsm.AddUserFolder("Nacho"));
        }
    }
}