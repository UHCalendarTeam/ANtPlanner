using ACL.Interfaces;
using DataLayer;
using DataLayer.Models.ACL;
using DataLayer.Models.Entities;
using Microsoft.AspNet.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using TreeForXml;

namespace ACL.Core
{
    public class ACLReport : IReportMethods
    {
        public async Task ProcessRequest(HttpRequest request, CalDavContext context, HttpResponse response)
        {
            ///check the depth of the header
            /// This report is only defined when the Depth header has value "0";
            /// other values result in a 400 (Bad Request) error response.
            if (request.Headers.ContainsKey("Depth"))
            {
                var depth = request.Headers["Depth"];
                if (depth != "\"0\"")
                {
                    response.StatusCode = 400;
                    return;
                }
            }

            string href = request.Path;
            //TODO: take here the email of the user by calling
            //to the authentication api
            string userEmail = "";

            response = null;
            //take the string representation of the body
            string bodyStr = request.Body.ToString();
            var xmlbody = XmlTreeStructure.Parse(bodyStr);

            switch (xmlbody.NodeName)
            {
                case "acl-principal-prop-set":
                    await AclPrincipalPropSet(xmlbody, context, response);
                    break;

                case "principal-match":
                    await PrincipalMatch(xmlbody, userEmail, href, context, response);
                    break;

                case "principal-property-search":
                    await PrincipalPropertySearch(xmlbody, request, context, response);
                    break;

                case "principal-search-property-set":
                    await PrincipalSearchPropertySet(response);
                    break;
            }
        }

        public async Task AclPrincipalPropSet(IXMLTreeStructure body, CalDavContext context, HttpResponse response)
        {
            ///take the requested properties from the body
            /// of the request
            IXMLTreeStructure propNode;

            ///first take the node container of the property names
            body.GetChildAtAnyLevel("prop", out propNode);

            ///take the children of the node, these are the proeprties
            var requestedProperties = propNode.Children.Select(x =>
            new KeyValuePair<string, string>(x.NodeName, x.MainNamespace));

            string colUrl = "";

            //Take the resource with the href == to the given url
            //TODO: should the href property be store in a property?
            var resource = context.CalendarResources.FirstOrDefault(x => x.Href == colUrl);

            //take the string representation of the acl property
            //this property is stored in xml format so is needed to
            //be parsed to xml
            var aclProperty = resource.Properties.First(x => x.Name == "acl");
            var aclXmlProperty = XDocument.Parse(aclProperty.Value);

            ///take the href of the principals of the property
            var principalsURLs = aclXmlProperty.Elements("principal").Select(x => x.Descendants("href").FirstOrDefault());

            Dictionary<Principal, IEnumerable<Property>> principals = new Dictionary<Principal, IEnumerable<Property>>();

            ///take all the principals with its url equal to the givens
            foreach (var pUrl in principalsURLs)
            {
                var principal = context.Principals.FirstOrDefault(principal1 => principal1.PrincipalURL == pUrl.Value);
                if (principal != null)
                    principals.Add(principal, null);
            }

            ///take the requested properties from the principals
            foreach (var principal in principals)
            {
                principals[principal.Key] = principal.Key.TakeProperties(requestedProperties);
            }

            await WriteBody(response, principals);
        }

        /// <summary>
        ///     The DAV:principal-match REPORT is used to identify all members (at
        ///     any depth) of the collection identified by the Request-URI that are
        ///     principals and that match the current user.
        ///     So it takes the resources of the collection and see wich ones match
        ///     the given principal. This is done comparing the principal's email
        ///     of the resource with the email of the given principal.
        /// </summary>
        /// <returns></returns>
        public async Task PrincipalMatch(IXMLTreeStructure body, string principalEmail, string href, CalDavContext context, HttpResponse response)
        {
            ///take the collection with the given href
            var col = context.CalendarCollections.FirstOrDefault(x => x.Url == href);

            //if the collection doesnt exit then return an error
            if (col == null)
            {
                await ReturnError(response, "Not Found", 404, href);
            }

            ///take all the resources from the collection.
            //var colResources = col.CalendarResources.Where(x=>x.)
        }

        public async Task PrincipalPropertySearch(IXMLTreeStructure body, HttpRequest request, CalDavContext context, HttpResponse response)
        {
            throw new NotImplementedException();
        }

        public async Task PrincipalSearchPropertySet(HttpResponse response)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Build the xml of the body and write
        /// its string representation to the HttpRespose.Body
        /// </summary>
        /// <param name="response">The response of the request.</param>
        /// <param name="principalsAndProperties">The principals with its properties.</param>
        /// <returns></returns>
        public async Task WriteBody(HttpResponse response,
            Dictionary<Principal, IEnumerable<Property>> principalsAndProperties)
        {
            ///build the root of the xml
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

            foreach (var pp in principalsAndProperties)
            {
                IXMLTreeStructure statusNode;

                ///each returned resource has is own response and href nodes
                var responseNode = new XmlTreeStructure("response", "DAV:");
                var hrefNode = new XmlTreeStructure("href", "DAV:");
                hrefNode.AddValue(pp.Key.PrincipalURL);

                ///href is a child pf response
                responseNode.AddChild(hrefNode);

                ///if the resource is null it was not foound so
                /// add an error status
                if (pp.Value == null)
                {
                    statusNode = new XmlTreeStructure("status", "DAV:");
                    statusNode.AddValue("HTTP/1.1 404 Not Found");
                    responseNode.AddChild(statusNode);
                }
                else
                {
                    var propstatNode = new XmlTreeStructure("propstat", "DAV:");
                    var propNode = new XmlTreeStructure("prop", "DAV:");
                    ///add the properties to the prop node.
                    foreach (var property in pp.Value)
                    {
                        propNode.AddChild(XmlTreeStructure.Parse(property.Value));
                    }

                    propstatNode.AddChild(propNode);
                    ///adding the status node
                    /// TODO: check the status!!
                    statusNode = new XmlTreeStructure("status", "DAV:");
                    statusNode.AddValue("HTTP/1.1 200 OK");

                    propstatNode.AddChild(statusNode);

                    responseNode.AddChild(propstatNode);
                }

                multistatusNode.AddChild(responseNode);
                await response.WriteAsync(multistatusNode.ToString());
            }
        }

        /// <summary>
        /// Used to build a response with an error
        /// </summary>
        /// <param name="response">The response that comes from the controller</param>
        /// <param name="errorMessage">The message to put in the error.</param>
        /// <param name="errorCode">The code of the error.</param>
        /// <param name="href">The requested href.</param>
        /// <returns></returns>
        private async Task ReturnError(HttpResponse response, string errorMessage, int errorCode, string href)
        {
            ///build the root of the xml
            var multistatusNode = new XmlTreeStructure("multistatus", "DAV:")
            {
                Namespaces = new Dictionary<string, string>
                {
                    {"D", "DAV:"},
                    {"C", "urn:ietf:params:xml:ns:caldav"}
                }
            };

            IXMLTreeStructure statusNode;

            ///each returned resource has is own response and href nodes
            var responseNode = new XmlTreeStructure("response", "DAV:");
            var hrefNode = new XmlTreeStructure("href", "DAV:");
            hrefNode.AddValue(href);

            ///href is a child pf response
            responseNode.AddChild(hrefNode);

            statusNode = new XmlTreeStructure("status", "DAV:");
            statusNode.AddValue($"HTTP/1.1 {errorCode} {errorMessage}");
            responseNode.AddChild(statusNode);

            multistatusNode.AddChild(responseNode);

            await response.WriteAsync(multistatusNode.ToString());
        }
    }
}