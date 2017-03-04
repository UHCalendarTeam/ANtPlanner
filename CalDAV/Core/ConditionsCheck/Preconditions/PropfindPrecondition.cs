using System.Net;
using System.Threading.Tasks;
using ACL.Core.Authentication;
using ACL.Core.CheckPermissions;
using CalDAV.Core.ConditionsCheck;
using CalDAV.Core.Method_Extensions;
using CalDAV.Method_Extensions;
using DataLayer;
using DataLayer.Models.Entities.ResourcesAndCollections;
using DataLayer.Models.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;

namespace CalDAV.ConditionsCheck.Preconditions
{
    public class PropfindPrecondition : IPrecondition
    {
        private readonly ICollectionRepository _collectionRepository;
        private readonly ICalendarResourceRepository _resourceRespository;
        private readonly IPermissionChecker _permissionChecker;
        private readonly IAuthenticate _authenticate;

        public PropfindPrecondition(IRepository<CalendarCollection, string> collectionRepository,
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

            string calendarResourceId = httpContext.Request.GetResourceId();

            string principalUrl = (await _authenticate.AuthenticateRequestAsync(httpContext))?.PrincipalUrl;

            //Todo: cambiar para await;
            var a = _collectionRepository.FindUrl(url);
            if (calendarResourceId == null &&  a == null)
            {
                httpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return false;
            }
            //todo: cambiar para que funcione el await
            var b = _resourceRespository.FindUrl(url);
            if (calendarResourceId != null && b == null)
            {
                httpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return false;
            }

            return PermissionCheck(url, calendarResourceId, principalUrl, httpContext.Response);
        }

        private bool PermissionCheck(string url, string resourceId,string principalUrl, HttpResponse response)
        {
          return resourceId == null?
                _permissionChecker.CheckPermisionForMethod(url, principalUrl, response, SystemProperties.HttpMethod.ProfindCollection) :
                _permissionChecker.CheckPermisionForMethod(url, principalUrl, response, SystemProperties.HttpMethod.ProfindResource);
        }
    }
}