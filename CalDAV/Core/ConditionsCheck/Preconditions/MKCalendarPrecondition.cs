using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using ACL.Core.Authentication;
using ACL.Core.CheckPermissions;
using CalDAV.Core.Method_Extensions;
using DataLayer;
using DataLayer.Models.Entities;
using DataLayer.Repositories;
using Microsoft.AspNet.Http;
using TreeForXml;

namespace CalDAV.Core.ConditionsCheck
{
    public class MKCalendarPrecondition : IPrecondition
    {
        private readonly CollectionRepository _collectionRepository;
        private readonly IPermissionChecker _permissionChecker;

        public MKCalendarPrecondition(IFileManagement fileManagement,
            IRepository<CalendarCollection, string> collectionRepository, IPermissionChecker permissionChecker)
        {
            fs = fileManagement;
            _collectionRepository = collectionRepository as CollectionRepository;
            _permissionChecker = permissionChecker;
        }

        public IFileManagement fs { get; }


        public async Task<bool> PreconditionsOK(Dictionary<string, string> propertiesAndHeaders, HttpResponse response)
        {
            #region Extracting Properties

            var body = propertiesAndHeaders["body"];
            var url = propertiesAndHeaders["url"];
            var principalUrl = propertiesAndHeaders["principalUrl"];

            #endregion

            if (!PermissionCheck(url, principalUrl, response))
                return false;

            if (fs.ExistCalendarCollection(url) || await _collectionRepository.Exist(url))
            {
                response.StatusCode = (int) HttpStatusCode.Forbidden;
                response.Body.Write(@"<?xml version='1.0' encoding='UTF-8'?>
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
                    response.StatusCode = (int) HttpStatusCode.Forbidden;
                    response.Body.Write("Wrong Body");
                    return false;
                }
                if (bodyTree.NodeName != "mkcalendar")
                {
                    response.StatusCode = (int) HttpStatusCode.Forbidden;
                    response.Body.Write("Wrong Body");

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