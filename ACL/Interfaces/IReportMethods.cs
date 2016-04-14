using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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
        string ProcessRequest(IXMLTreeStructure body, HttpRequest request);

        /// <summary>
        /// The DAV:acl-principal-prop-set report returns, for all principals in 
        /// the DAV:acl property(of the Request-URI) that are identified by 
        /// http(s) URLs or by a DAV:property principal, the value of the 
        /// properties specified in the REPORT request body.
        /// </summary>
        /// <returns></returns>
        string AclPrincipalPropSet();

        /// <summary>
        /// The DAV:principal-match REPORT is used to identify all members (at 
        /// any depth) of the collection identified by the Request-URI that are 
        /// principals and that match the current user.
        /// </summary>
        /// <returns></returns>
        string PrincipalMatch();


        string PrincipalProperty();
    }
}
