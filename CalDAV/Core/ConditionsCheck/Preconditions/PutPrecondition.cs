using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DataLayer;
using ICalendar.Calendar;
using ICalendar.ComponentProperties;
using ICalendar.GeneralInterfaces;
using ICalendar.Utils;

namespace CalDAV.Core.ConditionsCheck
{
    public class PutPrecondition : IPrecondition
    {
        IFileSystemManagement StorageManagement { get; }

        private CalDavContext db { get; }

        public PutPrecondition(IFileSystemManagement manager, CalDavContext context)
        {
            db = context;
            StorageManagement = manager;
        }
        public bool PreconditionsOK(Dictionary<string, string> propertiesAndHeaders)
        {
            #region Extracting Properties
            var userEmail = propertiesAndHeaders["userEmail"];
            var collectionName = propertiesAndHeaders["collectionName"];
            var calendarResourceId = propertiesAndHeaders["calendarResourceID"];
            var etag = propertiesAndHeaders["etag"];
            var body = propertiesAndHeaders["body"];
            //var reader = new StringReader(body);//esto aki no es necesario pues el constructor de VCalendar coge un string
            var iCalendar = new VCalendar(body);//lo que no estoy seguro que en el body solo haya el iCal string
            #endregion

            //check that resourceId don't exist but the collection does.
            if (!StorageManagement.SetUserAndCollection(userEmail, collectionName))
                return false;

            if (propertiesAndHeaders.ContainsKey("If-Match"))
            {
                //check that the value do exist
                if (!StorageManagement.ExistCalendarObjectResource( calendarResourceId))
                    return false;
            }

            if (propertiesAndHeaders.ContainsKey("If-Non-Match"))
            {
                //check that the value do exist
                if (StorageManagement.ExistCalendarObjectResource(calendarResourceId))
                    return false;
            }

            //calendar data is ok
            if (iCalendar == null)
                return false;

            //it does not contain more than two calendar components
            //and if it has 2, one must be VTIMEZONE
            if (iCalendar.CalendarComponents.Count > 2)
                return false;

            if (iCalendar.CalendarComponents.Count == 2)
            {
                if (!iCalendar.CalendarComponents.ContainsKey("VTIMEZONE"))
                    return false;

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
                        return false;
                }
            }


            var uidCalendar = ((ComponentProperty<string>)iCalendar.Properties["UID"]).Value;
            //Check that if the operation is create there is not another element in the collection with the same UID
            if (!StorageManagement.ExistCalendarObjectResource( calendarResourceId))
            {
                using (db)
                {
                    if ((from calendarResource in db.CalendarResources
                        where calendarResource.Uid == uidCalendar
                        select calendarResource).Count() > 0)
                        return false;

                }
            }
            //Check if the operation is update the element to be updated must have the same UID.
            else
            {
                using (db)
                {
                    if ((from calendarResource in db.CalendarResources
                         where calendarResource.Uid == uidCalendar
                         select calendarResource).Count() == 0)
                        return false;

                }
            }

            var methodProp = iCalendar.GetComponentProperties("METHOD");
            //iCalendar object MUST NOT implement METHOD property
            if (methodProp != null || ((IValue<string>)methodProp).Value != "")
                return false;


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
