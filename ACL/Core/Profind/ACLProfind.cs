using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ACL.Core.Authentication;
using DataLayer;
using DataLayer.Models.ACL;
using Microsoft.AspNet.Http;
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
        public ACLProfind(IAuthenticate authenticate)
        {
            _authenticate = authenticate;
        }

        /// <summary>
        /// Call the method to perform a PROFIND over a 
        /// principal.
        /// Initially the client could do a PROFIND over
        /// the server to discover all the user calendars
        /// or could PORFIND directly over a calendar URL. 
        /// </summary> 
        /// <param name="request">THe HttpRequest from the controller.</param>
        /// <param name="body">The request's body</param>
        /// <param name="response">The HttpResponse property from the controller.</param>
        /// <returns>The request</returns>
        public async Task Profind(HttpRequest request, HttpResponse response)
        {
            var requestPath = request.Path;

            //read the body of the request
            var bodyString = new StreamReader(request.Body).ReadToEnd();
            var context = new CalDavContext();

            //authenticate the user if exit if not create it in the system
            var principal = await _authenticate.AuthenticateRequest(request, response);


            IXMLTreeStructure body = XmlTreeStructure.Parse(bodyString);

            //take the requested properties
            var reqProperties = ExtractPropertiesNameMainNS(body);

            await BuildResponse(response, requestPath, reqProperties, principal);
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
            var multistatus = new XmlTreeStructure("multistatus", "DAV:");
            multistatus.Namespaces.Add("D", "DAV:");
            multistatus.Namespaces.Add("C", "urn:ietf:params:xml:ns:caldav");

            //create the response node.
            var responseNode = new XmlTreeStructure("response", "DAV:");

            //create the href node
            var hrefNode = new XmlTreeStructure("href", "DAV:");
            hrefNode.AddValue(requestedUrl);

            responseNode.AddChild(hrefNode);

            //in this section is where the "propstat" structure its build.
            var propstatNode = new XmlTreeStructure("propstat", "DAV:");

            var propNode = new XmlTreeStructure("prop", "DAV:");

            //add the requested properties to the propNode 
            //if the properties exist in the principal
            var properties = principal.Properties
                .Where(p => reqProperties.Contains(new KeyValuePair<string, string>(p.Namespace, p.Name)))
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

            multistatus.AddChild(responseNode);

            //here the multistatus xml for the body is built
            //have to write it to the response body.

            await response.WriteAsync(multistatus.ToString());
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
