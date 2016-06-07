using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Xml.Linq;
using DataLayer;
using DataLayer.Models.Entities;
using DataLayer.Repositories;
using Microsoft.AspNet.Http;


namespace ACL.Core.CheckPermissions
{
    public class PermissionsGuard : IPermissionChecker
    {
        private const string _readPermissionString = "read";
        private const string _writePermissionString = "write";
        private readonly ResourceRespository _resourceRepo;
        private readonly CollectionRepository _calendarRepo;
        /// <summary>
        /// the HTTP method that need the read privilege
        /// </summary>
        private readonly SystemProperties.HttpMethod[] _readMethods =
        {
            SystemProperties.HttpMethod.Get,
            SystemProperties.HttpMethod.Report,
            SystemProperties.HttpMethod.ProfindCollection,
            SystemProperties.HttpMethod.ProfindResource
        };
        /// <summary>
        /// the HTTP method that need the write privilege
        /// </summary>
        private readonly SystemProperties.HttpMethod[] _writeMethods =
        {
          SystemProperties.HttpMethod.PutCreate,
          SystemProperties.HttpMethod.PutUpdate,
          SystemProperties.HttpMethod.DeleteCollection,
          SystemProperties.HttpMethod.DeleteResource,
          SystemProperties.HttpMethod.MKCalendar,
          SystemProperties.HttpMethod.PropatchCollection,
          SystemProperties.HttpMethod.ProppatchResource

        };

        public PermissionsGuard(IRepository<CalendarResource, string> resRepository, IRepository<CalendarCollection, string> calRepository)
        {
            _resourceRepo = resRepository as ResourceRespository;
            _calendarRepo = calRepository as CollectionRepository;
        }

        /// <summary>
        ///     Check the permission needed to perform an action
        ///     by the given method.
        /// </summary>
        /// <param name="url">The URL that identifies the resource.</param>
        /// <param name="principalUrl">
        ///     The URL that identifies the principal
        ///     that is making the request.
        /// </param>
        /// <param name="response">The response for the client</param>
        /// <param name="method">
        ///     The method is gonna be used to check what
        ///     permission has to the grant.
        /// </param>
        /// <returns>
        ///     True if the principal has the permission granted on the resource
        ///     False otherwise
        /// </returns>
        public bool CheckPermisionForMethod(string url, string principalUrl, HttpResponse response,
            SystemProperties.HttpMethod method)
        {
            var permissions = TakePermissions(url, principalUrl, method);

            //check for the write permission
            if (_writeMethods.Contains(method) && permissions.Any(x => x == _writePermissionString))
                return true;
            //check for the read permission
            if (_readMethods.Contains(method) && permissions.Any(x => x == _readPermissionString))
                return true;

            SetDeniedResponse(response);
            return false;
        }


        /// <summary>
        ///     Take the permission granted for the given principal
        ///     Include the permissions granted to all the principals.
        /// </summary>
        /// <param name="url">The URL that identifies the resource.</param>
        /// <param name="principalUrl">
        ///     The URL that identifies the principal
        ///     that is making the request.
        /// </param>
        /// <returns></returns>
        private List<string> TakePermissions(string url, string principalUrl, SystemProperties.HttpMethod method)
        {
            var output = new List<string>();
            var acesToCheck = new List<XElement>();
            Property aclP;

            //for now we'll take the permission from the calendar collection and not from the resource
            if (url.Contains(".ics") || url.Contains(".ifb"))
                url = url.Remove(url.LastIndexOf("\\") + 1);
            #region uncomment this when after change the permissions
            //check where to take the acl proeprty
            //if the method perform an action on a collection then take the 
            //acl property form the collection
            //if (method == SystemProperties.HttpMethod.ProfindCollection
            //    || method == SystemProperties.HttpMethod.Report
            //    || method == SystemProperties.HttpMethod.PutCreate)
            //aclP = _calendarRepo.Get(url).Properties.FirstOrDefault(x => x.Name == "acl" && x.Namespace == "DAV:");
            //if the method perform an action on a resource in particular then take 
            //the acl from the resource
            //else
            //    aclP = _resourceRepo.Get(url).Properties.FirstOrDefault(x => x.Name == "acl" && x.Namespace == "DAV:");
            #endregion
            //take the acl property
            aclP = _calendarRepo.Get(url).Properties.FirstOrDefault(x => x.Name == "acl" && x.Namespace == "DAV:");

            var xdoc = XDocument.Parse(aclP.Value);
            XName aceName = "ace";
            var aces = xdoc.Descendants().Where(x => x.Name.LocalName == aceName).ToArray();

            //take the ACEs with the given principal
            acesToCheck.Add(aces.FirstOrDefault(ace => ace.Descendants()
                .FirstOrDefault(x => x.Name.LocalName == "href")?.Value == principalUrl));
            //take the ACEs granted to all the principals
            acesToCheck.Add(
                aces.FirstOrDefault(ace => ace.Descendants().FirstOrDefault(x => x.Name.LocalName == "principal")
                    .Descendants().FirstOrDefault(x => x.Name.LocalName == "all") != null));

            foreach (var ace in acesToCheck.Where(x => x != null))
            {
                output.AddRange(ace.Descendants()
                    .FirstOrDefault(x => x.Name.LocalName == "grant")
                    .Descendants().Where(x => x.Name.LocalName == "privilege")
                    .Descendants().Select(x => x.Name.LocalName));
            }

            return output;
        }

        /// <summary>
        ///     Set the response for the denied request.
        /// </summary>
        /// <param name="response"></param>
        private void SetDeniedResponse(HttpResponse response)
        {
            response.StatusCode = StatusCodes.Status403Forbidden;
            //add content to the body if needed
        }
    }
}