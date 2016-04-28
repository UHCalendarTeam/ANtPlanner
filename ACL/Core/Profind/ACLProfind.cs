using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACL.Core.Authentication;
using DataLayer;
using DataLayer.Models.ACL;
using Microsoft.AspNet.Http;
using TreeForXml;

namespace ACL.Core
{
    /// <summary>
    ///     Contains the PROFIND method for
    ///     the principals
    /// </summary>
    public class ACLProfind : IACLProfind
    {
        private readonly IAuthenticate _authenticate;
        private CalDavContext _context;

        public ACLProfind(IAuthenticate authenticate, CalDavContext context)
        {
            _authenticate = authenticate;
            _context = context;
        }

        /// <summary>
        ///     Call the method to perform a PROFIND over a
        ///     principal.
        ///     Initially the client could do a PROFIND over
        ///     the server to discover all the user calendars
        ///     or could PORFIND directly over a calendar URL.
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

            //try to authenticate the request either with the cookies or the user credentials
            principal = await _authenticate.AuthenticateRequest(httpContext);

            //if the principal is null then there is some problem with the authentication
            //so return
            if (principal == null)
                return;

            var body = XmlTreeStructure.Parse(bodyString);

            //take the requested properties
            var reqProperties = ExtractPropertiesNameMainNS(body);

            await BuildResponse(httpContext.Response, requestPath, reqProperties, principal);
        }

        /// <summary>
        ///     Having the principal and the requested properties
        ///     then proccess the requestedProperties and build the
        ///     response.
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
            var url = requestedUrl.Replace("/api/v1/caldav", "");
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

            var multiStatus = multistatusNode.ToString();
            var responseText = multistatusNode.ToString();
            byte[] responseBytes = Encoding.UTF8.GetBytes(responseText);
            response.ContentLength = responseBytes.Length;
            await response.Body.WriteAsync(responseBytes, 0, responseBytes.Length);
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
                    props.Children.Select(
                        child =>
                            new KeyValuePair<string, string>(child.NodeName,
                                string.IsNullOrEmpty(child.MainNamespace) ? "DAV:" : child.MainNamespace)));
            return retList;
        }
    }
}