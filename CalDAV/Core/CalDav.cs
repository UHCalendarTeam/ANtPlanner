using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CalDAV.Core.ConditionsCheck;
using CalDAV.Core.Propfind;
using CalDAV.Models;
using CalDAV.Utils.XML_Processors;

using ICalendar.Calendar;
using ICalendar.GeneralInterfaces;
using TreeForXml;

namespace CalDAV.Core
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class CalDav : ICalDav
    {
        private IFileSystemManagement StorageManagement { get; }

        private IPropfindMethods PropFindMethods { get; set; }

        private IPrecondition preconditionCheck { get; set; }
        private IPostcondition postconditionCheck { get; }

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

        //TODO: Nacho
        public XmlTreeStructure PropFind(Dictionary<string, string> propertiesAndHeaders, string body)
        {
            #region Extracting Properties
            string userEmail;
            propertiesAndHeaders.TryGetValue("userEmail", out userEmail);

            string collectionName;
            propertiesAndHeaders.TryGetValue("collectionName", out collectionName);

            string calendarResourceId;
            propertiesAndHeaders.TryGetValue("calendarResourceID", out calendarResourceId);

            //Manage the depth as nullable.
            int depth = -1;
            string strDepth;
            propertiesAndHeaders.TryGetValue("depth", out strDepth);
            try
            {
                depth = int.Parse(strDepth);
            }
            catch (Exception)
            {
                depth = -1;
            }


            #endregion

            //Creating and filling the root of the xml tree response
            XmlTreeStructure response = new XmlTreeStructure("multistatus","DAV:");
            response.Namespaces.Add("D", "DAV:");
            response.Namespaces.Add("C", "urn:ietf:params:xml:ns:caldav");

            //Tool that contains the methods for propfind.
            PropFindMethods = new CalDavPropfind();

            //if the body is empty asumme that is an allprop request.          
            if (body == null)
            {
                PropFindMethods.AllPropMethod(userEmail, collectionName, calendarResourceId, depth, response);

                return response;
            }

            var xmlTree = XmlTreeStructure.Parse(body);

            if (xmlTree.NodeName != "propfind")
                return null;

            //en cada object resource devolver el etag
            var propType = xmlTree.Children[0];
            switch (propType.NodeName)
            {
                case "prop":
                    if (calendarResourceId != null)
                    {
                        PropFindMethods.PropObjectResource(userEmail, collectionName, calendarResourceId, (XmlTreeStructure)propType, response);
                    }
                    else
                        PropFindMethods.PropMethod(userEmail, collectionName, depth, (XmlTreeStructure)propType, response);
                    break;
                case "allprop":
                    PropFindMethods.AllPropMethod(userEmail, collectionName, calendarResourceId, depth, response);
                    break;
                case "propname":
                    if (calendarResourceId != null)
                    {
                        PropFindMethods.PropNameObjectResource(userEmail, collectionName, calendarResourceId, response);
                    }
                    else
                        PropFindMethods.PropNameMethod(userEmail, collectionName, depth, response);
                    break;
                default:
                    return null;
            }

            //REQUEST PROPERTIES
            //allprop --the include element makes the server return other properties that will not be returns otherwise
            //getetag property return the etag of the COR for sincro options.

            return response;
        }

        //TODO: Nacho
        public string Report(Dictionary<string, string> propertiesAndHeaders, string body)
        {
            throw new NotImplementedException();
        }

        #region PUT resource
        //TODO: Nacho
        public bool AddCalendarObjectResource(Dictionary<string, string> propertiesAndHeaders, out string retEtag)
        {
            #region Extracting Properties

            string userEmail;
            propertiesAndHeaders.TryGetValue("userEmail", out userEmail);

            string collectionName;
            propertiesAndHeaders.TryGetValue("collectionName", out collectionName);

            string calendarResourceId;
            propertiesAndHeaders.TryGetValue("calendarResourceID", out calendarResourceId);

            string etag;
            propertiesAndHeaders.TryGetValue("etag", out etag);

            string body;
            propertiesAndHeaders.TryGetValue("body", out body);
            #endregion

            retEtag = null;

            //Note: calendar object resource = COR

            //CheckAllPreconditions
            preconditionCheck = new PutPrecondition(StorageManagement);
            if (!preconditionCheck.PreconditionsOK(propertiesAndHeaders))
                return false;

            //etag value of "If-Match"
            string updateEtag;

            if (propertiesAndHeaders.TryGetValue("If-Match", out updateEtag))
            {
                //Taking etag from If-Match header.
                propertiesAndHeaders["etag"] = updateEtag;
                return UpdateCalendarObjectResource(propertiesAndHeaders, out retEtag);
            }
            else if (propertiesAndHeaders.ContainsKey("If-Non-Match"))
            {
                return CreateCalendarObjectResource(propertiesAndHeaders, out retEtag);
            }
            else
            {
                //update or create
                using (var db = new CalDavContext())
                {
                    if (db.CalendarResourceExist(userEmail, collectionName, calendarResourceId) &&
                        StorageManagement.ExistCalendarObjectResource(userEmail, collectionName, calendarResourceId))
                    {
                        return UpdateCalendarObjectResource(propertiesAndHeaders,
                            out retEtag);
                    }
                    return CreateCalendarObjectResource(propertiesAndHeaders, out retEtag);
                }

            }
            //return HTTP 201 Created 
        }

        /// <summary>
        /// Creates a new COR from a PUT when a "If-Non-Match" header is included
        /// </summary>
        /// <param name="propertiesAndHeaders"></param>
        /// <param name="retEtag"></param>
        /// <param></param>
        private bool CreateCalendarObjectResource(Dictionary<string, string> propertiesAndHeaders, out string retEtag)
        {
            #region Extracting Properties
            string userEmail;
            propertiesAndHeaders.TryGetValue("userEmail", out userEmail);

            string collectionName;
            propertiesAndHeaders.TryGetValue("collectionName", out collectionName);

            string calendarResourceId;
            propertiesAndHeaders.TryGetValue("calendarResourceID", out calendarResourceId);

            string etag;
            propertiesAndHeaders.TryGetValue("etag", out etag);

            string body;
            propertiesAndHeaders.TryGetValue("body", out body);
            #endregion


            TextReader reader = new StringReader(body);
            var iCal = new VCalendar(body);
            retEtag = etag;

            using (var db = new CalDavContext())
            {
                if (!db.CollectionExist(userEmail, collectionName))
                    return false;

                //TODO:Calculate Etag

                //filling the resource
                var resource = FillResource(propertiesAndHeaders, db, iCal, out retEtag);
                //adding the resource to the db
                db.CalendarResources.Add(resource);

                //adding the file
                StorageManagement.AddCalendarObjectResourceFile(userEmail, collectionName, calendarResourceId, body);


                return true;
            }

        }

        /// <summary>
        /// Updates an existing COR from a PUT when a "If-Match" header is included using the corresponding etag.
        /// </summary>
        ///<param name="propertiesAndHeaders"></param>
        /// <param name="retEtag"></param>
        private bool UpdateCalendarObjectResource(Dictionary<string, string> propertiesAndHeaders, out string retEtag)
        {
            #region Extracting Properties
            string userEmail;
            propertiesAndHeaders.TryGetValue("userEmail", out userEmail);

            string collectionName;
            propertiesAndHeaders.TryGetValue("collectionName", out collectionName);

            string calendarResourceId;
            propertiesAndHeaders.TryGetValue("calendarResourceID", out calendarResourceId);

            string etag;
            propertiesAndHeaders.TryGetValue("etag", out etag);

            string body;
            propertiesAndHeaders.TryGetValue("body", out body);
            #endregion

            TextReader reader = new StringReader(body);
            var iCal = new VCalendar(body);

            retEtag = etag;

            //This means that the object in the body is not correct
            if (iCal == null)
                return false;

            using (var db = new CalDavContext())
            {
                if (!db.CollectionExist(userEmail, collectionName) || !db.CalendarResourceExist(userEmail, collectionName, calendarResourceId))
                    return false;

                //Fill the resource
                var resource = FillResource(propertiesAndHeaders, db, iCal, out retEtag);
                var prevResource = db.GetCalendarResource(userEmail, collectionName, calendarResourceId);
                int prevEtag;
                int actualEtag;
                string tempEtag = prevResource.GetEtag;
                if (int.TryParse(tempEtag, out prevEtag) && int.TryParse(retEtag, out actualEtag))
                {
                    if (actualEtag > prevEtag)
                    {
                        if (resource.Uid != prevResource.Uid)
                            return false;
                        //Adding to the dataBase
                        db.CalendarResources.Update(resource);

                        //Removing old File 
                        StorageManagement.DeleteCalendarObjectResource(userEmail, collectionName, calendarResourceId);
                        //Adding New File
                        StorageManagement.AddCalendarObjectResourceFile(userEmail, collectionName, calendarResourceId, body);

                    }
                    else
                        retEtag = tempEtag;
                }
                else
                    return false;

            }
            return true;
        }

        /// <summary>
        /// Method in charge of fill a CalendarResource and Return it.
        /// </summary>
        /// <param name="propertiesAndHeaders"></param>
        /// <param name="db"></param>
        /// <param name="iCal"></param>
        /// <param name="retEtag"></param>
        /// <returns></returns>
        private CalendarResource FillResource(Dictionary<string, string> propertiesAndHeaders, CalDavContext db, VCalendar iCal, out string retEtag)
        {
            #region Extracting Properties
            string userEmail;
            propertiesAndHeaders.TryGetValue("userEmail", out userEmail);

            string collectionName;
            propertiesAndHeaders.TryGetValue("collectionName", out collectionName);

            string calendarResourceId;
            propertiesAndHeaders.TryGetValue("calendarResourceID", out calendarResourceId);

            string etag;
            propertiesAndHeaders.TryGetValue("etag", out etag);
            #endregion

            //TODO: aki estabas cogiendo las propiedades del VCALENDAR
            //para coger las del CalComponent tienes que buscarlo
            //en iCal.CalendarComponents y coger la q no es VTIMEZONE

            CalendarResource resource = new CalendarResource();

            //TODO: Take the resource Etag if the client send it if not assign one
            if (etag != null)
                resource.GetEtag = etag;
            else
            {
                //resource.Etag    
            }
            retEtag = resource.GetEtag;

            resource.User = db.GetUser(userEmail);
            resource.Collection = db.GetCollection(userEmail, collectionName);
            IComponentProperty property;
            var calendarComp = iCal.CalendarComponents.Where(comp => comp.Key != "VTIMEZONE").FirstOrDefault().Value.FirstOrDefault();

            if (calendarComp == null)
                return null;

            property = calendarComp.GetComponentProperty("DTSTART");

            if (property != null)
            {
                resource.DtStart = ((IValue<DateTime>)property).Value;
            }
            property = calendarComp.GetComponentProperty("DTEND");
            if (property != null)
            {
                resource.DtEnd = ((IValue<DateTime>)property).Value;
            }
            property = calendarComp.GetComponentProperty("UID");
            if (property != null)
            {
                resource.Uid = ((IValue<string>)property).Value;
            }

            resource.FileName = calendarResourceId;

            resource.UserId = resource.User.UserId;

            resource.ResourceType = calendarComp.Name;

            //TODO: Recurrence figure out how to assign this
            //resource.Recurrence =

            return resource;
        }
        #endregion

        //TODO:Nacho
        public string PropPatch(Dictionary<string, string> propertiesAndHeaders, string Body)
        {
            throw new NotImplementedException();
        }

        public bool DeleteCalendarObjectResource(Dictionary<string, string> propertiesAndHeaders)
        {
            var userEmail = propertiesAndHeaders["userEmail"];
            var collectionName = propertiesAndHeaders["collectionName"];
            var calendarResourceId = propertiesAndHeaders["calendarResourceId"];
            using (var db = new CalDavContext())
            {
                var resource = db.GetCollection(userEmail, collectionName).CalendarResources.First(x => x.FileName == calendarResourceId);
                db.CalendarResources.Remove(resource);
                db.SaveChanges();
            }
            return StorageManagement.DeleteCalendarObjectResource(userEmail, collectionName, calendarResourceId);
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
            var calendarResourceId = propertiesAndHeaders["calendarResourceId"];

            //Must return the Etag header of the COR

            using (var db = new CalDavContext())
            {
                var calendarRes = db.GetCalendarResource(userEmail, collectionName, calendarResourceId);
                etag = calendarRes.GetEtag;
            }
            return StorageManagement.GetCalendarObjectResource(userEmail, collectionName, calendarResourceId);
        }

        public string ReadCalendarCollection(Dictionary<string, string> propertiesAndHeaders)
        {
            throw new NotImplementedException();
        }

    }
}
