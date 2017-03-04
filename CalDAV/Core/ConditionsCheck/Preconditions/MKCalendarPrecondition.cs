using System.Collections.Generic;
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
using TreeForXml;

namespace CalDAV.Core.ConditionsCheck
{
    public class MKCalendarPrecondition : IPrecondition
    {
        private readonly CollectionRepository _collectionRepository;
        private readonly IPermissionChecker _permissionChecker;
        public IFileManagement fs { get; }
        private readonly IAuthenticate _autheticate;
        public MKCalendarPrecondition(IFileManagement fileManagement,
            IRepository<CalendarCollection, string> collectionRepository, IPermissionChecker permissionChecker, IAuthenticate authenticate)
        {
            fs = fileManagement;
            _collectionRepository = collectionRepository as CollectionRepository;
            _permissionChecker = permissionChecker;
            _autheticate = authenticate;
        }




        public async Task<bool> PreconditionsOK(HttpContext httpContext)
        {
            #region Extracting Properties

            var body = httpContext.Request.Body.StreamToString();
            var url = httpContext.Request.GetRealUrl();
            var principalUrl = (await _autheticate.AuthenticateRequestAsync(httpContext))?.PrincipalUrl;

            #endregion

            if (!PermissionCheck(url, principalUrl, httpContext.Response))
                return false;

            if (fs.ExistCalendarCollection(url) || await _collectionRepository.Exist(url))
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                httpContext.Response.Body.Write(@"<?xml version='1.0' encoding='UTF-8'?>
<error xmlns='DAV:'>
  <resource-must-be-null/>  
</error>");

                return false;
            }

            if (!string.IsNullOrEmpty(body))
            {
                var bodyTree = XmlTreeStructure.Parse(body);
                if (bodyTree == null)
                {
                    httpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    httpContext.Response.Body.Write("Wrong Body");
                    return false;
                }
                if (bodyTree.NodeName != "mkcalendar")
                {
                    httpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    httpContext.Response.Body.Write("Wrong Body");

                    return false;
                }
            }

            return true;
        }

        private bool PermissionCheck(string url, string principalUrl, HttpResponse response)
        {
            return _permissionChecker.CheckPermisionForMethod(url, principalUrl, response,
                SystemProperties.HttpMethod.MKCalendar);
        }
    }
}