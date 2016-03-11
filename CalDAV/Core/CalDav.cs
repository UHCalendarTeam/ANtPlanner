using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalDAV.Models;
using CalDAV.Utils.XML_Processors;
using Microsoft.Data.Entity;

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

        public string MkCalendar(string user, string collection, string body)
        {
            var properties = XMLParsers.XMLMKCalendarParser(body);
            StartUp.CreateCollectionForUser(user, collection);
            return "";

        }

        public string PropFind(string userEmail, string collectionName, Stream body)
        {
            //REQUEST PROPERTIES
            //prop property return the value of the specified property
            //allprop property return the value of all properties --the include elemen makes the server return 
            //other properties that will not be returnes otherwise
            //getetag property return the etag of the COR for sincro options.
            throw new NotImplementedException();
        }


        public string Report(string userEmail, string collectionName, Stream body)
        {
            throw new NotImplementedException();
        }

        public void AddCalendarObjectResource(string userEmail, string collectionName, string resourceId, Stream body)
        {
            //Note: calendar object resource = COR

            //check that resourceId don't exist but the collection does.
            //other case it updates the specified COR.
            //if the client intend to create (not update) a new COR it SHOULD add a HTTP header "If-None-Match : *" 
            // it secures that it will not update an COR in case that the id already exist without notice the client.
            //for an update use "If-Match" and the specific Etag
            //when created or updated the response header MUST have the Etag

            //body is a clean(without XML) iCalendar

            //return HTTP 201 Created 

            throw new NotImplementedException();
        }

        public string PropPatch(string userEmail, string collectionName, Stream Body)
        {
            throw new NotImplementedException();
        }

        public bool DeleteCalendarObjectResource(string userEmail, string collectionName, string resourceId)
        {
            using (var db = new CalDavContext())
            {
                var resource = db.GetCollection(userEmail, collectionName).CalendarResources.First(x => x.FileName == resourceId);
                db.CalendarResources.Remove(resource);
                db.SaveChanges();
            }
            return StorageManagement.DeleteCalendarObjectResource(userEmail, collectionName, resourceId);
        }

        public bool DeleteCalendarCollection(string userEmail, string collectionName)
        {
            using (var db = new CalDavContext())
            {
                var collection = db.GetCollection(userEmail, collectionName);
                if (collection == null)
                    return false;
                db.CalendarCollections.Remove(collection);
                return StorageManagement.DeleteCalendarCollection(userEmail, collectionName);


            }


        }


        public string ReadCalendarObjectResource(string userEmail, string collectionName, string resourceId, out string etag)
        {
            //Must return the Etag header of the COR

            using (var db = new CalDavContext())
            {
                var calendarRes = db.GetCalendarResource(userEmail, collectionName, resourceId);
                etag = calendarRes.Etag;
            }
            return StorageManagement.GetCalendarObjectResource(userEmail, collectionName, resourceId);
        }

        public string ReadCalendarCollection(string userEmail, string collectionName)
        {
            throw new NotImplementedException();
        }

    }
}
