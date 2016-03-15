using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CalDAV.Models;
using ICalendar.ComponentProperties;
using ICalendar.Utils;

namespace CalDAV.Core.ConditionsCheck
{
    public class PutPrecondition : IPrecondition
    {
        IFileSystemManagement StorageManagement { get; }

        public PutPrecondition(IFileSystemManagement manager)
        {
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
            var reader = new StringReader(body);
            var iCalendar = Parser.CalendarBuilder(reader);
            #endregion

            //check that resourceId don't exist but the collection does.
            if (!StorageManagement.ExistCalendarCollection(userEmail, collectionName))
                return false;

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
                var uid = ((ComponentProperty<string>)calendarComponent.FirstOrDefault().Properties["UID"].FirstOrDefault()).Value;
                string uidComp;
                foreach (var component in calendarComponent)
                {
                    uidComp = ((ComponentProperty<string>)component.Properties["UID"].FirstOrDefault()).Value;
                    if (uid != uidComp)
                        return false;
                }
            }


            var uidCalendar = ((ComponentProperty<string>)iCalendar.Properties["UID"].FirstOrDefault()).Value;
            //Check that if the operation is create there is not another element in the collection with the same UID
            if (!StorageManagement.ExistCalendarObjectResource(userEmail, collectionName, calendarResourceId))
            {
                
                using (var db = new CalDavContext())
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
                using (var db = new CalDavContext())
                {
                    if ((from calendarResource in db.CalendarResources
                         where calendarResource.Uid == uidCalendar
                         select calendarResource).Count() == 0)
                        return false;

                }
            }

            //TODO: Check if the size in octets of the resource is less-equal
            //than the max-resource-size property


            //Cheking that all DateTime values are 


            return true;
        }
    }
}
