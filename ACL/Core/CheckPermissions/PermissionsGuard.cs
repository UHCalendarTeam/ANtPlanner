using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DataLayer.Repositories;

namespace ACL.Core.CheckPermissions
{
    public class PermissionsGuard : IPermissionChecker
    {
        private readonly ResourceRespository _resourceRepo;
        private const string _readPermissionString = "read";
        private const string _writePermissionString = "write";

        public PermissionsGuard(ResourceRespository repo)
        {
            _resourceRepo = repo;
        }

        public bool CheckReadPermission(string resourceUrl, string principalUrl)
        {
            var permissions = TakePermissions(resourceUrl, principalUrl);

            if (permissions.Any(x => x == _readPermissionString))
                return true;

            return true;
        }

        public bool CheckWritePermission(string resourceUrl, string principalUrl)
        {
            var permissions = TakePermissions(resourceUrl, principalUrl);

            if (permissions.Any(x => x == _writePermissionString))
                return true;

            return true;
        }


        /// <summary>
        ///     Take the permission granted for the given principal
        ///     Include the permissions granted to all the principals.
        /// </summary>
        /// <param name="resourceUrl">The URL that identifies the resource.</param>
        /// <param name="principalUrl">
        ///     The URL that identifies the principal
        ///     that is making the request.
        /// </param>
        /// <returns></returns>
        private List<string> TakePermissions(string resourceUrl, string principalUrl)
        {
            var output = new List<string>();
            var acesToCheck = new List<XElement>();
            var aclP =
                _resourceRepo.Get(resourceUrl).Properties.FirstOrDefault(x => x.Name == "acl" && x.Namespace == "DAV:");
            //take the acl property

            var xdoc = XDocument.Parse(aclP.Value);
            IEnumerable<XElement> principalGrantPermissions = null;
            XName aceName = "ace";
            var descendants = xdoc.Descendants();
            var aces = descendants.Where(x => x.Name.LocalName == aceName).ToArray();

            //take the ACEs with the given principal
            acesToCheck.Add(aces.FirstOrDefault(ace => ace.Descendants()
                .FirstOrDefault(x => x.Name.LocalName == "href")?.Value == principalUrl));
            //take the ACEs granted to all the principals
            acesToCheck.Add(aces.FirstOrDefault(ace => ace.Descendants().FirstOrDefault(x => x.Name.LocalName == "principal")
                .Descendants().FirstOrDefault(x => x.Name.LocalName == "all") != null));

            foreach (var ace in acesToCheck)
            {
                output.AddRange(ace.Descendants()
             .FirstOrDefault(x => x.Name.LocalName == "grant")
             .Descendants().Where(x => x.Name.LocalName == "privilege")
             .Descendants().Select(x=>x.Name.LocalName));
            }
             
            return output;
        }
    }
}