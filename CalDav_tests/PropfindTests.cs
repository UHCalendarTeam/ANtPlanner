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
            XmlTreeStructure root = new XmlTreeStructure("multistatus", "D");
            string test = root.MainNamespace;
            Assert.True(root.NodeName == "multistatus" && test == "D");
        }
    }
}
