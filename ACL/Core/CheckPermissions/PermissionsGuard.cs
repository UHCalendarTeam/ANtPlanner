﻿using System.Collections.Generic;
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

        public PermissionsGuard(IRepository<CalendarResource,string> resRepository, IRepository<CalendarCollection, string> calRepository)
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

            if (method == SystemProperties.HttpMethod.Put ||
                method == SystemProperties.HttpMethod.Delete ||
                method == SystemProperties.HttpMethod.Propatch)
                if (permissions.Any(x => x == _writePermissionString))
                    return true;

            if (method == SystemProperties.HttpMethod.Get ||
                method == SystemProperties.HttpMethod.Report ||
                method == SystemProperties.HttpMethod.Profind)
                if (permissions.Any(x => x == _readPermissionString))
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
            var aclP =
                _resourceRepo.Get(url).Properties.FirstOrDefault(x => x.Name == "acl" && x.Namespace == "DAV:");
            //take the acl property

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

            foreach (var ace in acesToCheck)
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