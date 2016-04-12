using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ACL.Interfaces;
using DataLayer;
using Microsoft.AspNet.Http;
using TreeForXml;
using DataLayer.Entities;

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
            context.CalendarResources.FirstOrDefault(x => x.Href == colUrl);
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
