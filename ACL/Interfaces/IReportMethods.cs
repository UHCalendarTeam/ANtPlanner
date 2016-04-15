using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DataLayer;
using Microsoft.AspNet.Http;
using TreeForXml;

namespace ACL.Interfaces
{
    /// <summary>
    /// Define the report method available for the resources.
    /// </summary>
    public interface IReportMethods
    {
        /// <summary>
        /// Call this method to resolve a REPORT request
        /// for any of the ACL's defined methods.
        /// </summary>
        /// <returns></returns>
        void ProcessRequest(HttpRequest request, CalDavContext context, HttpResponse response);

        /// <summary>
        /// The DAV:acl-principal-prop-set report returns, for all principals in 
        /// the DAV:acl property(of the Request-URI) that are identified by 
        /// http(s) URLs or by a DAV:property principal, the value of the 
        /// properties specified in the REPORT request body.
        /// </summary>
        /// <returns></returns>
        void AclPrincipalPropSet(IXMLTreeStructure body, HttpRequest request, CalDavContext context, HttpResponse response);

        /// <summary>
        /// The DAV:principal-match REPORT is used to identify all members (at 
        /// any depth) of the collection identified by the Request-URI that are 
        /// principals and that match the current user.
        /// </summary>
        /// <returns></returns>
        void PrincipalMatch(IXMLTreeStructure body, HttpRequest request, CalDavContext context, HttpResponse response);

        /// <summary>
        /// The DAV:principal-property-search REPORT performs a search for all 
        /// principals whose properties contain character data that matches the 
        /// search criteria specified in the request.
        /// </summary>
        /// <returns></returns>
        void PrincipalPropertySearch(IXMLTreeStructure body, HttpRequest request, CalDavContext context, HttpResponse response);

        /// <summary>
        /// The DAV:principal-search-property-set REPORT identifies those 
        /// properties that may be searched using the DAV:principal-property 
        /// search REPORT(defined in Section 9.4).
        /// </summary>
        /// <returns></returns>
        void PrincipalSearchPropertySet( HttpResponse response);
    }
}
