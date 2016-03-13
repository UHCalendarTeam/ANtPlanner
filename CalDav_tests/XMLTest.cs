using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalDAV.Utils.XML_Processors;
using Xunit;


namespace CalDav_tests
{
    
    public class XMLTest
    {
        [Fact]
        public void ParseMKCALENDAR()
        {
            var doc = @"<?xml version=""1.0"" encoding=""utf-8""?>
  <C:mkcalendar xmlns:D=""DAV:"" xmlns:C=""urn:ietf:params:xml:ns:caldav"">
 <D:set>
  <D:prop>
   <D:displayname> Lisa Events</D:displayname>
      <C:calendar-description xml:lang=""en""> Calendar restricted to events.</C:calendar-description>
            <C:supported-calendar-component-set>
             <C:comp name=""VEVENT"" />
              </C:supported-calendar-component-set>              
</D:prop>
</D:set>
</C:mkcalendar>";
            var dict =XMLParsers.XMLMKCalendarParser(doc);
            var expectedDict = new Dictionary<string, List<string>>
            {
                {"displayname", new List<string>() {"Lisa Events"}},
                {"calendar-description", new List<string>() {"Calendar restricted to events."}},
                {"supported-calendar-component-set", new List<string>() {"VEVENT"}}
            };
            Assert.Equal(dict.Count, expectedDict.Count);
        }



        [Fact]
        public void UnitTest2()
        {
            var tree = new XMLTreeStructure("node1", new List<string>() { "DAV", "urn:ietf:params:xml:ns:caldav" });
            tree.AddChild(new XMLTreeStructure("child1")).AddChild(new XMLTreeStructure("child2"));
            tree.GetChildAtAnyLevel("child2").AddChild(new XMLTreeStructure("child3"))
                .GetChild("child3").AddChild(new XMLTreeStructure("child4")).GetChildAtAnyLevel("child4")
                .AddChild(new XMLTreeStructure("child5"));
            var child6 = new XMLTreeStructure("child6");
            tree.GetChildAtAnyLevel("child5").AddChild(child6);
            Assert.Equal(tree.GetChildAtAnyLevel("child6"), child6);
        }






    }
}
