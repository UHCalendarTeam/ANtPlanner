using ACL.Core.Authentication;
using DataLayer;
using DataLayer.Models.ACL;
using Microsoft.AspNet.Http;
using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using TreeForXml;

namespace ACL.Core
{
    /// <summary>
    /// Contains the PROFIND method for
    /// the principals
    /// </summary>
    public class ACLProfind : IACLProfind
    {
        private IAuthenticate _authenticate;
        private CalDavContext _context;

        public ACLProfind(IAuthenticate authenticate, CalDavContext context)
        {
            _authenticate = authenticate;
            _context = context;
        }

        /// <summary>
        /// Call the method to perform a PROFIND over a
        /// principal.
        /// Initially the client could do a PROFIND over
        /// the server to discover all the user calendars
        /// or could PORFIND directly over a calendar URL.
        /// </summary>
        /// <param name="request">THe HttpRequest from the controller.</param>
        /// <param name="response">The HttpResponse property from the controller.</param>
        /// <param name="data"></param>
        /// <param name="body">The request's body</param>
        /// <returns>The request</returns>
        public async Task Profind(HttpContext httpContext)
        {
            var requestPath = httpContext.Request.Path;
            var streamReader = new StreamReader(httpContext.Request.Body);
            //read the body of the request
            var bodyString = streamReader.ReadToEnd();

            Principal principal;

            //if from the controller comes the principal data, means that the user
            //exist in the system, so take it
            //the principalId comes in the Session.
            if (httpContext.Session.Keys.Any(key=> key == "principalId"))
            {
                principal =
                    _context.Principals.Include(p => p.Properties)
                        .FirstOrDefault(p => p.PrincipalStringIdentifier == httpContext.Session.GetString("principalId"));
                //TODO: check the user's credentials
            }

            //authenticate the user if exist, if not create it in the system
            else
                principal = await _authenticate.AuthenticateRequest(httpContext);

            IXMLTreeStructure body = XmlTreeStructure.Parse(bodyString);

            //take the requested properties
            var reqProperties = ExtractPropertiesNameMainNS(body);

            await BuildResponse(httpContext.Response, requestPath, reqProperties, principal);
        }

        /// <summary>
        /// Having the principal and the requested properties
        /// then proccess the requestedProperties and build the
        /// response.
        /// </summary>
        /// <param name="response">The HttpResponse from the controller.</param>
        /// <param name="requestedUrl">The principal url if anyurl </param>
        /// <param name="reqUrlreqProperties">The requested properties.</param>
        /// <param name="principal">The instance of the pricipal that is requested</param>
        /// <returns>The final response to return. Has the body with the response and the</returns>
        public async Task BuildResponse(HttpResponse response, string requestedUrl,
            List<KeyValuePair<string, string>> reqProperties, Principal principal)
        {
            var multistatusNode = new XmlTreeStructure("multistatus", "DAV:");
            multistatusNode.Namespaces.Add("D", "DAV:");
            multistatusNode.Namespaces.Add("C", "urn:ietf:params:xml:ns:caldav");

            //create the response node.
            var responseNode = new XmlTreeStructure("response", "DAV:");

            //create the href node
            var hrefNode = new XmlTreeStructure("href", "DAV:");
            string url = requestedUrl.Replace("/api/v1/caldav", "");
            hrefNode.AddValue(url);

            responseNode.AddChild(hrefNode);

            //in this section is where the "propstat" structure its build.
            var propstatNode = new XmlTreeStructure("propstat", "DAV:");

            var propNode = new XmlTreeStructure("prop", "DAV:");

            //add the requested properties to the propNode
            //if the properties exist in the principal
            var properties = principal.Properties
                .Where(p => reqProperties.Contains(new KeyValuePair<string, string>(p.Name, p.Namespace)))
                .Select(x => XmlTreeStructure.Parse(x.Value));
            //add the properties to the propNode
            foreach (var property in properties)
            {
                propNode.AddChild(property);
            }

            var statusNode = new XmlTreeStructure("status", "DAV:")
            {
                Value = "HTTP/1.1 200 OK"
            };

            ///add the propNOde and the status node to the propStatNode
            propstatNode.AddChild(propNode).AddChild(statusNode);

            responseNode.AddChild(propstatNode);

            multistatusNode.AddChild(responseNode);

            //here the multistatus xml for the body is built
            //have to write it to the response body.

            string multiStatus = multistatusNode.ToString();

            await response.WriteAsync(multiStatus);
        }

        /// <summary>
        ///     Extract all property names and property namespace from a prop element of a  propfind body.
        /// </summary>
        /// <param name="propFindTree"></param>
        /// <returns></returns>
        private List<KeyValuePair<string, string>> ExtractPropertiesNameMainNS(IXMLTreeStructure propFindTree)
        {
            var retList = new List<KeyValuePair<string, string>>();
            IXMLTreeStructure props;

            if (propFindTree.GetChildAtAnyLevel("prop", out props))
                retList.AddRange(
                    props.Children.Select(child => new KeyValuePair<string, string>(child.NodeName, string.IsNullOrEmpty(child.MainNamespace) ? "DAV:" : child.MainNamespace)));
            return retList;
        }
    }
}