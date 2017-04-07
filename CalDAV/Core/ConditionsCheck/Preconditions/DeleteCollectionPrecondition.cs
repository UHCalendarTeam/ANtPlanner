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
    public class DeleteCollectionPrecondition:IPrecondition
    {
        private readonly ICollectionRepository _collectionRepository;
        private readonly ICalendarResourceRepository _calendar_resourceRespository;
        private readonly IPermissionChecker _permissionChecker;
        private readonly IAuthenticate _authenticate;

        public DeleteCollectionPrecondition(ICollectionRepository collectionRepository,
            ICalendarResourceRepository resourceRepository, IPermissionChecker permissionChecker, IAuthenticate authenticate)
        {
            _collectionRepository = collectionRepository;
            _calendar_resourceRespository = resourceRepository;
            _permissionChecker = permissionChecker;
            _authenticate = authenticate;
        }

        public async Task<bool> PreconditionsOK(HttpContext httpContext)
        {
            string url = httpContext.Request.GetRealUrl();

            string principalUrl = (await _authenticate.AuthenticateRequestAsync(httpContext))?.PrincipalUrl;
            
            return PermissionCheck(url, principalUrl, httpContext.Response);
        }
        private bool PermissionCheck(string url, string principalUrl, HttpResponse response)
        {
            return _permissionChecker.CheckPermisionForMethod(url, principalUrl, response,
                SystemProperties.HttpMethod.DeleteCollection);
        }
    }
}
