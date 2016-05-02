using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using CalDAV.Core.Method_Extensions;
using DataLayer;
using DataLayer.Models.Entities;
using DataLayer.Repositories;
using ICalendar.Calendar;
using Microsoft.AspNet.Http;
using TreeForXml;

namespace CalDAV.Core.ConditionsCheck
{
    public class PutPrecondition : IPrecondition
    {
        public PutPrecondition(IFileSystemManagement manager, IRepository<CalendarCollection, string> collectionRepository, IRepository<CalendarResource, string> resourceRepository)
        {
            _collectionRepository = collectionRepository as CollectionRepository;
            _resourceRespository = resourceRepository as ResourceRespository;
            StorageManagement = manager;
        }

        private readonly CollectionRepository _collectionRepository;
        private readonly ResourceRespository _resourceRespository;

        private IFileSystemManagement StorageManagement { get; }

        public bool PreconditionsOK(Dictionary<string, string> propertiesAndHeaders, HttpResponse response)
        {
            #region Extracting Properties
            var url = propertiesAndHeaders["url"];

            var contentSize = propertiesAndHeaders["content-length"];
            var body = propertiesAndHeaders["body"];
            VCalendar iCalendar;
            try
            {
                iCalendar = new VCalendar(body); //lo que no estoy seguro que en el body solo haya el iCal string
            }
            catch (Exception)
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                return false;
            }
           

            #endregion

           

            //check that resourceId don't exist but the collection does.
            if (!StorageManagement.ExistCalendarCollection(url.Remove(url.LastIndexOf("/", StringComparison.Ordinal) + 1)))
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
                return false;
            }


            //check that if the resource exist then all its components different of VTIMEZONE has to have the same UID
            //if the resource not exist can not be another resource with the same uid.
            if (!StorageManagement.ExistCalendarObjectResource(url))
            {
                var component = iCalendar.CalendarComponents.FirstOrDefault(comp => comp.Key != "VTIMEZONE").Value;
                var uid = component.FirstOrDefault()?.Properties["UID"].StringValue;
                // var resource = db.GetCalendarResource(userEmail, collectionName, calendarResourceId);
                var collection = _collectionRepository.Get(url.Remove(url.LastIndexOf("/", StringComparison.Ordinal) + 1)).Result;
                foreach (var calendarresource in collection.CalendarResources)
                {
                    if (uid == calendarresource.Uid)
                    {
                        response.StatusCode = (int)HttpStatusCode.Conflict;
                        response.Body.Write(
                            $@"<?xml version='1.0' encoding='UTF-8'?>
<error xmlns='DAV:'>
<no-uid-conflict xmlns='urn:ietf:params:xml:ns:caldav'>
<href xmlns='DAV:'>{SystemProperties._baseUrl + calendarresource
                                .Href}</href>
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

                    var resource = _resourceRespository.Get(url).Result;

                    if (resource.Uid != null && resource.Uid != uid)
                    {
                        response.StatusCode = (int)HttpStatusCode.Conflict;
                        response.Body.Write(
                            $@"<?xml version='1.0' encoding='UTF-8'?>
<error xmlns='DAV:'>
<no-uid-conflict xmlns='urn:ietf:params:xml:ns:caldav'>
<href xmlns='DAV:'>{SystemProperties._baseUrl + resource
                                .Href}</href>
</no-uid-conflict>
</error>");
                        return false;
                    }
                }
            }


            if (propertiesAndHeaders.ContainsKey("If-Match"))
            {
                //check that the value do exist
                if (!StorageManagement.ExistCalendarObjectResource(url))
                {
                    response.StatusCode = (int)HttpStatusCode.PreconditionFailed;
                    return false;
                }
            }

            if (propertiesAndHeaders.ContainsKey("If-None-Match"))
            {

                //check that the value do not exist
                if (StorageManagement.ExistCalendarObjectResource(url))
                {
                    response.StatusCode = (int)HttpStatusCode.PreconditionFailed;
                    return false;
                }
            }


            //it does not contain more than two calendar components
            //and if it has 2, one must be VTIMEZONE
            if (iCalendar.CalendarComponents.Count > 2)
            {

                if (!iCalendar.CalendarComponents.ContainsKey("VTIMEZONE"))
                {
                    response.StatusCode = (int)HttpStatusCode.Conflict;
                    response.Body.Write(@"<?xml version='1.0' encoding='UTF-8'?>
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
                            response.StatusCode = (int)HttpStatusCode.Conflict;
                            response.Body.Write(@"<?xml version='1.0' encoding='UTF-8'?>
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
                    response.StatusCode = (int)HttpStatusCode.Conflict;
                    response.Body.Write(@"<?xml version='1.0' encoding='UTF-8'?>
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
                response.StatusCode = (int)HttpStatusCode.Conflict;
                response.Body.Write(@"<?xml version='1.0' encoding='UTF-8'?>
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
                var collection = _collectionRepository.Get(url.Remove(url.LastIndexOf("/", StringComparison.Ordinal) + 1)).Result;
                //here the max-resource-property of the collection is called.
                var maxSize = collection.Properties.FirstOrDefault(p => p.Name == "max-resource-size" && p.Namespace == "urn:ietf:params:xml:ns:caldav");
                int maxSizeInt;
                if (int.TryParse(XmlTreeStructure.Parse(maxSize?.Value).Value, out maxSizeInt) && contentSizeInt > maxSizeInt)
                {
                    response.StatusCode = (int)HttpStatusCode.Conflict;
                    response.Body.Write(@"<?xml version='1.0' encoding='UTF-8'?>
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

            return true;
        }
    }
}