using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalDAV.Utils.XML_Processors;
using Xunit;

namespace CalDav_tests
{
    public class PropfindTests
    {
        [Fact]
        public void CreateRootWithNamespace()
        {
            XMLTreeStructure root = new XMLTreeStructure("multistatus", new List<string>() {"D= \"DAV: \""});
            string test = root.Namespaces.Single();
            Assert.True(root.NodeName == "multistatus" && test == "D= \"DAV: \"");
        }
    }
}
