using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using CalDAV.Core.ConditionsCheck;
using CalDAV.Core.Propfind;
using DataLayer;
using CalDAV.Utils.XML_Processors;
using ICalendar.Calendar;
using ICalendar.GeneralInterfaces;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using TreeForXml;

namespace CalDAV.Core
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class CalDav : ICalDav
    {
        public CalDav(IFileSystemManagement fsManagement, CalDavContext _context)
        {
            StorageManagement = fsManagement;
            db = _context;
        }

        private IFileSystemManagement StorageManagement { get; }

        private IPropfindMethods PropFindMethods { get; set; }
        private IPrecondition PreconditionCheck { get; set; }
        private IPoscondition PosconditionCheck { get; set; }

        private IStartUp StartUp { get; set; }

        private CalDavContext db { get; }

        public KeyValuePair<HttpStatusCode, string> MkCalendar(Dictionary<string, string> propertiesAndHeaders, string body)
        {
            #region Extracting Properties
            string userEmail;
            propertiesAndHeaders.TryGetValue("userEmail", out userEmail);

            string collectionName;
            propertiesAndHeaders.TryGetValue("collectionName", out collectionName);

            string url;
            propertiesAndHeaders.TryGetValue("url", out url);
            #endregion

            propertiesAndHeaders.Add("body", body);

            PreconditionCheck = new MKCalendarPrecondition(StorageManagement, db);
            PosconditionCheck = new MKCalendarPosCondition(StorageManagement, db);

            //Checking that all precondition pass
            //If not the corresponding statusCode and error message are returned inside
            //the errorMessage property
            var errorMessage = new KeyValuePair<HttpStatusCode, string>();
            if (!PreconditionCheck.PreconditionsOK(propertiesAndHeaders, out errorMessage))
                return errorMessage;

            //If it has not body, it is created with default values.
            if (string.IsNullOrEmpty(body))
            {
                return CreateDefaultCalendar(propertiesAndHeaders, ref errorMessage);
            }

            //If a body exist the it is parsed like an XmlTree
            var mkCalendarTree = XmlTreeStructure.Parse(body);

            //if it does not have set property it is treated as a empty body.
            if (mkCalendarTree.Children.Count == 0)
            {
                return CreateDefaultCalendar(propertiesAndHeaders, ref errorMessage);
            }

            //now it is assumed that the body contains a set
            var setTree = mkCalendarTree.GetChild("set");

            #region Response Construction in case of error
            //this only if error during processing.
            //Creating and filling the root of the xml tree response
            //All response of a request is conformed by a "multistatus" element.
            var multistatus = new XmlTreeStructure("multistatus", "DAV:");
            multistatus.Namespaces.Add("D", "DAV:");
            multistatus.Namespaces.Add("C", "urn:ietf:params:xml:ns:caldav");

            var response = new XmlTreeStructure("response", "DAV:");
            multistatus.AddChild(response);

            var href = new XmlTreeStructure("href", "DAV:");
            href.AddValue("/api/v1/caldav/" + userEmail + "/calendars/" + collectionName + "/");
            #endregion
            //Check if any error occurred during body processing.
            var hasError = BuiltResponseForSet(userEmail, collectionName, null, false, setTree, response);

            if (hasError)
            {
                ChangeToDependencyError(response);
                //TODO: aki debe ser en realidad 207 multistatus pero no lo encuentro.
                return new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.Forbidden, multistatus.ToString());
            }

            db.SaveChanges();
            //StartUp.CreateCollectionForUser(propertiesAndHeaders["userEmail"], propertiesAndHeaders["collectionName"]);
            return new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.Created, null);
        }

        private KeyValuePair<HttpStatusCode, string> CreateDefaultCalendar(Dictionary<string, string> propertiesAndHeaders, ref KeyValuePair<HttpStatusCode, string> errorMessage)
        {
            #region Extracting Properties
            string userEmail;
            propertiesAndHeaders.TryGetValue("userEmail", out userEmail);

            string collectionName;
            propertiesAndHeaders.TryGetValue("collectionName", out collectionName);

            string url;
            propertiesAndHeaders.TryGetValue("url", out url);
            #endregion
            //Adding the collection to the database
            var user = db.GetUser(userEmail);
            var collection = new CalendarCollection { Name = collectionName, Url = url };
            user.CalendarCollections.Add(collection);

            //Adding the collection folder.
            StorageManagement.AddCalendarCollectionFolder(userEmail, collectionName);

            //Checking Preconditions   
            if (PosconditionCheck.PosconditionOk(propertiesAndHeaders, out errorMessage))
            {
                db.SaveChanges();
                return new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.Created, null);
            }


            return errorMessage;
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

            //Taking depth form headers.
            //Depth 0 means that it looks for prop only in the collection
            //Depth 1 means that it looks in their childs too.
            //And infinitum that looks in the entirely tree.
            var depth = -1;
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
            //All response are composed of a "multistatus" xml element
            //witch contains a "response" element for each collection and resource analized.
            //As a child of the "response" there is a list of "propstat". One for each different status obtained
            //trying to get the specified properties.
            //Inside every "propstatus" there are a xml element "prop" with all the properties that match with
            //the given "status" and a "status" xml containing the message of his "propstat".

            //Todo respuesta de propfind esta compuesta de un elemento xml "multistatus",
            //El cual contiene un elemento xml "response" por cada colleccion o recurso analizado.
            //Dentro de cada "response" hay una lista de "propstat", uno por cada status distinto obtenido
            //al intentar recobrar las propiedades especificadas.
            //Dentro de cada "propstatus" hay un xml "prop" con todas las propiedades que mapean con el
            //status correspondiente y un xml "status" que tiene el mensaje del estado de dicho "propstat". 
            var response = new XmlTreeStructure("multistatus", "DAV:");
            response.Namespaces.Add("D", "DAV:");
            response.Namespaces.Add("C", "urn:ietf:params:xml:ns:caldav");

            //Tool that contains the methods for propfind.
            PropFindMethods = new CalDavPropfind(db);

            //if the body is empty assume that is an allprop request.          
            if (body == null)
            {
                PropFindMethods.AllPropMethod(userEmail, collectionName, calendarResourceId, depth, null, response);

                return response;
            }

            //parsing the body into a xml tree
            var xmlTree = XmlTreeStructure.Parse(body);

            //Managing if the body was ok
            if (xmlTree.NodeName != "propfind")
                return null;

            //Finding the right method of propfind, it is found in the first child of the tree.
            //This methods take the response tree and they completed it with the necessary values and structure.
            var propType = xmlTree.Children[0];
            switch (propType.NodeName)
            {
                case "prop":
                    var props = ExtractPropertiesNameMainNS((XmlTreeStructure)xmlTree);
                    PropFindMethods.PropMethod(userEmail, collectionName, calendarResourceId, depth, props, response);
                    break;
                case "allprop":
                    var additionalProperties = ExtractIncludePropertiesNameMainNS((XmlTreeStructure)xmlTree);
                    PropFindMethods.AllPropMethod(userEmail, collectionName, calendarResourceId, depth,
                        additionalProperties, response);
                    break;
                case "propname":
                    PropFindMethods.PropNameMethod(userEmail, collectionName, calendarResourceId, depth, response);
                    break;
                default:
                    return null;
            }

            return response;
        }

        //TODO: Nacho
        public string Report(Dictionary<string, string> propertiesAndHeaders, string body)
        {
            throw new NotImplementedException();
        }

        #region Proppatch
        //TODO:Nacho
        public string PropPatch(Dictionary<string, string> propertiesAndHeaders, string body)
        {
            #region Docummentation
            //Proppatch is the method used by WebDAV for update, create and delete properties.

            //The body structure of a Proppatch request is declare as a "proppertyupdate" xml.

            //As a child of the "proppertyupdate" there are list of "set" and "remove" indicating the
            //operations that have to be process. This element have to be process in order (top to bottom).

            //There has to be at least one expected element inside "proppertyupdate".

            //Each "set" element is composed by a "prop" element witch contains the property name and value
            //of the properties that have to created or updated (if exists or not).

            //The same happens for the "remove" elements but these don't include the value of the property inside
            //the "prop" element. 
            #endregion

            #region Extracting Properties

            string userEmail;
            propertiesAndHeaders.TryGetValue("userEmail", out userEmail);

            string collectionName;
            propertiesAndHeaders.TryGetValue("collectionName", out collectionName);

            string calendarResourceId;
            propertiesAndHeaders.TryGetValue("calendarResourceID", out calendarResourceId);
            #endregion

            //Creating and filling the root of the xml tree response
            //All response of a request is conformed by a "multistatus" element.
            var multistatus = new XmlTreeStructure("multistatus", "DAV:");
            multistatus.Namespaces.Add("D", "DAV:");
            multistatus.Namespaces.Add("C", "urn:ietf:params:xml:ns:caldav");

            //getting the request body structure
            IXMLTreeStructure xmlTree;
            try
            {
                xmlTree = XmlTreeStructure.Parse(body);
            }
            catch (Exception)
            {
                throw;
            }


            //checking that the request has propertyupdate node

            if (xmlTree.NodeName != "propertyupdate")
                throw new ArgumentException(@"Body in bad format, body of proppatch must contain ""propertyupdate"" xml element");
            IXMLTreeStructure propertyupdate = xmlTree;
            //aliasing the list with all "set" and "remove" structures inside "propertyupdate".
            List<IXMLTreeStructure> setsAndRemoves = propertyupdate.Children;

            //propertyupdate must have at least one element
            if (setsAndRemoves.Count == 0)
                throw new ArgumentException("propertyupdate must have at least one element");

            //The structure of a response for a proppatch has a "multistatus"
            //as root inside it, there is only one response because depth is not allowed.
            //Inside the "response" is necessary to add a "propstat" for each property.
            //This "propstat" is built with a "prop" element containing just the property name 
            //and a "status" with the exit status code.
            var response = new XmlTreeStructure("response", "DAV:");
            multistatus.AddChild(response);

            #region Adding the <D:href>/api/v1/caldav/{userEmail}/calendars/{collectionName}/{calendarResourceId}?</D:href>
            var href = new XmlTreeStructure("href", "DAV:");

            if (calendarResourceId == null)
                href.AddValue("/api/v1/caldav/" + userEmail + "/calendars/" + collectionName + "/");
            else
                href.AddValue("/api/v1/caldav/" + userEmail + "/calendars/" + collectionName + "/" + calendarResourceId);

            response.AddChild(href);
            #endregion



            //Proppatch is atomic, though when an error occurred in one property,
            //all failed, an all other properties received a "424 failed dependency". 
            bool hasError = false;

            //Here it is garanted that if an error occured during the processing of the operations
            //The changes will not be stored in db thanks to a rollback.

            using (db)
            {
                //For each set and remove try to execute the operation if something fails 
                //put the Failed Dependency Error to every property before and after the error
                //even if the operation for the property was succesfully changed.
                foreach (var setOrRemove in setsAndRemoves)
                {
                    if (setOrRemove.NodeName == "set")
                        hasError = BuiltResponseForSet(userEmail, collectionName, calendarResourceId, hasError, setOrRemove, response);
                    else
                        hasError = BuiltResponseForRemove(userEmail, collectionName, calendarResourceId, hasError, setOrRemove, response);
                }

                if (hasError)
                {
                    ChangeToDependencyError(response);
                }
                else
                {
                    db.SaveChanges();
                }


            }

            return multistatus.ToString();
        }

        private void ChangeToDependencyError(XmlTreeStructure response)
        {
            //this method is the one in charge of fix the status messages 
            //of all the properties that were fine before the error.
            foreach (var child in response.Children)
            {
                if (child.NodeName != "propstat")
                    continue;

                var status = child.GetChild("status");
                var statMessage = status.Value;
                //if the message is not OK means that we reach
                //the error and no more further message changing is needed.
                if (statMessage != "HTTP/1.1 200 OK")
                    return;
                ((XmlTreeStructure)status).Value = "HTTP/1.1 424 Failed Dependency";
            }
        }

        private bool BuiltResponseForRemove(string userEmail, string collectionName, string calendarResourceId, bool errorOccurred, IXMLTreeStructure removeTree, IXMLTreeStructure response)
        {
            CalendarResource resource = null;
            CalendarCollection collection = null;
            if (calendarResourceId != null)
                resource = db.GetCalendarResource(userEmail, collectionName, calendarResourceId);
            else
                collection = db.GetCollection(userEmail, collectionName);

            //For each property it is tried to remove, if not possible change the error occured to true and
            //continue setting dependency error to the rest. 
            var prop = removeTree.GetChild("prop");
            var errorStack = new Stack<string>();
            foreach (var property in prop.Children)
            {
                //The structure for the response does not change.
                //It is constructed with a propstat and the value is never showed in the prop element.
                var propstat = new XmlTreeStructure("propstat", "DAV:");
                var stat = new XmlTreeStructure("status", "DAV:");
                var resProp = new XmlTreeStructure("prop", "DAV:");

                propstat.AddChild(stat);
                propstat.AddChild(resProp);
                resProp.AddChild(new XmlTreeStructure(property.NodeName, property.MainNamespace));
                response.AddChild(propstat);

                //If an error occurred previously the stat if 424 Failed Dependency.
                if (errorOccurred)
                    stat.Value = "HTTP/1.1 424 Failed Dependency";


                else
                {
                    //Try to remove the specified property, gets an error message from the stack in case of problems.
                    errorOccurred = !(resource?.RemoveProperty(property.NodeName, property.MainNamespace, errorStack) ?? collection.RemoveProperty(property.NodeName, property.MainNamespace, errorStack));
                    if (errorOccurred && errorStack.Count > 0)
                        stat.Value = errorStack.Pop();
                    else
                    {
                        stat.Value = "HTTP/1.1 200 OK";
                        // db.SaveChanges();
                    }


                }

            }
            return errorOccurred;
        }

        private bool BuiltResponseForSet(string userEmail, string collectionName, string calendarResourceId, bool errorOccurred, IXMLTreeStructure setTree, IXMLTreeStructure response)
        {
            CalendarResource resource = null;
            CalendarCollection collection = null;
            if (calendarResourceId != null)
                resource = db.GetCalendarResource(userEmail, collectionName, calendarResourceId);
            else
                collection = db.GetCollection(userEmail, collectionName);

            //For each property it is tried to remove, if not possible change the error occured to true and
            //continue setting dependency error to the rest. 
            var prop = setTree.GetChild("prop");
            var errorStack = new Stack<string>();
            foreach (var property in prop.Children)
            {
                var propstat = new XmlTreeStructure("propstat", "DAV:");
                var stat = new XmlTreeStructure("status", "DAV:");
                var resProp = new XmlTreeStructure("prop", "DAV:");

                resProp.AddChild(new XmlTreeStructure(property.NodeName, property.MainNamespace));
                propstat.AddChild(stat);
                propstat.AddChild(resProp);

                response.AddChild(propstat);

                if (errorOccurred)
                    stat.Value = "HTTP/1.1 424 Failed Dependency";

                else
                {
                    //Try to modify the specified property if it exist, if not try to create it
                    //gets an error message from the stack in case of problems.
                    errorOccurred = !(resource?.CreateOrModifyProperty(property.NodeName, property.MainNamespace, GetValueFromRealProperty(property), errorStack) ?? collection.CreateOrModifyProperty(property.NodeName, property.MainNamespace, GetValueFromRealProperty(property), errorStack));
                    if (errorOccurred && errorStack.Count > 0)
                        stat.Value = errorStack.Pop();
                    else
                    {
                        stat.Value = "HTTP/1.1 200 OK";
                        //db.SaveChanges();
                    }

                }

            }
            return errorOccurred;
        }

        /// <summary>
        /// This method only functionality is to take the string representation of a property without
        /// the first line, witch is the template for xml.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        private string GetValueFromRealProperty(IXMLTreeStructure property)
        {
            var temp = property.ToString();
            return temp.Replace(@"<?xml version=""1.0"" encoding=""utf-8""?>", "").TrimStart();
        }

        #endregion

        public bool DeleteCalendarObjectResource(Dictionary<string, string> propertiesAndHeaders)
        {
            var userEmail = propertiesAndHeaders["userEmail"];
            var collectionName = propertiesAndHeaders["collectionName"];
            var calendarResourceId = propertiesAndHeaders["calendarResourceId"];

            ///if the collection doesnt exist in the user folder
            /// the can't do anything
            if (!StorageManagement.SetUserAndCollection(userEmail, collectionName))
                return true;

            var resource =
                db.GetCollection(userEmail, collectionName)
                    .Calendarresources.First(x => x.Href == calendarResourceId);
            db.CalendarResources.Remove(resource);
            db.SaveChanges();

            return StorageManagement.DeleteCalendarObjectResource(calendarResourceId);
        }

        public bool DeleteCalendarCollection(Dictionary<string, string> propertiesAndHeaders)
        {
            var userEmail = propertiesAndHeaders["userEmail"];
            var collectionName = propertiesAndHeaders["collectionName"];
            var resourceId = propertiesAndHeaders["resourceId"];

            if (!StorageManagement.SetUserAndCollection(userEmail, collectionName))
                return false;


            var collection = db.GetCollection(userEmail, collectionName);
            if (collection == null)
                return false;
            db.CalendarCollections.Remove(collection);
            return StorageManagement.DeleteCalendarCollection();
        }

        public string ReadCalendarObjectResource(Dictionary<string, string> propertiesAndHeaders, out string etag)
        {
            var userEmail = propertiesAndHeaders["userEmail"];
            var collectionName = propertiesAndHeaders["collectionName"];
            var calendarResourceId = propertiesAndHeaders["calendarResourceId"];

            //Must return the Etag header of the COR

            var calendarRes = db.GetCalendarResource(userEmail, collectionName, calendarResourceId);
            etag = calendarRes.Properties.Where(x => x.Name == "getetag").SingleOrDefault().Value;

            return StorageManagement.GetCalendarObjectResource(calendarResourceId);
        }

        public string ReadCalendarCollection(Dictionary<string, string> propertiesAndHeaders)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        ///     Extract all property names and property namespace from a prop element of a  propfind body.
        /// </summary>
        /// <param name="propFindTree"></param>
        /// <returns></returns>
        private List<KeyValuePair<string, string>> ExtractPropertiesNameMainNS(IXMLTreeStructure propFindTree)
        {
            var retList = new List<KeyValuePair<string, string>>();
            IXMLTreeStructure props;

            if (propFindTree.GetChildAtAnyLevel("prop", out props))
                retList.AddRange(
                    props.Children.Select(child => new KeyValuePair<string, string>(child.NodeName, child.MainNamespace)));
            return retList;
        }

        /// <summary>
        ///     Extract all property names and property namespace from a include element of a  propfind body in the allproperty
        ///     method.
        /// </summary>
        /// <param name="propFindTree"></param>
        /// <returns></returns>
        private List<KeyValuePair<string, string>> ExtractIncludePropertiesNameMainNS(XmlTreeStructure propFindTree)
        {
            var retList = new List<KeyValuePair<string, string>>();
            IXMLTreeStructure includes;
            if (propFindTree.GetChildAtAnyLevel("include", out includes))
            {
                retList.AddRange(
                    includes.Children.Select(
                        child => new KeyValuePair<string, string>(child.NodeName, child.MainNamespace)));
            }
            return retList;
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

            ///if the collection doesnt exist in the user folder
            /// the can't do anything
            if (!StorageManagement.SetUserAndCollection(userEmail, collectionName))
            {
                retEtag = "";
                return false;
            }

            retEtag = null;

            //Note: calendar object resource = COR

            //CheckAllPreconditions
            var errorMessage = new KeyValuePair<HttpStatusCode, string>();
            PreconditionCheck = new PutPrecondition(StorageManagement, db);
            if (!PreconditionCheck.PreconditionsOK(propertiesAndHeaders, out errorMessage))
                return false;

            //etag value of "If-Match"
            string updateEtag;

            if (propertiesAndHeaders.TryGetValue("If-Match", out updateEtag))
            {
                //Taking etag from If-Match header.
                propertiesAndHeaders["etag"] = updateEtag;
                return UpdateCalendarObjectResource(propertiesAndHeaders, out retEtag);
            }
            if (propertiesAndHeaders.ContainsKey("If-Non-Match"))
            {
                return CreateCalendarObjectResource(propertiesAndHeaders, out retEtag);
            }
            //TODO: Trying to get db by dependency injection
            //update or create
            //using (var db = new CalDavContext())
            // {
            if (db.CalendarResourceExist(userEmail, collectionName, calendarResourceId) &&
                StorageManagement.ExistCalendarObjectResource(calendarResourceId))
            {
                return UpdateCalendarObjectResource(propertiesAndHeaders,
                    out retEtag);
            }
            return CreateCalendarObjectResource(propertiesAndHeaders, out retEtag);
            // }
            //return HTTP 201 Created 
        }

        /// <summary>
        ///     Creates a new COR from a PUT when a "If-Non-Match" header is included
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

            ///if the collection doesnt exist in the user folder
            /// the can't do anything
            if (!StorageManagement.SetUserAndCollection(userEmail, collectionName))
            {
                retEtag = "";
                return false;
            }

            TextReader reader = new StringReader(body);
            var iCal = new VCalendar(body);
            retEtag = etag;

            //TODO: Trying to get db by dependency injection
            //using (var db = new CalDavContext())
            //{
            if (!db.CollectionExist(userEmail, collectionName))
                return false;

            //TODO:Calculate Etag

            //filling the resource
            var resource = FillResource(propertiesAndHeaders, iCal, out retEtag);
            //adding the resource to the db
            db.CalendarResources.Add(resource);

            //adding the file
            StorageManagement.AddCalendarObjectResourceFile(calendarResourceId, body);


            return true;
            //}
        }

        /// <summary>
        ///     Updates an existing COR from a PUT when a "If-Match" header is included using the corresponding etag.
        /// </summary>
        /// <param name="propertiesAndHeaders"></param>
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

            ///if the collection doesn't exist in the user folder
            /// then can't do anything
            if (!StorageManagement.SetUserAndCollection(userEmail, collectionName))
            {
                retEtag = "";
                return false;
            }

            TextReader reader = new StringReader(body);
            var iCal = new VCalendar(body);

            retEtag = etag;

            //This means that the object in the body is not correct
            if (iCal == null)
                return false;

            //TODO: Trying to get db by dependency injection
            //  using (var db = new CalDavContext())
            // {
            if (!db.CollectionExist(userEmail, collectionName) ||
                !db.CalendarResourceExist(userEmail, collectionName, calendarResourceId))
                return false;

            //Fill the resource
            var resource = FillResource(propertiesAndHeaders, iCal, out retEtag);
            var prevResource = db.GetCalendarResource(userEmail, collectionName, calendarResourceId);
            int prevEtag;
            int actualEtag = 0;
            string tempEtag = "0";
            //TODO: Este metodo tiene que ser revisado por completo
            //var tempEtag = prevResource.Getetag;

            //TODO: esto siempre va a fallar pues los eTags no son int
            if (int.TryParse(tempEtag, out prevEtag) && int.TryParse(retEtag, out actualEtag))
            {
                if (actualEtag > prevEtag)
                {
                    if (resource.Uid != prevResource.Uid)
                        return false;
                    //Adding to the dataBase
                    db.CalendarResources.Update(resource);

                    //Removing old File 
                    StorageManagement.DeleteCalendarObjectResource(calendarResourceId);
                    //Adding New File
                    StorageManagement.AddCalendarObjectResourceFile(calendarResourceId, body);
                }
                else
                    retEtag = tempEtag;
            }
            else
                return false;

            // }
            return true;
        }

        /// <summary>
        ///     Method in charge of fill a CalendarResource and Return it.
        /// </summary>
        /// <param name="propertiesAndHeaders"></param>
        /// <param name="iCal"></param>
        /// <param name="retEtag"></param>
        /// <returns></returns>
        private CalendarResource FillResource(Dictionary<string, string> propertiesAndHeaders, VCalendar iCal,
            out string retEtag)
        {
            //TODO: Cambiar como se cogen las propiedades contruir como xml.

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

            ///if the collection doesnt exist in the user folder
            /// the can't do anything
            if (!StorageManagement.SetUserAndCollection(userEmail, collectionName))
            {
                retEtag = "";
                return null;
            }

            //TODO: aki estabas cogiendo las propiedades del VCALENDAR
            //para coger las del CalComponent tienes que buscarlo
            //en iCal.CalendarComponents y coger la q no es VTIMEZONE

            var resource = new CalendarResource();

            //TODO: Take the resource Etag if the client send it if not assign one
            if (etag != null)
                resource.Properties.Where(x => x.Name == "getetag").SingleOrDefault().Value = etag;
            retEtag = resource.Properties.Where(x => x.Name == "getetag").SingleOrDefault().Value;

            // resource.User = db.GetUser(userEmail);
            resource.Collection = db.GetCollection(userEmail, collectionName);
            IComponentProperty property;
            var calendarComp =
                iCal.CalendarComponents.Where(comp => comp.Key != "VTIMEZONE").FirstOrDefault().Value.FirstOrDefault();

            if (calendarComp == null)
                return null;

            property = calendarComp.GetComponentProperty("DTSTART");

            //if (property != null)
            //{
            //    //building as xml
            //    resource.DtStart = XMLBuilders.XmlBuilder("dtstart", "urn:ietf:params:xml:ns:caldav", ((IValue<DateTime>)property).Value.ToString()) ;
            //}
            //property = calendarComp.GetComponentProperty("DTEND");
            //if (property != null)
            //{
            //    resource.DtEnd = ((IValue<DateTime>)property).Value;
            //}
            property = calendarComp.GetComponentProperty("UID");
            if (property != null)
            {
                resource.Uid = ((IValue<string>)property).Value;
            }

            resource.Href = calendarResourceId;

            //resource.UserId = resource.User.UserId;

            //resource.ResourceType = calendarComp.Name;

            //TODO: Recurrence figure out how to assign this
            //resource.Recurrence =

            return resource;
        }

        #endregion
    }
}