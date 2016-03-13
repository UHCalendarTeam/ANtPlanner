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


        [Fact]
        public void UnitTest3()
        {
            var doc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<C:calendar-query xmlns:D=""DAV:""
xmlns:C=""urn:ietf:params:xml:ns:caldav"">
<D:prop>
<D:getetag/>
<C:calendar-data>
<C:comp name=""VCALENDAR"">
<C:prop name=""VERSION""/>
<C:comp name=""VEVENT"">
<C:prop name=""SUMMARY""/>
<C:prop name=""UID""/>
<C:prop name=""DTSTART""/>
<C:prop name=""DTEND""/>
<C:prop name=""DURATION""/>
<C:prop name=""RRULE""/>
<C:prop name=""RDATE""/>
<C:prop name=""EXRULE""/>
<C:prop name=""EXDATE""/>
<C:prop name=""RECURRENCE-ID""/>
</C:comp>
<C:comp name=""VTIMEZONE""/>
</C:comp>
</C:calendar-data>
</D:prop>
<C:filter>
<C:comp-filter name=""VCALENDAR"">
<C:comp-filter name=""VEVENT"">
<C:time-range start=""20060104T000000Z""
end=""20060105T000000Z""/>
</C:comp-filter>
</C:comp-filter>
</C:filter>
</C:calendar-query>";
            var result = XMLParsers.GenericParser(doc);
            Assert.NotNull(result.GetChildAtAnyLevel("filter").GetChild("comp-filter"));
        }

        /// <summary>
        /// Checking the getChildAtAnyLevel
        /// </summary>
        [Fact]
        public void UnitTest4()
        {
            var doc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<C:calendar-query xmlns:D=""DAV:""
xmlns:C=""urn:ietf:params:xml:ns:caldav"">
<D:prop>
<D:getetag/>
<C:calendar-data>
<C:comp name=""VCALENDAR"">
<C:prop name=""VERSION""/>
<C:comp name=""VEVENT"">
<C:prop name=""SUMMARY""/>
<C:prop name=""UID""/>
<C:prop name=""DTSTART""/>
<C:prop name=""DTEND""/>
<C:prop name=""DURATION""/>
<C:prop name=""RRULE""/>
<C:prop name=""RDATE""/>
<C:prop name=""EXRULE""/>
<C:prop name=""EXDATE""/>
<C:prop name=""RECURRENCE-ID""/>
</C:comp>
<C:comp name=""VTIMEZONE""/>
</C:comp>
</C:calendar-data>
</D:prop>
<C:filter>
<C:comp-filter name=""VCALENDAR"">
<C:comp-filter name=""VEVENT"">
<C:time-range start=""20060104T000000Z""
end=""20060105T000000Z""/>
</C:comp-filter>
</C:comp-filter>
</C:filter>
</C:calendar-query>";
            var result = XMLParsers.GenericParser(doc);
            Assert.Equal(result.GetChildAtAnyLevel("filter").GetChild("comp-filter").Attributes["name"], "VCALENDAR");
        }






    }
}
