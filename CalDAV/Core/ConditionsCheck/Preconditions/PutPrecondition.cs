using System.Collections.Generic;
using System.Linq;
using System.Net;
using CalDAV.Core.Method_Extensions;
using DataLayer;
using ICalendar.Calendar;
using ICalendar.ComponentProperties;
using ICalendar.GeneralInterfaces;
using Microsoft.AspNet.Http;

namespace CalDAV.Core.ConditionsCheck
{
    public class PutPrecondition : IPrecondition
    {
        public PutPrecondition(IFileSystemManagement manager, CalDavContext context)
        {
            db = context;
            StorageManagement = manager;
        }

        private IFileSystemManagement StorageManagement { get; }

        private CalDavContext db { get; }

        public bool PreconditionsOK(Dictionary<string, string> propertiesAndHeaders, HttpResponse response)
        {
            #region Extracting Properties
            var url = propertiesAndHeaders["url"];

            var body = propertiesAndHeaders["body"];
            //var reader = new StringReader(body);//esto aki no es necesario pues el constructor de VCalendar coge un string
            var iCalendar = new VCalendar(body); //lo que no estoy seguro que en el body solo haya el iCal string

            #endregion

            //calendar data is ok

            if (iCalendar == null)
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                return false;
            }

            //check that resourceId don't exist but the collection does.
            if (!StorageManagement.ExistCalendarCollection(url.Remove(url.LastIndexOf("/") + 1)))
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
                return false;
            }


            //check that if the resource exist then it has to have the same UID
            //if the resource not exist can not be another resource with the same uid.
            if (!StorageManagement.ExistCalendarObjectResource(url))
            {
                var uid = iCalendar.GetComponentProperties("UID");
                // var resource = db.GetCalendarResource(userEmail, collectionName, calendarResourceId);
                var collection = db.GetCollection(url.Remove(url.LastIndexOf("/") + 1));
                foreach (var calendarresource in collection.CalendarResources)
                {
                    if (uid.StringValue == calendarresource.Uid)
                    {
                        response.StatusCode = (int)HttpStatusCode.Conflict;
                        response.Body.Write(
                            $@"<?xml version='1.0' encoding='UTF-8'?>
<error xmlns='DAV:'>
<no-uid-conflict xmlns='urn:ietf:params:xml:ns:caldav'>
<href xmlns='DAV:'>{calendarresource
                                .Href}</href>
</no-uid-conflict>
</error>");
                        return false;
                    }
                }
            }
            else
            {
                var uid = iCalendar.GetComponentProperties("UID");
                var resource = db.GetCalendarResource(url);
                if (resource.Uid != null && resource.Uid == uid.StringValue)
                {
                    response.StatusCode = (int)HttpStatusCode.Conflict;
                    response.Body.Write(
                        $@"<?xml version='1.0' encoding='UTF-8'?>
<error xmlns='DAV:'>
<no-uid-conflict xmlns='urn:ietf:params:xml:ns:caldav'>
<href xmlns='DAV:'>{resource
                            .Href}</href>
</no-uid-conflict>
</error>");
                    return false;
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

            if (propertiesAndHeaders.ContainsKey("If-Non-Match"))
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
                response.StatusCode = (int)HttpStatusCode.Conflict;
                response.Body.Write(@"<?xml version='1.0' encoding='UTF-8'?>
<error xmlns='DAV:'>
<valid-calendar-object-resource xmlns='urn:ietf:params:xml:ns:caldav'></valid-calendar-object-resource>
<error-description xmlns='http://twistedmatrix.com/xml_namespace/dav/'>
Wrong amount of calendar components
</error-description>
</error>");
                return false;
            }


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


                var calendarComponent =
                    iCalendar.CalendarComponents.Where(comp => comp.Key != "VTIMEZONE").Single().Value;

                //A Calendar Component can be separated in multiples calendar components but all MUST
                //have the same UID.
                var uid = ((ComponentProperty<string>)calendarComponent.FirstOrDefault().Properties["UID"]).Value;
                string uidComp;
                foreach (var component in calendarComponent)
                {
                    uidComp = ((ComponentProperty<string>)component.Properties["UID"]).Value;
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


            //TODO: Check if the size in octets of the resource is less-equal
            //than the max-resource-size property


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