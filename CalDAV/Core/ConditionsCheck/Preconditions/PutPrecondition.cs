using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ACL.Core.Authentication;
using ACL.Core.CheckPermissions;
using CalDAV.Core.Method_Extensions;
using DataLayer;
using DataLayer.Models.ACL;
using DataLayer.Models.Entities;
using DataLayer.Repositories;
using ICalendar.Calendar;
using Microsoft.AspNetCore.Http;
using TreeForXml;

namespace CalDAV.Core.ConditionsCheck
{
    public class PutPrecondition : IPrecondition
    {
        private readonly CollectionRepository _collectionRepository;
        private readonly ResourceRespository _resourceRespository;
        private readonly IPermissionChecker _permissionChecker;
        private readonly IAuthenticate _authenticate;
        public PutPrecondition(IFileManagement manager,
            IRepository<CalendarCollection, string> collectionRepository,
            IRepository<CalendarResource, string> resourceRepository, IPermissionChecker permissionChecker, IAuthenticate authenticate)
        {
            _collectionRepository = collectionRepository as CollectionRepository;
            _resourceRespository = resourceRepository as ResourceRespository;
            StorageManagement = manager;
            _permissionChecker = permissionChecker;
            _authenticate = authenticate;
        }

        private IFileManagement StorageManagement { get; }

        public async Task<bool> PreconditionsOK(HttpContext httpContext)
        {
            #region Extracting Properties

            var url = httpContext.Request.GetRealUrl();

            var contentSize = httpContext.Request.GetContentLength();
            var body = httpContext.Request.Body.StreamToString();
            var principalUrl = (await _authenticate.AuthenticateRequestAsync(httpContext))?.PrincipalURL;

            var ifMatch = httpContext.Request.GetIfMatchValues();
            var ifNoneMatch = httpContext.Request.GetIfNoneMatchValues();
            VCalendar iCalendar;
            try
            {
                iCalendar = new VCalendar(body);
            }
            catch (Exception)
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return false;
            }

            #endregion

            //check that resourceId don't exist but the collection does.
            if (
                !StorageManagement.ExistCalendarCollection(url.Remove(url.LastIndexOf("/", StringComparison.Ordinal) + 1)))
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return false;
            }

            var creationMode = !await _resourceRespository.Exist(url);
            if (!PermissionCheck(url, creationMode, principalUrl, httpContext.Response))
            {
                return false;
            }

            //check that if the resource exist then all its components different of VTIMEZONE has to have the same UID
            //if the resource not exist can not be another resource with the same uid.
            if (!StorageManagement.ExistCalendarObjectResource(url))
            {
                var component = iCalendar.CalendarComponents.FirstOrDefault(comp => comp.Key != "VTIMEZONE").Value;
                var uid = component.FirstOrDefault()?.Properties["UID"].StringValue;
                // var resource = db.GetCalendarResource(userEmail, collectionName, calendarResourceId);
                var collection =
                    _collectionRepository.Get(url.Remove(url.LastIndexOf("/", StringComparison.Ordinal) + 1));
                foreach (var calendarresource in collection.CalendarResources)
                {
                    if (uid == calendarresource.Uid)
                    {
                        httpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;
                        httpContext.Response.Body.Write(
                            $@"<?xml version='1.0' encoding='UTF-8'?>
<error xmlns='DAV:'>
<no-uid-conflict xmlns='urn:ietf:params:xml:ns:caldav'>
<href xmlns='DAV:'>{SystemProperties._baseUrl + calendarresource.Href}</href>
</no-uid-conflict>
</error>");
                        return false;
                    }
                }
            }
            else
            {
                //If the resource exist the procedure is update and for that the uid has to be the same.
                var components = iCalendar.CalendarComponents.FirstOrDefault(comp => comp.Key != "VTIMEZONE").Value;
                var calendarComponent = components.FirstOrDefault();
                if (calendarComponent != null)
                {
                    var uid = calendarComponent.Properties["UID"].StringValue;

                    var resource = _resourceRespository.Get(url);

                    if (resource.Uid != null && resource.Uid != uid)
                    {
                        httpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;
                        httpContext.Response.Body.Write(
                            $@"<?xml version='1.0' encoding='UTF-8'?>
<error xmlns='DAV:'>
<no-uid-conflict xmlns='urn:ietf:params:xml:ns:caldav'>
<href xmlns='DAV:'>{SystemProperties._baseUrl + resource.Href}</href>
</no-uid-conflict>
</error>");
                        return false;
                    }
                }
            }


            if (!string.IsNullOrEmpty(ifMatch))
            {
                //check that the value do exist
                if (!StorageManagement.ExistCalendarObjectResource(url))
                {
                    httpContext.Response.StatusCode = (int)HttpStatusCode.PreconditionFailed;
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(ifNoneMatch))
            {
                //check that the value do not exist
                if (StorageManagement.ExistCalendarObjectResource(url))
                {
                    httpContext.Response.StatusCode = (int)HttpStatusCode.PreconditionFailed;
                    return false;
                }
            }


            //it does not contain more than two calendar components
            //and if it has 2, one must be VTIMEZONE
            if (iCalendar.CalendarComponents.Count > 2)
            {
                if (!iCalendar.CalendarComponents.ContainsKey("VTIMEZONE"))
                {
                    httpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;
                    httpContext.Response.Body.Write(@"<?xml version='1.0' encoding='UTF-8'?>
<error xmlns='DAV:'>
<valid-calendar-object-resource xmlns='urn:ietf:params:xml:ns:caldav'></valid-calendar-object-resource>
<error-description xmlns='http://twistedmatrix.com/xml_namespace/dav/'>
VTimezone Calendar Component Must be present.
</error-description>
</error>");
                    return false;
                }

                var calendarComponents =
                    iCalendar.CalendarComponents.FirstOrDefault(comp => comp.Key != "VTIMEZONE").Value;

                //A Calendar Component can be separated in multiples calendar components but all MUST
                //have the same UID.
                var calendarComponent = calendarComponents.FirstOrDefault();
                if (calendarComponent != null)
                {
                    var uid = calendarComponent.Properties["UID"].StringValue;
                    foreach (var component in calendarComponents)
                    {
                        var uidComp = component.Properties["UID"].StringValue;
                        if (uid != uidComp)
                        {
                            httpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;
                            httpContext.Response.Body.Write(@"<?xml version='1.0' encoding='UTF-8'?>
<error xmlns='DAV:'>
<valid-calendar-object-resource xmlns='urn:ietf:params:xml:ns:caldav'></valid-calendar-object-resource>
<error-description xmlns='http://twistedmatrix.com/xml_namespace/dav/'>
If the count of calendar components execeds 2 including VTimezone the rest must have the same Uid and the same type.
</error-description>
</error>");
                            return false;
                        }
                    }
                }

                //                response.StatusCode = (int)HttpStatusCode.Conflict;
                //                response.Body.Write(@"<?xml version='1.0' encoding='UTF-8'?>
                //<error xmlns='DAV:'>
                //<valid-calendar-object-resource xmlns='urn:ietf:params:xml:ns:caldav'></valid-calendar-object-resource>
                //<error-description xmlns='http://twistedmatrix.com/xml_namespace/dav/'>
                //Wrong amount of calendar components
                //</error-description>
                //</error>");
                //                return false;
            }

            //precondition responsible of check that an VTIMEZONE is obligatory 
            if (iCalendar.CalendarComponents.Count == 2)
            {
                if (!iCalendar.CalendarComponents.ContainsKey("VTIMEZONE"))
                {
                    httpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;
                    httpContext.Response.Body.Write(@"<?xml version='1.0' encoding='UTF-8'?>
<error xmlns='DAV:'>
<valid-calendar-object-resource xmlns='urn:ietf:params:xml:ns:caldav'></valid-calendar-object-resource>
<error-description xmlns='http://twistedmatrix.com/xml_namespace/dav/'>
VTimezone Calendar Component Must be present.
</error-description>
</error>");
                    return false;
                }
            }


            //var uidCalendar = ((ComponentProperty<string>)iCalendar.Properties["UID"]).Value;
            ////Check that if the operation is create there is not another element in the collection with the same UID
            //if (!StorageManagement.ExistCalendarObjectResource(calendarResourceId))
            //{
            //    using (db)
            //    {
            //        if ((from calendarResource in db.CalendarResources
            //             where calendarResource.Uid == uidCalendar
            //             select calendarResource).Count() > 0)
            //            return false;

            //    }
            //}
            ////Check if the operation is update the element to be updated must have the same UID.
            //else
            //{
            //    using (db)
            //    {
            //        if ((from calendarResource in db.CalendarResources
            //             where calendarResource.Uid == uidCalendar
            //             select calendarResource).Count() == 0)
            //            return false;

            //    }
            //}

            var methodProp = iCalendar.GetComponentProperties("METHOD");
            //iCalendar object MUST NOT implement METHOD property
            if (!string.IsNullOrEmpty(methodProp?.StringValue))
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;
                httpContext.Response.Body.Write(@"<?xml version='1.0' encoding='UTF-8'?>
<error xmlns='DAV:'>
<valid-calendar-object-resource xmlns='urn:ietf:params:xml:ns:caldav'></valid-calendar-object-resource>
<error-description xmlns='http://twistedmatrix.com/xml_namespace/dav/'>
Method prop must not be present
</error-description>
</error>");
                return false;
            }

            //This precondition is the one in charge of check that the size of the body of the resource
            //included in the request dont exceeds the max-resource-size property of the colletion.
            int contentSizeInt;
            //for that i need that the controller has as request header content-size available 
            if (!string.IsNullOrEmpty(contentSize) && int.TryParse(contentSize, out contentSizeInt))
            {
                var collection =
                    _collectionRepository.Get(url.Remove(url.LastIndexOf("/", StringComparison.Ordinal) + 1));
                //here the max-resource-property of the collection is called.
                var maxSize =
                    collection.Properties.FirstOrDefault(
                        p => p.Name == "max-resource-size" && p.Namespace == "urn:ietf:params:xml:ns:caldav");
                int maxSizeInt;
                if (int.TryParse(XmlTreeStructure.Parse(maxSize?.Value).Value, out maxSizeInt) &&
                    contentSizeInt > maxSizeInt)
                {
                    httpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;
                    httpContext.Response.Body.Write(@"<?xml version='1.0' encoding='UTF-8'?>
<error xmlns='DAV:'>
<max-resource-size xmlns='urn:ietf:params:xml:ns:caldav'></max-resource-size>
<error-description xmlns='http://twistedmatrix.com/xml_namespace/dav/'>
Content size exceeds max size allowed.
</error-description>
</error>");
                    return false;
                }
            }


            //TODO: Checking that all DateTime values are less-equal than
            //the max-date-time 

            //TODO: Checking that all DateTime values are grater-equal than
            //the min-date-time

            //TODO: Checking that the number of recurring instances is less-equal
            //than the max-instances property value.

            return await Task.FromResult(true);
        }

        private bool PermissionCheck(string url, bool creationMode, string principalUrl, HttpResponse response)
        {
            return creationMode ?
                  _permissionChecker.CheckPermisionForMethod(url.Remove(url.LastIndexOf("/", StringComparison.Ordinal) + 1), principalUrl, response, SystemProperties.HttpMethod.PutCreate) :
                  _permissionChecker.CheckPermisionForMethod(url, principalUrl, response, SystemProperties.HttpMethod.PutUpdate);
        }
    }
}