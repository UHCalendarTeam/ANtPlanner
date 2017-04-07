using System.Net;
using System.Threading.Tasks;
using ACL.Core.Authentication;
using ACL.Core.CheckPermissions;
using CalDAV.Core.ConditionsCheck;
using CalDAV.Method_Extensions;
using DataLayer;
using DataLayer.Models.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;

namespace CalDAV.ConditionsCheck.Preconditions
{
    public class ProppatchPrecondition : IPrecondition
    {
        private readonly ICollectionRepository _collectionRepository;
        private readonly ICalendarResourceRepository _calendarResourceRespository;
        private readonly IPermissionChecker _permissionChecker;
        private readonly IAuthenticate _authenticate;

        public ProppatchPrecondition(ICollectionRepository collectionRepository,
            ICalendarResourceRepository calendarResourceRepository, IPermissionChecker permissionChecker, IAuthenticate authenticate)
        {
            _collectionRepository = collectionRepository;
            _calendarResourceRespository = calendarResourceRepository;
            _permissionChecker = permissionChecker;
            _authenticate = authenticate;
        }

        public async Task<bool> PreconditionsOK(HttpContext httpContext)
        {
            string calendarResourceId = httpContext.Request.GetResourceId();

            string url = httpContext.Request.GetRealUrl();

            string principalUrl= (await _authenticate.AuthenticateRequestAsync(httpContext))?.PrincipalUrl;

            if (calendarResourceId == null && !await _collectionRepository.Exist(url))
            {
                httpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return false;
            }
            if (calendarResourceId != null && !await _calendarResourceRespository.Exist(url))
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