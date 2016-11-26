using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using ACL.Core.Authentication;
using ACL.Core.CheckPermissions;
using CalDAV.Core.Method_Extensions;
using DataLayer;
using DataLayer.Models.Entities;
using DataLayer.Repositories;
using Microsoft.AspNetCore.Http;

namespace CalDAV.Core.ConditionsCheck.Preconditions
{
    public class ProppatchPrecondition : IPrecondition
    {
        private readonly CollectionRepository _collectionRepository;
        private readonly ResourceRespository _resourceRespository;
        private readonly IPermissionChecker _permissionChecker;
        private readonly IAuthenticate _authenticate;

        public ProppatchPrecondition(IRepository<CalendarCollection, string> collectionRepository,
            IRepository<CalendarResource, string> resourceRepository, IPermissionChecker permissionChecker, IAuthenticate authenticate)
        {
            _collectionRepository = collectionRepository as CollectionRepository;
            _resourceRespository = resourceRepository as ResourceRespository;
            _permissionChecker = permissionChecker;
            _authenticate = authenticate;
        }

        public async Task<bool> PreconditionsOK(HttpContext httpContext)
        {
            string calendarResourceId = httpContext.Request.GetResourceId();

            string url = httpContext.Request.GetRealUrl();

            string principalUrl= (await _authenticate.AuthenticateRequestAsync(httpContext))?.PrincipalURL;

            if (calendarResourceId == null && !await _collectionRepository.Exist(url))
            {
                httpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return false;
            }
            if (calendarResourceId != null && !await _resourceRespository.Exist(url))
            {
                httpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return false;
            }
            return PermissionCheck(url, calendarResourceId, principalUrl, httpContext.Response);
        }

        private bool PermissionCheck(string url, string resourceId, string principalUrl, HttpResponse response)
        {
            return resourceId == null ?
                  _permissionChecker.CheckPermisionForMethod(url, principalUrl, response, SystemProperties.HttpMethod.PropatchCollection) :
                  _permissionChecker.CheckPermisionForMethod(url, principalUrl, response, SystemProperties.HttpMethod.ProppatchResource);
        }
    }
}