using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ACL.Core.CheckPermissions;
using DataLayer;
using DataLayer.Models.Entities;
using DataLayer.Repositories;
using Microsoft.AspNet.Http;

namespace CalDAV.Core.ConditionsCheck.Preconditions
{
    public class DeleteCollectionPrecondition:IPrecondition
    {
        private readonly CollectionRepository _collectionRepository;
        private readonly ResourceRespository _resourceRespository;
        private readonly IPermissionChecker _permissionChecker;

        public DeleteCollectionPrecondition(IRepository<CalendarCollection, string> collectionRepository,
            IRepository<CalendarResource, string> resourceRepository, IPermissionChecker permissionChecker)
        {
            _collectionRepository = collectionRepository as CollectionRepository;
            _resourceRespository = resourceRepository as ResourceRespository;
            _permissionChecker = permissionChecker;
        }

        public async Task<bool> PreconditionsOK(Dictionary<string, string> propertiesAndHeaders, HttpResponse response)
        {
            string url;
            propertiesAndHeaders.TryGetValue("url", out url);

            string principalUrl;
            propertiesAndHeaders.TryGetValue("principalUrl", out principalUrl);
            
            return PermissionCheck(url, principalUrl, response);


        }
        private bool PermissionCheck(string url, string principalUrl, HttpResponse response)
        {
            return _permissionChecker.CheckPermisionForMethod(url, principalUrl, response,
                SystemProperties.HttpMethod.DeleteCollection);
        }
    }
}
