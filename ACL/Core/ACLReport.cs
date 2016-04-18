using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using ACL.Interfaces;
using DataLayer;
using Microsoft.AspNet.Http;
using TreeForXml;
using DataLayer.Models.Entities;
using DataLayer.Models.ACL;
using DataLayer.Models.Entities;

namespace ACL.Core
{
    public class ACLReport:IReportMethods
    {
        public void ProcessRequest(HttpRequest request, CalDavContext context, HttpResponse response)
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
            

            response = null;
            //take the string representation of the body
            string bodyStr = request.Body.ToString();
            var xmlbody = XmlTreeStructure.Parse(bodyStr);

            switch (xmlbody.NodeName)
            {
                case "acl-principal-prop-set":
                     AclPrincipalPropSet(xmlbody, request, context, response );
                    break;
                case "principal-match":
                     PrincipalMatch(xmlbody, request, context, response);
                    break;
                case "principal-property-search":
                     PrincipalPropertySearch(xmlbody, request, context, response);
                    break;
                case "principal-search-property-set":
                     PrincipalSearchPropertySet( response);
                    break;
            }
            

        }

        public void AclPrincipalPropSet(IXMLTreeStructure body, HttpRequest request, CalDavContext context, HttpResponse response)
        {
            response = null;

            ///take the requested properties from the body
            /// of the request
            IXMLTreeStructure propNode;

            ///first take the node container of the property names
            body.GetChildAtAnyLevel("prop",out propNode);

            ///take the children of the node, these are the proeprties
            var requestedProperties = propNode.Children.Select(x => 
            new KeyValuePair<string, string>( x.NodeName, x.MainNamespace));

            string colUrl = "";

            //Take the resource with the href == to the given url
            var resource = context.CalendarResources.FirstOrDefault(x => x.Href == colUrl);

            //take the accessControlProperties of the resource
            var accControlProp = resource.AccessControlProperties;

            //take the string representation of the acl property
            //this property is stored in xml format so is needed to
            //be parsed to xml
            var aclProperty = accControlProp.Acl;
            var xml = XDocument.Parse(aclProperty);

            ///take the href of the principals of the property
            var principalsURLs = xml.Elements("principal").Select(x => x.Descendants("href").FirstOrDefault());

            Dictionary<Principal,IEnumerable<Property>> principals = new Dictionary<Principal,IEnumerable<Property>>();

            ///take all the principals with its url equal to the givens
            foreach (var pUrl in principalsURLs)
            {
                var principal = context.Principals.FirstOrDefault(principal1 => principal1.PrincipalURL == pUrl.Value);
                if(principal!=null)
                    principals.Add(principal, null);
            }

            ///take the requested properties from the principals
            foreach (var principal in principals)
            {
               principals[principal.Key] = principal.Key.TakeProperties(requestedProperties);
            }


           


        }

        public void PrincipalMatch(IXMLTreeStructure body, HttpRequest request, CalDavContext context, HttpResponse response)
        {
            throw new NotImplementedException();
        }

        public void PrincipalPropertySearch(IXMLTreeStructure body, HttpRequest request, CalDavContext context, HttpResponse response)
        {
            throw new NotImplementedException();
        }

        public void PrincipalSearchPropertySet(HttpResponse response)
        {
            throw new NotImplementedException();
        }

        public async Task WriteBody(HttpResponse response,
            Dictionary<Principal, IEnumerable<Property>> principalsAndProperties)
        {

            ///build the root of the xml
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
                    foreach (var property in pp.Value)
                    {
                        propNode.AddChild()
                    }

                    propstatNode.AddChild(propNode);
                    ///adding the status node
                    /// TODO: check the status!!
                    statusNode = new XmlTreeStructure("status", "DAV:");
                    statusNode.AddValue("HTTP/1.1 200 OK");

                    propstatNode.AddChild(statusNode);

                    responseNode.AddChild(propstatNode);
                }

                mutistatusNode.AddChild(responseNode);
            }

        }


    }
}
