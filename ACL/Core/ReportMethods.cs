using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using ACL.Interfaces;
using DataLayer;
using Microsoft.AspNet.Http;
using TreeForXml;
using DataLayer.Entities;
using DataLayer.Models.ACL;

namespace ACL.Core
{
    public class ACLReport:IReportMethods
    {
        public string ProcessRequest(HttpRequest request, CalDavContext context)
        {
            //take the string representation of the body
            string bodyStr = request.Body.ToString();
            var xmlbody = XmlTreeStructure.Parse(bodyStr);

            switch (xmlbody.NodeName)
            {
                case "acl-principal-prop-set":
                    return AclPrincipalPropSet(xmlbody, request, context);
                case "principal-match":
                    return PrincipalMatch(xmlbody, request, context);
                case "principal-property-search":
                    return PrincipalPropertySearch(xmlbody, request, context);
                case "principal-search-property-set":
                    return PrincipalSearchPropertySet();
            }
            return "";

        }

        public string AclPrincipalPropSet(IXMLTreeStructure body, HttpRequest request, CalDavContext context)
        {
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

            List<Principal> principals = new List<Principal>();

            ///take all the principals with its url equal to the givens
            foreach (var pUrl in principalsURLs)
            {
                var principal = context.Principals.FirstOrDefault(principal1 => principal1.PrincipalURL == pUrl.Value);
                if(principal!=null)
                    principals.Add(principal);
            }





        }

        public string PrincipalMatch(IXMLTreeStructure body, HttpRequest request, CalDavContext context)
        {
            throw new NotImplementedException();
        }

        public string PrincipalPropertySearch(IXMLTreeStructure body, HttpRequest request, CalDavContext context)
        {
            throw new NotImplementedException();
        }

        public string PrincipalSearchPropertySet()
        {
            throw new NotImplementedException();
        }

     
    }
}
