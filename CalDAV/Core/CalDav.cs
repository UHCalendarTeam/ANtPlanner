﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalDAV.Models;
using CalDAV.Utils.XML_Processors;
using ICalendar.Calendar;
using ICalendar.Utils;
using Microsoft.Data.Entity;
using ICalendar.GeneralInterfaces;

namespace CalDAV.Core
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class CalDav : ICalDav
    {
        private IFileSystemManagement StorageManagement { get; }

        private IStartUp StartUp { get; set; }

        public CalDav(IFileSystemManagement fsManagement)
        {
            StorageManagement = fsManagement;
        }

        public string MkCalendar(Dictionary<string, string> propertiesAndHeaders, string body)
        {
            var properties = XMLParsers.XMLMKCalendarParser(body);
            StartUp.CreateCollectionForUser(propertiesAndHeaders["userEmail"], propertiesAndHeaders["collectionName"]);
            return "";

        }
        //TODO: ADriano
        public string PropFind(Dictionary<string, string> propertiesAndHeaders, string body)
        {
            //REQUEST PROPERTIES
            //prop property return the value of the specified property
            //allprop property return the value of all properties --the include elemen makes the server return 
            //other properties that will not be returnes otherwise
            //getetag property return the etag of the COR for sincro options.
            throw new NotImplementedException();
        }

        //TODO: Nacho
        public string Report(Dictionary<string, string> propertiesAndHeaders, string body)
        {
            throw new NotImplementedException();
        }

        //TODO: Adriano
        public bool AddCalendarObjectResource(Dictionary<string, string> propertiesAndHeaders, string body, out string retEtag)
        {

            var userEmail = propertiesAndHeaders["userEmail"];
            var collectionName = propertiesAndHeaders["collectionName"];
            var resourceId = propertiesAndHeaders["resourceId"];
            var etag = propertiesAndHeaders["Etag"];
            retEtag = null;

            //Note: calendar object resource = COR

            //check that resourceId don't exist but the collection does.
            if (!StorageManagement.ExistCalendarCollection(userEmail, collectionName))
                return false;
            //other case it updates the specified COR.
            //if the client intend to create (not update) a new COR it SHOULD add a HTTP header "If-None-Match : *" 
            // it secures that it will not update an COR in case that the id already exist without notice the client.
            //for an update use "If-Match" and the specific Etag
            //when created or updated the response header MUST have the Etag

            //etag value of "If-Match"
            string updateEtag;

            if (propertiesAndHeaders.TryGetValue("If-Match", out updateEtag))
            {
                //check that the value do exist
                if (!StorageManagement.ExistCalendarObjectResource(userEmail, collectionName, resourceId))
                    return false;
                return UpdateCalendarObjectResource(userEmail, collectionName, resourceId, updateEtag, body, out retEtag);
            }
            else if (propertiesAndHeaders.ContainsKey("If-Non-Match"))
            {
                //check that the value dont exist
                if (StorageManagement.ExistCalendarObjectResource(userEmail, collectionName, resourceId))
                    return false;
                return CreateCalendarObjectResource(userEmail, collectionName, resourceId, etag, body, out retEtag);
            }
            else
            {
                using (var db = new CalDavContext())
                {
                    if (db.CalendarResourceExist(userEmail, collectionName, resourceId) &&
                        StorageManagement.ExistCalendarObjectResource(userEmail, collectionName, resourceId))
                    {
                        return UpdateCalendarObjectResource(userEmail, collectionName, resourceId, etag, body,
                            out retEtag);
                    }
                    return CreateCalendarObjectResource(userEmail, collectionName, resourceId, etag, body, out retEtag);
                }
                //update or create
            }

            //body is a clean(without XML) iCalendar

            //return HTTP 201 Created 

            throw new NotImplementedException();
        }
        //TODO:Nacho
        public string PropPatch(Dictionary<string, string> propertiesAndHeaders, string Body)
        {
            throw new NotImplementedException();
        }

        public bool DeleteCalendarObjectResource(Dictionary<string, string> propertiesAndHeaders)
        {
            var userEmail = propertiesAndHeaders["userEmail"];
            var collectionName = propertiesAndHeaders["collectionName"];
            var resourceId = propertiesAndHeaders["resourceId"];
            using (var db = new CalDavContext())
            {
                var resource = db.GetCollection(userEmail, collectionName).CalendarResources.First(x => x.FileName == resourceId);
                db.CalendarResources.Remove(resource);
                db.SaveChanges();
            }
            return StorageManagement.DeleteCalendarObjectResource(userEmail, collectionName, resourceId);
        }

        public bool DeleteCalendarCollection(Dictionary<string, string> propertiesAndHeaders)
        {
            var userEmail = propertiesAndHeaders["userEmail"];
            var collectionName = propertiesAndHeaders["collectionName"];
            var resourceId = propertiesAndHeaders["resourceId"];
            using (var db = new CalDavContext())
            {
                var collection = db.GetCollection(userEmail, collectionName);
                if (collection == null)
                    return false;
                db.CalendarCollections.Remove(collection);
                return StorageManagement.DeleteCalendarCollection(userEmail, collectionName);


            }


        }


        public string ReadCalendarObjectResource(Dictionary<string, string> propertiesAndHeaders, out string etag)
        {
            var userEmail = propertiesAndHeaders["userEmail"];
            var collectionName = propertiesAndHeaders["collectionName"];
            var resourceId = propertiesAndHeaders["resourceId"];

            //Must return the Etag header of the COR

            using (var db = new CalDavContext())
            {
                var calendarRes = db.GetCalendarResource(userEmail, collectionName, resourceId);
                etag = calendarRes.Etag;
            }
            return StorageManagement.GetCalendarObjectResource(userEmail, collectionName, resourceId);
        }

        public string ReadCalendarCollection(Dictionary<string, string> propertiesAndHeaders)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new COR from a PUT when a "If-Non-Match" header is included
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="collectionName"></param>
        /// <param name="calendarResource"></param>
        /// <param name="etag"></param>
        /// <param name="body"></param>
        /// <param></param>
        private bool CreateCalendarObjectResource(string userEmail, string collectionName, string calendarResource, string etag, string body, out string retEtag)
        {
            TextReader reader = new StringReader(body);
            var iCal = ICalendar.Utils.Parser.CalendarBuilder(reader);
            retEtag = etag;

            using (var db = new CalDavContext())
            {
                if (!db.CollectionExist(userEmail, collectionName))
                    return false;


                //filling the resource
                var resource = FillResource(userEmail, collectionName, calendarResource, etag, db, iCal, out retEtag);
                //adding the resource to the db
                db.CalendarResources.Add(resource);

                //adding the file
                StorageManagement.AddCalendarObjectResourceFile(userEmail, collectionName, calendarResource, body);


                return true;
            }

        }


        /// <summary>
        /// Updates an existing COR from a PUT when a "If-Match" header is included using the corresponding etag.
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="collectionName"></param>
        /// <param name="resourceId"></param>
        /// <param name="etag"></param>
        /// <param name="body"></param>
        /// <param name="retEtag"></param>
        private bool UpdateCalendarObjectResource(string userEmail, string collectionName, string resourceId,
            string etag, string body, out string retEtag)
        {
            TextReader reader = new StringReader(body);
            var iCal = ICalendar.Utils.Parser.CalendarBuilder(reader);

            retEtag = etag;

            //This means that the object in the body is not correct
            if (iCal == null)
                return false;

            using (var db = new CalDavContext())
            {
                if (!db.CollectionExist(userEmail, collectionName) || !db.CalendarResourceExist(userEmail, collectionName, resourceId))
                    return false;

                //Fill the resource
                var resource = FillResource(userEmail, collectionName, resourceId, etag, db, iCal, out retEtag);

                //Adding to the dataBase
                db.CalendarResources.Update(resource);

                //Removing old File 
                StorageManagement.DeleteCalendarObjectResource(userEmail, collectionName, resourceId);
                //Adding New File
                StorageManagement.AddCalendarObjectResourceFile(userEmail, collectionName, resourceId, body);
            }
            return true;
        }

        /// <summary>
        /// Method in charge of fill a CalendarResource and Return it.
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="collectionName"></param>
        /// <param name="calendarResource"></param>
        /// <param name="etag"></param>
        /// <param name="db"></param>
        /// <param name="iCal"></param>
        /// <param name="retEtag"></param>
        /// <returns></returns>
        private CalendarResource FillResource(string userEmail, string collectionName, string calendarResource, string etag, CalDavContext db, VCalendar iCal, out string retEtag)
        {
            CalendarResource resource = new CalendarResource();
            resource.User = db.GetUser(userEmail);
            resource.Collection = db.GetCollection(userEmail, collectionName);
            IList<IComponentProperty> list;
            if (iCal.Properties.TryGetValue("DTSTART", out list))
            {
                DateTime? temp;
                var firstOrDefault = list.FirstOrDefault();
                if (firstOrDefault != null &&
                    ICalendar.Utils.Utils.ToDateTime(((IValue<string>)firstOrDefault).Value.ToLower(), out temp))
                    resource.DtStart = temp.Value;

            }
            if (iCal.Properties.TryGetValue("DTEND", out list))
            {
                DateTime? temp;
                var firstOrDefault = list.FirstOrDefault();
                if (firstOrDefault != null &&
                    ICalendar.Utils.Utils.ToDateTime(((IValue<string>)firstOrDefault).Value.ToLower(), out temp))
                    resource.DtEnd = temp.Value;

            }
            if (iCal.Properties.TryGetValue("UID", out list))
            {
                var firstOrDefault = list.FirstOrDefault();
                if (firstOrDefault != null) resource.Uid = ((IValue<string>)firstOrDefault).Value.ToLower();
            }
            //TODO: Take the resource Etag if the client send it if not assign one
            if (etag != null)
                resource.Etag = etag;
            else
            {
                //resource.Etag    
            }
            retEtag = resource.Etag;

            resource.FileName = calendarResource;

            resource.UserId = resource.User.UserId;

            //TODO: Recurrence figure out how to assign this
            //resource.Recurrence = 

            return resource;
        }
    }
}
