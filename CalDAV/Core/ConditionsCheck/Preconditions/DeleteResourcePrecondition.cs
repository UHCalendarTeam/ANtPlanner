using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ACL.Core.Authentication;
using ACL.Core.CheckPermissions;
using CalDAV.Core.Method_Extensions;
using CalDAV.Method_Extensions;
using DataLayer;
using DataLayer.Models.Entities;
using DataLayer.Models.Entities.ResourcesAndCollections;
using DataLayer.Models.Interfaces.Repositories;
using DataLayer.Models.Repositories;
using Microsoft.AspNetCore.Http;

namespace CalDAV.Core.ConditionsCheck.Preconditions
{
    public class DeleteResourcePrecondition : IPrecondition
    {
        private readonly ICollectionRepository _collectionRepository;
        private readonly ICalendarResourceRepository _resourceRespository;
        private readonly IPermissionChecker _permissionChecker;
        private readonly IAuthenticate _authenticate; 

        public DeleteResourcePrecondition(IRepository<CalendarCollection, string> collectionRepository,
            IRepository<CalendarResource, string> resourceRepository, IPermissionChecker permissionChecker, IAuthenticate authenticate)
        {
            _collectionRepository = collectionRepository as ICollectionRepository;
            _resourceRespository = resourceRepository as ICalendarResourceRepository;
            _permissionChecker = permissionChecker;
            _authenticate = authenticate;
        }

        public async Task<bool> PreconditionsOK(HttpContext httpContext)
        {
            string url = httpContext.Request.GetRealUrl();

            string principalUrl = (await _authenticate.AuthenticateRequestAsync(httpContext))?.PrincipalUrl;

            //if (!await _collectionRepository.Exist(url))
            //{
            //    response.StatusCode = (int)HttpStatusCode.NotFound;
            //    return false;
            //}
            //if (!await _resourceRespository.Exist(url))
            //{
            //    response.StatusCode = (int)HttpStatusCode.NotFound;
            //    return false;
            //}

            return PermissionCheck(url, principalUrl, httpContext.Response);


        }
        private bool PermissionCheck(string url, string principalUrl, HttpResponse response)
        {
            return _permissionChecker.CheckPermisionForMethod(url, principalUrl, response,
                SystemProperties.HttpMethod.DeleteResource);
        }
    }
}
