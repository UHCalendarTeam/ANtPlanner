using System;
using System.Collections.Generic;
using System.Linq;
using CalDAV.Core.Method_Extensions;
using ICalendar.Calendar;
using TreeForXml;

namespace CalDAV.Core
{
    /// <summary>
    ///     THis class contain the logic for processing a
    ///     REPORT Request.
    /// </summary>
    public class ReportMethods : IReportMethods
    {
        public ReportMethods(string userEmail, string collectionName)
        {
            UserEmail = userEmail;
            CollectionName = collectionName;
        }

        public ReportMethods()
        {
            
        }
        private string UserEmail { get; set; }
        private string CollectionName { get; set; }


        public string ExpandProperty()
        {
            throw new NotImplementedException();
        }

        public string ProcessRequest(IXMLTreeStructure xmlBody, IFileSystemManagement storageManagement)
        {
            //take the first node of the xml and 
            //process the request by it
           // var node = xmlBody.Children.First();

            switch (xmlBody.NodeName)
            {
                case "calendar-query":
                    return CalendarQuery(xmlBody, storageManagement);
                default:
                    throw new NotImplementedException($"The REPORT request {xmlBody.NodeName} with ns equal to {xmlBody.MainNamespace} is not implemented yet .");


            }
        }

        public string CalendarQuery(IXMLTreeStructure xmlDoc, IFileSystemManagement fs)
        {
            /// take the first prop node to know the data that
            /// should ne returned
            IXMLTreeStructure propNode;
            xmlDoc.GetChildAtAnyLevel("prop", out propNode);

            ///get the filters to be applied
            IXMLTreeStructure componentFilter;
            xmlDoc.GetChildAtAnyLevel("filter", out componentFilter);


            Dictionary<string, string> userResources;
            var fileM = new FileSystemManagement();
            fs.GetAllCalendarObjectResource(out userResources);
            var userCalendars = userResources.ToDictionary(userResource => userResource.Key,
                userResource => VCalendar.Parse(userResource.Value));

            ///apply the filters to the calendars
            var filteredCalendars = userCalendars.Where(x => x.Value.FilterResource(componentFilter));
            return ToXmlString(filteredCalendars, propNode);
            
        }

        /// <summary>
        ///     Take the calendar that passed the filter and
        ///     create the multi-status xml.
        /// </summary>
        /// <param name="resources">The resources to be returned</param>
        /// <param name="calDataNode">
        ///     THis is the node with name ="prop"
        ///     When used in a calendaring REPORT request, the CALDAV:calendar-data XML
        ///     element specifies which parts of calendar object resources need to be returned in the
        ///     response.If the CALDAV:calendar-data XML element doesn't contain any
        ///     CALDAV:comp element, calendar object resources will be returned in their entirety.
        /// </param>
        /// <returns>The string representation of the multi-status Xml with the results.</returns>
        public string ToXmlString(IEnumerable<KeyValuePair<string, VCalendar>> resources, IXMLTreeStructure calDataNode)
        {
            var mutistatusNode = new XmlTreeStructure("multi-status", "DAV:")
            {
                Namespaces = new Dictionary<string, string>
                {
                    {"D", "DAV:"},
                    {"C", "urn:ietf:params:xml:ns:caldav"}
                }
            };

            //take the node that specified the comp and properties
            //to return
            

            foreach (var resource in resources)
            {
                ///each returned resource has is own response and href nodes
                var responseNode = new XmlTreeStructure("response", "DAV:");
                var hrefNode = new XmlTreeStructure("href", "DAV:");
                hrefNode.AddValue(resource.Key);

                ///href is a child pf response
                responseNode.AddChild(hrefNode);

                var propstatNode = new XmlTreeStructure("propstat", "DAV:");

                //that the requested data
                var propNode = ProccessPropNode(calDataNode, resource.Value);

                propstatNode.AddChild(propNode);

                ///adding the status node
                /// TODO: check the status!!
                var statusNode = new XmlTreeStructure("status", "DAV:");
                statusNode.AddValue("HTTP/1.1 200 OK");

                propstatNode.AddChild(statusNode);

                responseNode.AddChild(propstatNode);

                mutistatusNode.AddChild(responseNode);
            }
            return mutistatusNode.ToString();
        }

        /// <summary>
        ///     Take the prop node that specified the properties and
        ///     component that are requested, extract this data from
        ///     the system and the VCalendar and return the container
        ///     node with this data.
        /// </summary>
        /// <param name="incomPropNode">
        ///     This node contains the requested data. Is the first prop node
        ///     of the calendar-query.
        /// </param>
        /// <param name="resource">The calendar where to extract the data.</param>
        /// <returns>Return the prop node that contains the requested data</returns>
        private IXMLTreeStructure ProccessPropNode(IXMLTreeStructure incomPropNode, VCalendar resource)
        {
            var outputRoot = new XmlTreeStructure("prop", "DAV:");

            foreach (var prop in incomPropNode.Children)
            {
                //create an instance of a XMlTreeStrucure with the same name and
                //ns that the requested
                var currentProp = new XmlTreeStructure(prop.NodeName, prop.MainNamespace);
                switch (prop.NodeName)
                {
                    case "getetag":
                        //TODO: take the real eTag of the resource in here
                        currentProp.AddValue("Put the real eTag value here");
                        break;

                    case "calendar-data":
                        ///see if the calendar-data describes pros to take
                        /// if does then take them if not take it all
                        currentProp.AddValue(prop.Children.Any() ? resource.ToString(prop) : resource.ToString());
                        break;
                }
                outputRoot.AddChild(currentProp);
            }

            return outputRoot;
        }
    }
}