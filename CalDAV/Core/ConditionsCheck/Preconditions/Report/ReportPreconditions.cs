using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using ACL.Core.Authentication;
using ACL.Core.CheckPermissions;
using CalDAV.Core.ConditionsCheck.Preconditions.Report;
using CalDAV.Core.Method_Extensions;
using DataLayer;
using ICalendar.Calendar;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace CalDAV.Core.ConditionsCheck.Preconditions
{
    public class ReportPreconditions : IReportPreconditions
    {
        private IPermissionChecker _permissionChecker;
        private IAuthenticate _authenticator;
        public ReportPreconditions(IPermissionChecker permissionChecker, IAuthenticate authenticate)
        {
            _permissionChecker = permissionChecker;
            _authenticator = authenticate;
        }

        public bool PreconditionProcessor(HttpContext httpContext)
        {
            var principal = _authenticator.AuthenticateRequest(httpContext);
            var resourceUrl = GetRealUrl(httpContext.Request);

            //for the moment just checking the permission
            //on the resource
            return HasPermission(principal.PrincipalUrl,resourceUrl,httpContext.Response);

            
        }

        public bool SuppoprtedCalendarData(string contentType, string version)
        {
            throw new NotImplementedException();
        }

        public bool ValidFilter(XDocument xDoc)
        {
            throw new NotImplementedException();
        }

        public bool SupportedFilter(XDocument compFilter)
        {
            throw new NotImplementedException();
        }

        public bool ValidCalendarData(VCalendar timeZone)
        {
            throw new NotImplementedException();
        }

        public bool MinDateTime(List<DateTime> dts)
        {
            throw new NotImplementedException();
        }

        public bool MaxDateTime(List<DateTime> dts)
        {
            throw new NotImplementedException();
        }

        public bool SupportedCollation(string collation)
        {
            throw new NotImplementedException();
        }

        public bool HasPermission(string pUrl, string resUrl, HttpResponse response)
        {
            return _permissionChecker.CheckPermisionForMethod(resUrl, pUrl, response, SystemProperties.HttpMethod.Report);
        }

        public void SetErrorResponse(int statusCode, string errorMessage, HttpContext httpContext)
        {
            XNamespace xmlnsDav = "DAV:";
            XNamespace xmlnsCaldav = "urn:ietf:params:xml:ns:caldav";

            XDocument respBody = new XDocument(
                new XElement(xmlnsDav+"error",
                new XElement(xmlnsCaldav+errorMessage)));
            httpContext.Response.Body.Write(respBody.ToString());
            httpContext.Response.StatusCode = statusCode;
        }



        private string GetRealUrl(HttpRequest request)
        {
            var url = request.GetEncodedUrl();
            var host = "http://" + request.Host.Value + SystemProperties._baseUrl;
            url = url.Replace(host, "");
            return url;
        }
    }
}
