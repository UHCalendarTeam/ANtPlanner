using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CalDAV.Core.Method_Extensions;
using DataLayer;
using ICalendar.Calendar;
using Microsoft.AspNet.Http;
using Microsoft.Data.Entity;
using TreeForXml;

namespace CalDAV.Core
{
    /// <summary>
    ///     THis class contain the logic for processing a
    ///     REPORT Request.
    /// </summary>
    public class CollectionReport : ICollectionReport
    {
        private CalDavContext _context;

        public CollectionReport(CalDavContext context)
        {
            _context = context;
        }

        private string UserEmail { get; set; }
        private string CollectionName { get; set; }


        public string ExpandProperty()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Process the report depending on the values of the header 
        /// and the body.
        /// </summary>
        /// <param name="headerValues">The neccessary properties and values of the header</param>
        /// <param name="body">The string representation of the body</param>
        /// <returns></returns>
        public async Task ProcessRequest(HttpContext httpContext)
        {
            var body = new StreamReader(httpContext.Request.Body).ReadToEnd();

            // var node = xmlBody.Children.First();
            var xmlBody = XmlTreeStructure.Parse(body);

            ///take the target url that is the identifier of the collection
            var urlId = httpContext.Request.GetRealUrl();

            //take the first node of the xml and process the request
            //by the name of the first node
            switch (xmlBody.NodeName)
            {
                case "calendar-query":
                    await CalendarQuery(xmlBody,  urlId, httpContext);
                    break;
                case "calendar-multiget":
                    await CalendarMultiget(xmlBody, httpContext);
                    break;
                default:
                    throw new NotImplementedException(
                        $"The REPORT request {xmlBody.NodeName} with ns equal to {xmlBody.MainNamespace} is not implemented yet .");
            }
        }


        /// <summary>
        ///     The CALDAV:calendar-query REPORT performs a search for all calendar object resources that match a
        ///     specified filter. The response of this report will contain all the WebDAV properties and calendar object
        ///     resource data specified in the request. In the case of the CALDAV:calendar-data XML element, one can
        ///     explicitly specify the calendar components and properties that should be returned in the calendar object
        ///     resource data that matches the filter.
        /// </summary>
        /// <param name="xmlDoc">The body of the request.</param>
        /// <param name="fs">The FileManagementSystem instance that points to the requested collection.</param>
        /// <returns></returns>
        public async Task CalendarQuery(IXMLTreeStructure xmlDoc,  string collectionURl, HttpContext httpContext)
        {
            IFileSystemManagement fs = new FileSystemManagement();
            /// take the first prop node to know the data that
            /// should ne returned
            IXMLTreeStructure propNode;
            xmlDoc.GetChildAtAnyLevel("prop", out propNode);

            ///get the filters to be applied
            IXMLTreeStructure componentFilter;
            xmlDoc.GetChildAtAnyLevel("filter", out componentFilter);


            Dictionary<string, string> userResources;
          
            fs.GetAllCalendarObjectResource(collectionURl, out userResources);
            var userCalendars = userResources.ToDictionary(userResource => userResource.Key,
                userResource => VCalendar.Parse(userResource.Value));

            ///apply the filters to the calendars
            var filteredCalendars = userCalendars.Where(x => x.Value.FilterResource(componentFilter));
            await ReportResponseBuilder(filteredCalendars, propNode, httpContext);
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
        public async Task ReportResponseBuilder(IEnumerable<KeyValuePair<string, VCalendar>> resources,
            IXMLTreeStructure calDataNode, HttpContext httpContext)
        {
            var multistatusNode = new XmlTreeStructure("multistatus", "DAV:")
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
                IXMLTreeStructure statusNode;

                ///each returned resource has is own response and 
                /// 
                ///  nodes
                var responseNode = new XmlTreeStructure("response", "DAV:");
                var hrefNode = new XmlTreeStructure("href", "DAV:");
                var href = resource.Key[0] != '/' ? "/" + resource.Key : resource.Key;
                hrefNode.AddValue(href);

                ///href is a child pf response
                responseNode.AddChild(hrefNode);

                ///if the resource is null it was not foound so
                /// add an error status
                if (resource.Value == null)
                {
                    statusNode = new XmlTreeStructure("status", "DAV:");
                    statusNode.AddValue("HTTP/1.1 404 Not Found");
                    responseNode.AddChild(statusNode);
                }
                else
                {
                    var propstatNode = new XmlTreeStructure("propstat", "DAV:");

                    //that the requested data
                    var propNode = ProccessPropNode(calDataNode, resource);

                    propstatNode.AddChild(propNode);

                    ///adding the status node
                    /// TODO: check the status!!
                    statusNode = new XmlTreeStructure("status", "DAV:");
                    statusNode.AddValue("HTTP/1.1 200 OK");

                    propstatNode.AddChild(statusNode);

                    responseNode.AddChild(propstatNode);
                }

                multistatusNode.AddChild(responseNode);
            }
            var responseText = multistatusNode.ToString();
            byte[] responseBytes = Encoding.UTF8.GetBytes(responseText);
            httpContext.Response.ContentLength = responseBytes.Length;
            await httpContext.Response.Body.WriteAsync(responseBytes, 0 , responseBytes.Length);
        }


        /// <summary>
        ///     The CALDAV:calendar-multiget REPORT is used to retrieve specific calendar object resources from within a
        ///     collection, if the Request-URI is a collection, or to retrieve a specific calendar object resource, if the
        ///     Request-URI is a calendar object resource. This report is similar to the CALDAV:calendar-query REPORT
        ///     (see Section 7.8), except that it takes a list of DAV:href elements, instead of a CALDAV:filter element, to
        ///     determine which calendar object resources to return
        /// </summary>
        /// <param name="xmlDoc">The body of the request.</param>
        /// <param name="storageManagement">The FileManagementSystem instance that points to the requested collection.</param>
        /// <returns></returns>
        private async Task CalendarMultiget(IXMLTreeStructure xmlBody, HttpContext httpContext)
        {
            /// take the first prop node to know the data that
            /// should ne returned
            IXMLTreeStructure propNode;
            xmlBody.GetChildAtAnyLevel("prop", out propNode);

            //take the href nodes. Contain the direction of the resources files that
            //are requested
            var hrefs = xmlBody.Children.Where(node => node.NodeName == "href").Select(href => href.Value);

            var result = new Dictionary<string, string>();

            /// process the requested resources
            foreach (var href in hrefs)
            {
                var fs = new FileSystemManagement();

                var resourceContent = fs.GetCalendarObjectResource(href).Result;

                result.Add(href, resourceContent);
            }
            await ReportResponseBuilder(result
                .Select(
                    x =>
                        new KeyValuePair<string, VCalendar>(x.Key,
                            string.IsNullOrEmpty(x.Value) ? null : VCalendar.Parse(x.Value))), propNode, httpContext);
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
        private IXMLTreeStructure ProccessPropNode(IXMLTreeStructure incomPropNode, KeyValuePair<string,VCalendar> resource)
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
                        //take the getetag property from the target resource
                        var href = resource.Key[0] != '/'? "/" + resource.Key : resource.Key;
                        
                        var cal = _context.GetCalendarResource(href);
                        var etag = cal.Properties.FirstOrDefault(p => p.Name == "getetag" && p.Namespace == "DAV:");
                        currentProp.AddValue(etag.PropertyRealValue());
                        break;

                    case "calendar-data":
                        ///see if the calendar-data describes pros to take
                        /// if does then take them if not take it all
                        currentProp.AddValue(prop.Children.Any() ? resource.Value.ToString(prop) : resource.Value.ToString());
                        break;
                    default:
                        throw new NotImplementedException(
                            $"The requested property with name {prop.NodeName} is not implemented.");
                }
                outputRoot.AddChild(currentProp);
            }

            return outputRoot;
        }
    }
}