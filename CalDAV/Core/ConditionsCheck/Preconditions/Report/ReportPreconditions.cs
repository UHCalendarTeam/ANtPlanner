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
using ICalendar.Calendar;
using Microsoft.AspNet.Http;

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


            return true;
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

        public bool HasPermission(HttpContext httpContext)
        {
            throw new NotImplementedException();
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
    }
}
