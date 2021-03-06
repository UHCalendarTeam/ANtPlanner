﻿//using System.Collections.Generic;
//using System.Linq;
//using System.Xml.Linq;
//using TreeForXml;
//using Xunit;

//namespace CalDav_tests
//{
//    public class XMLTest
//    {
//        [Fact]
//        public void ParseMKCALENDAR()
//        {
//            /* var dict =XMLParsers.XMLMKCalendarParser(doc);
//             var expectedDict = new Dictionary<string, List<string>>
//             {
//                 {"displayname", new List<string>() {"Lisa Events"}},
//                 {"calendar-description", new List<string>() {"Calendar restricted to events."}},
//                 {"supported-calendar-component-set", new List<string>() {"VEVENT"}}
//             };
//             Assert.Equal(dict.Count, expectedDict.Count);*/
//        }


//        [Fact]
//        public void UnitTest2()
//        {
//            var tree = new XmlTreeStructure("node1", "DAV:",
//                new Dictionary<string, string>
//                {
//                    {"D", "DAV:"},
//                    {"C", "urn:ietf:params:xml: ns: caldav"}
//                });
//            tree.AddChild(new XmlTreeStructure("child1", null)).
//                AddChild(new XmlTreeStructure("child2", null));
//            IXMLTreeStructure child2;
//            tree.GetChildAtAnyLevel("child2", out child2);
//            child2.AddChild(new XmlTreeStructure("child3", null))
//                .GetChild("child3").AddChild(new XmlTreeStructure("child4", null));
//            IXMLTreeStructure child4;
//            tree.GetChildAtAnyLevel("child4", out child4);
//            child4.AddChild(new XmlTreeStructure("child5", null));
//            var child6 = new XmlTreeStructure("child6", null);
//            IXMLTreeStructure child5;
//            tree.GetChildAtAnyLevel("child5", out child5);
//            child5.AddChild(child6);
//            IXMLTreeStructure child6_1;
//            tree.GetChildAtAnyLevel("child6", out child6_1);
//            IXMLTreeStructure test;
//            tree.GetChildAtAnyLevel("prop", out test);

//            Assert.Null(test);

//            test = tree.GetChild("prop");

//            Assert.Null(test);
//            Assert.Equal(child6_1, child6);
//        }


//        [Fact]
//        public void UnitTest3()
//        {
//            var doc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
//<C:calendar-query xmlns:D=""DAV:""
//xmlns:C=""urn:ietf:params:xml:ns:caldav"">
//<D:prop>
//<D:getetag/>
//<C:calendar-data>
//<C:comp name=""VCALENDAR"">
//<C:prop name=""VERSION""/>
//<C:comp name=""VEVENT"">
//<C:prop name=""SUMMARY""/>
//<C:prop name=""UID""/>
//<C:prop name=""DTSTART""/>
//<C:prop name=""DTEND""/>
//<C:prop name=""DURATION""/>
//<C:prop name=""RRULE""/>
//<C:prop name=""RDATE""/>
//<C:prop name=""EXRULE""/>
//<C:prop name=""EXDATE""/>
//<C:prop name=""RECURRENCE-ID""/>
//</C:comp>
//<C:comp name=""VTIMEZONE""/>
//</C:comp>
//</C:calendar-data>
//</D:prop>
//<C:filter>
//<C:comp-filter name=""VCALENDAR"">
//<C:comp-filter name=""VEVENT"">
//<C:time-range start=""20060104T000000Z""
//end=""20060105T000000Z""/>
//</C:comp-filter>
//</C:comp-filter>
//</C:filter>
//</C:calendar-query>";
//            var result = XmlTreeStructure.Parse(doc);
//            IXMLTreeStructure filter;
//            Assert.True(result.GetChildAtAnyLevel("filter", out filter));
//            Assert.NotNull(filter.GetChild("comp-filter"));
//        }

//        /// <summary>
//        ///     Checking the getChildAtAnyLevel
//        /// </summary>
//        [Fact]
//        public void UnitTest4()
//        {
//            var doc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
//<C:calendar-query xmlns:D=""DAV:""
//xmlns:C=""urn:ietf:params:xml:ns:caldav"">
//<D:prop>
//<D:getetag/>
//<C:calendar-data>
//<C:comp name=""VCALENDAR"">
//<C:prop name=""VERSION""/>
//<C:comp name=""VEVENT"">
//<C:prop name=""SUMMARY""/>
//<C:prop name=""UID""/>
//<C:prop name=""DTSTART""/>
//<C:prop name=""DTEND""/>
//<C:prop name=""DURATION""/>
//<C:prop name=""RRULE""/>
//<C:prop name=""RDATE""/>
//<C:prop name=""EXRULE""/>
//<C:prop name=""EXDATE""/>
//<C:prop name=""RECURRENCE-ID""/>
//</C:comp>
//<C:comp name=""VTIMEZONE""/>
//</C:comp>
//</C:calendar-data>
//</D:prop>
//<C:filter>
//<C:comp-filter name=""VCALENDAR"">
//<C:comp-filter name=""VEVENT"">
//<C:time-range start=""20060104T000000Z""
//end=""20060105T000000Z""/>
//</C:comp-filter>
//</C:comp-filter>
//</C:filter>
//</C:calendar-query>";
//            var xDoc = XDocument.Parse(doc);
//            xDoc.ToString();
//            var temp1 = xDoc.Root.Attributes().Where(x => x.IsNamespaceDeclaration);
//            var item = xDoc.CreateWriter();

//            var result = XmlTreeStructure.Parse(doc);
//            IXMLTreeStructure filter;
//            Assert.True(result.GetChildAtAnyLevel("filter", out filter));
//            Assert.Equal(filter.GetChild("comp-filter").Attributes["name"], "VCALENDAR");
//        }

//        [Fact]
//        public void UnitTestFoo()
//        {
//            XNamespace ns = "DAV:";
//            XNamespace ns1 = "Attribute:";
//            var xmlTree1 = new XElement("Root",
//                new XElement(ns + "Child1", null),
//                new XElement(ns + "Child2", 2),
//                new XElement(ns1 + "Child3", 3),
//                new XElement(ns + "Child4", 4),
//                new XElement(ns + "Child5", 5),
//                new XElement(ns + "Child6", 6)
//                );
//            xmlTree1.Add(new XAttribute(XNamespace.Xmlns + "D", "DAV:"));
//            xmlTree1.Add(new XAttribute(XNamespace.Xmlns + "R", "Attribute:"));


//            var document = new XDocument(new XDeclaration("1.0", "utf-8", null), xmlTree1);

//            var docStr = document.ToString(SaveOptions.DisableFormatting);
//        }


//        [Fact]
//        public void IXmlTreeStrucureToString()
//        {
//            var doc = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
//<C:calendar-query xmlns:D=""DAV:""
//xmlns:C=""urn:ietf:params:xml:ns:caldav"">
//<D:prop>
//<D:getetag/>
//<C:calendar-data>
//<C:comp name=""VCALENDAR"">
//<C:prop name=""VERSION""/>
//<C:comp name=""VEVENT"">
//<C:prop name=""SUMMARY""/>
//<C:prop name=""UID""/>
//<C:prop name=""DTSTART""/>
//<C:prop name=""DTEND""/>
//<C:prop name=""DURATION""/>
//<C:prop name=""RRULE""/>
//<C:prop name=""RDATE""/>
//<C:prop name=""EXRULE""/>
//<C:prop name=""EXDATE""/>
//<C:prop name=""RECURRENCE-ID""/>
//</C:comp>
//<C:comp name=""VTIMEZONE""/>
//</C:comp>
//</C:calendar-data>
//</D:prop>
//<C:filter>
//<C:comp-filter name=""VCALENDAR"">
//<C:comp-filter name=""VEVENT"">
//<C:time-range start=""20060104T000000Z""
//end=""20060105T000000Z""/>
//</C:comp-filter>
//</C:comp-filter>
//</C:filter>
//</C:calendar-query>";
//            var xmlTreeStructure = XmlTreeStructure.Parse(doc);
//            var xmlTreeStructure2 = XmlTreeStructure.Parse(xmlTreeStructure.ToString());
//            var xmlStr1 = xmlTreeStructure.ToString();
//            var xmlStr2 = xmlTreeStructure2.ToString();

//            Assert.Equal(xmlStr1, xmlStr2);
//        }
//    }
//}