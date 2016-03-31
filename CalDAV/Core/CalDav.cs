using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public CalDav(IFileSystemManagement fsManagement, CalDavContext _context)
        {
            StorageManagement = fsManagement;
            db = _context;
        }

        private IFileSystemManagement StorageManagement { get; }

        private IPropfindMethods PropFindMethods { get; set; }
        private IPrecondition PreconditionCheck { get; set; }
        private IPostcondition postconditionCheck { get; }

        private IStartUp StartUp { get; set; }

        private CalDavContext db { get; }

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

        //TODO:Nacho
        public string PropPatch(Dictionary<string, string> propertiesAndHeaders, string body)
        {
            #region Extracting Properties

            string userEmail;
            propertiesAndHeaders.TryGetValue("userEmail", out userEmail);

            string collectionName;
            propertiesAndHeaders.TryGetValue("collectionName", out collectionName);

            string calendarResourceId;
            propertiesAndHeaders.TryGetValue("calendarResourceID", out calendarResourceId);
            #endregion

            //Creating and filling the root of the xml tree response
            var response = new XmlTreeStructure("multistatus", "DAV:");
            response.Namespaces.Add("D", "DAV:");
            response.Namespaces.Add("C", "urn:ietf:params:xml:ns:caldav");

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

            IXMLTreeStructure propertyupdate;
            if (!xmlTree.GetChildAtAnyLevel("propertyupdate", out propertyupdate))
                throw new ArgumentException(@"Body in bad format, body of proppatch must contain ""propertyupdate"" xml element");

            List<IXMLTreeStructure> setsAndRemoves = propertyupdate.Children;

            //propertyupdate must have at least one element
            if(setsAndRemoves.Count==0)
                throw new ArgumentException("propertyupdate must have at least one element");


            throw new NotImplementedException();
        }

        

        public bool DeleteCalendarObjectResource(Dictionary<string, string> propertiesAndHeaders)
        {
            var userEmail = propertiesAndHeaders["userEmail"];
            var collectionName = propertiesAndHeaders["collectionName"];
            var calendarResourceId = propertiesAndHeaders["calendarResourceId"];

            ///if the collection doesnt exist in the user folder
            /// the can't do anything
            if (!StorageManagement.SetUserAndCollection(userEmail, collectionName))
                return true;

            //TODO: Trying to get db by dependency injection
            //using (var db = new CalDavContext())
            // {
            var resource =
                db.GetCollection(userEmail, collectionName)
                    .Calendarresources.First(x => x.FileName == calendarResourceId);
            db.CalendarResources.Remove(resource);
            db.SaveChanges();
            //  }
            return StorageManagement.DeleteCalendarObjectResource(calendarResourceId);
        }

        public bool DeleteCalendarCollection(Dictionary<string, string> propertiesAndHeaders)
        {
            var userEmail = propertiesAndHeaders["userEmail"];
            var collectionName = propertiesAndHeaders["collectionName"];
            var resourceId = propertiesAndHeaders["resourceId"];

            if (!StorageManagement.SetUserAndCollection(userEmail, collectionName))
                return false;

            //TODO: Trying to get db by dependency injection
            //  using (var db = new CalDavContext())
            //{
            var collection = db.GetCollection(userEmail, collectionName);
            if (collection == null)
                return false;
            db.CalendarCollections.Remove(collection);
            return StorageManagement.DeleteCalendarCollection();


            //  }
        }

        public string ReadCalendarObjectResource(Dictionary<string, string> propertiesAndHeaders, out string etag)
        {
            var userEmail = propertiesAndHeaders["userEmail"];
            var collectionName = propertiesAndHeaders["collectionName"];
            var calendarResourceId = propertiesAndHeaders["calendarResourceId"];

            //Must return the Etag header of the COR
            //TODO: Trying to get db by dependency injection
            // using (var db = new CalDavContext())
            //{
            var calendarRes = db.GetCalendarResource(userEmail, collectionName, calendarResourceId);
            etag = calendarRes.Getetag;
            // }
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
            PreconditionCheck = new PutPrecondition(StorageManagement);
            if (!PreconditionCheck.PreconditionsOK(propertiesAndHeaders))
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
            int actualEtag;
            var tempEtag = prevResource.Getetag;

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
                resource.Getetag = etag;
            retEtag = resource.Getetag;

            resource.User = db.GetUser(userEmail);
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

            resource.FileName = calendarResourceId;

            resource.UserId = resource.User.UserId;

            //resource.ResourceType = calendarComp.Name;

            //TODO: Recurrence figure out how to assign this
            //resource.Recurrence =

            return resource;
        }

        #endregion
    }
}