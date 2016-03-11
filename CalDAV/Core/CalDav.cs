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
            return "";

        }

        public string PropFind(string userName, string collectionName,Stream body)
        {
            //REQUEST PROPERTIES
            //prop property return the value of the specified property
            //allprop property return the value of all properties --the include elemen makes the server return 
            //other properties that will not be returnes otherwise
            //getetag property return the etag of the COR for sincro options.
            throw new NotImplementedException();
        }


        public string Report(string username, string collectionName, Stream body)
        {
            throw new NotImplementedException();
        }

        public void AddCalendarObjectResource(string username, string collectionName, string resourceId, Stream body)
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

        public string PropPatch(string userName, string collectionName, Stream Body)
        {
            throw new NotImplementedException();
        }

        public bool DeleteCalendarObjectResource(string username, string collectionName, string resourceId)
        {
            using (var db = new CalDavContext())
            {
                var resource = db.GetCollection(username, collectionName).CalendarResources.First(x => x.FileName == resourceId);
                db.CalendarResources.Remove(resource);
                db.SaveChanges();
            }
            return StorageManagement.DeleteCalendarObjectResource(username, collectionName, resourceId);
        }

        public bool DeleteCalendarCollection(string username, string collectionName)
        {
            throw new NotImplementedException();
        }

        
        public string ReadCalendarObjectResource(string username, string collectionName, string resourceId)
        {
            //Must return the Etag header of the COR


            return "testing GET";
        }

        public string ReadCalendarCollection(string username, string collectionName)
        {
            throw new NotImplementedException();
        }

    }
}
