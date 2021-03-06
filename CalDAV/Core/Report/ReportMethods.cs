﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalDAV.Core.ConditionsCheck.Preconditions.Report;
using CalDAV.Core.Method_Extensions;
using DataLayer;
using DataLayer.Models.Entities;
using DataLayer.Models.Entities.ResourcesAndCollections;
using DataLayer.Models.Interfaces.Repositories;
using DataLayer.Models.Repositories;
using ICalendar.Calendar;
using Microsoft.AspNetCore.Http;
using TreeForXml;

namespace CalDAV.Core
{
    /// <summary>
    ///     THis class contain the logic for processing a
    ///     REPORT Request.
    /// </summary>
    public class CollectionReport : ICollectionReport
    {
        public readonly ICalendarResourceRepository _resourceRepository;
        public readonly IReportPreconditions _preconditions;

        public CollectionReport(ICalendarResourceRepository resRepository, IReportPreconditions preconditions)
        {
            _resourceRepository = resRepository;
            _preconditions = preconditions;
        }

        public string ExpandProperty()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Process the report depending on the values of the header
        ///     and the body.
        /// </summary>
        /// <returns></returns>
        public async Task ProcessRequest(HttpContext httpContext)
        {
            //check the preconditions for the HTTP REPORT method
            if(!_preconditions.PreconditionProcessor(httpContext))
                return;

            var body = new StreamReader(httpContext.Request.Body).ReadToEnd();

            // var node = xmlBody.Children.First();
            var xmlBody = XmlTreeStructure.Parse(body);

            //take the target url that is the identifier of the collection
            var urlId = httpContext.Request.GetRealUrl();

            //take the first node of the xml and process the request
            //by the name of the first node
            switch (xmlBody.NodeName)
            {
                case "calendar-query":
                    await CalendarQuery(xmlBody, urlId, httpContext);
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
        /// <param name="collectionURl"></param>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public async Task CalendarQuery(IXMLTreeStructure xmlDoc, string collectionURl, HttpContext httpContext)
        {
            IFileManagement fs = new FileManagement();
            // take the first prop node to know the data that
            // should ne returned
            IXMLTreeStructure propNode;
            xmlDoc.GetChildAtAnyLevel("prop", out propNode);

            //get the filters to be applied
            IXMLTreeStructure componentFilter;
            xmlDoc.GetChildAtAnyLevel("filter", out componentFilter);


            var userResources = new Dictionary<string, string>();

            ///TODO: have to add the DTSTART and DTEND of the resource in the DB
            ///generally the first time that a client syncronize with the system
            /// the client send a REPORT resquest with a time-filter query. So and 
            /// optimization would be to know if the request has a time-filter query
            /// run the query in db in just load in memory the resources that pass the filter.
            /// Right now we are loading all the resources i memory and running the filters
            /// which may impact the memory consumption depending on how many resources the user has.
            await fs.GetAllCalendarObjectResource(collectionURl, userResources);
            var userCalendars = userResources.ToDictionary(userResource => userResource.Key,
                userResource => VCalendar.Parse(userResource.Value));

            //apply the filters to the calendars
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
        /// <param name="httpContext"></param>
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

                //each returned resource has is own response and nodes
                var responseNode = new XmlTreeStructure("response", "DAV:");
                var hrefNode = new XmlTreeStructure("href", "DAV:");
                var href = resource.Key[0] != '/' ? "/" + resource.Key : resource.Key;
                hrefNode.AddValue(href);

                //href is a child pf response
                responseNode.AddChild(hrefNode);

                //if the resource is null it was not foound so
                // add an error status
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
                    var propStats = await ProccessPropNode(calDataNode, resource);


                    foreach (var propStat in propStats)
                    {
                        responseNode.AddChild(propStat);
                    }
                }

                multistatusNode.AddChild(responseNode);
            }
            var responseText = multistatusNode.ToString();
            var responseBytes = Encoding.UTF8.GetBytes(responseText);
            httpContext.Response.ContentLength = responseBytes.Length;
            await httpContext.Response.Body.WriteAsync(responseBytes, 0, responseBytes.Length);
        }


        /// <summary>
        ///     The CALDAV:calendar-multiget REPORT is used to retrieve specific calendar object resources from within a
        ///     collection, if the Request-URI is a collection, or to retrieve a specific calendar object resource, if the
        ///     Request-URI is a calendar object resource. This report is similar to the CALDAV:calendar-query REPORT
        ///     (see Section 7.8), except that it takes a list of DAV:href elements, instead of a CALDAV:filter element, to
        ///     determine which calendar object resources to return
        /// </summary>
        /// <returns></returns>
        public async Task CalendarMultiget(IXMLTreeStructure xmlBody, HttpContext httpContext)
        {
            // take the first prop node to know the data that
            // should ne returned
            IXMLTreeStructure propNode;
            xmlBody.GetChildAtAnyLevel("prop", out propNode);

            //take the href nodes. Contain the direction of the resources files that
            //are requested
            var hrefs = xmlBody.Children.Where(node => node.NodeName == "href").Select(href => href.Value);

            var result = new Dictionary<string, string>();

            // process the requested resources taking each
            // from the file system
            foreach (var href in hrefs)
            {
                var fs = new FileManagement();

                var resourceContent = await fs.GetCalendarObjectResource(href);

                result.Add(href, resourceContent);
            }
            //build the response and send it to the client.
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
        private async Task<List<IXMLTreeStructure>> ProccessPropNode(IXMLTreeStructure incomPropNode,
            KeyValuePair<string, VCalendar> resource)
        {
            var output = new List<IXMLTreeStructure>();

            var resPropertiesOk = new List<XmlTreeStructure>();
            var resPropertiesNotExist = new List<XmlTreeStructure>();

            var href = resource.Key[0] != '/' ? "/" + resource.Key : resource.Key;
            var calResource = _resourceRepository.Find(href);

            foreach (var prop in incomPropNode.Children)
            {
                //create an instance of a XMlTreeStrucure with the same name and
                //ns that the requested
                var currentPropNode = new XmlTreeStructure(prop.NodeName, prop.MainNamespace);
                switch (prop.NodeName)
                {
                    //if the requested prop is calendar data then take the content of the
                    //resource
                    case "calendar-data":
                        //see if the calendar-data describes pros to take
                        // if does then take them if not take it all
                        currentPropNode.AddValue(prop.Children.Any()
                            ? resource.Value.ToString(prop)
                            : resource.Value.ToString());
                        resPropertiesOk.Add(currentPropNode);
                        break;
                    //if not try to take the property from the resource's properties
                    default:
                        Property currentProperty;
                        try
                        {
                            currentProperty = calResource.Properties.FirstOrDefault(p => p.Name == prop.NodeName);
                            currentPropNode.Value = currentProperty != null ? currentProperty.PropertyRealValue() : "";
                        }
                        catch
                        {
                            currentProperty = null;
                        }
                        if (currentProperty != null)
                            resPropertiesOk.Add(currentPropNode);
                        else
                            resPropertiesNotExist.Add(currentPropNode);
                        break;
                }
            }

            #region Adding nested propOK

            //This procedure has been explained in another method.
            //Here the retrieve properties are grouped.

            var propstatOK = new XmlTreeStructure("propstat", "DAV:");
            var propOk = new XmlTreeStructure("prop", "DAV:");

            //Here i add all properties to the prop. 
            foreach (var property in resPropertiesOk)
            {
                propOk.AddChild(property);
            }

            propstatOK.AddChild(propOk);

            #endregion

            #region Adding nested status OK

            var statusOK = new XmlTreeStructure("status", "DAV:");
            statusOK.AddValue("HTTP/1.1 200 OK");
            propstatOK.AddChild(statusOK);

            #endregion

            #region Adding nested propWrong

            //Here the properties that could not be retrieved are grouped.
            var propstatWrong = new XmlTreeStructure("propstat", "DAV:");
            var propWrong = new XmlTreeStructure("prop", "DAV:");

            //Here i add all properties to the prop. 
            foreach (var property in resPropertiesNotExist)
            {
                propWrong.AddChild(property);
            }

            propstatWrong.AddChild(propWrong);

            #endregion

            #region Adding nested status Not Found

            var statusWrong = new XmlTreeStructure("status", "DAV:");
            statusWrong.AddValue("HTTP/1.1 400 Not Found");
            propstatWrong.AddChild(statusWrong);

            #endregion

            #region Adding responseDescription when wrong

            //Here i add an description for explain the errors.
            //This should be aplied in all method with an similar structure but for the moment is only used here.
            //However this is not required. 
            var responseDescrpWrong = new XmlTreeStructure("responsedescription", "DAV:");
            responseDescrpWrong.AddValue("The properties doesn't  exist");
            propstatWrong.AddChild(responseDescrpWrong);

            #endregion

            if (resPropertiesOk.Any())
                output.Add(propstatOK);
            if (resPropertiesNotExist.Any())
                output.Add(propstatWrong);

            return output;
        }
    }
}