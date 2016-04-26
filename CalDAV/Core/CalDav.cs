using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ACL.Core;
using CalDAV.Core.ConditionsCheck;
using CalDAV.Core.Method_Extensions;
using CalDAV.Core.Propfind;
using DataLayer;
using DataLayer.Models.Entities;
using DataLayer.Models.Method_Extensions;
using ICalendar.Calendar;
using Microsoft.AspNet.Http;
using Microsoft.Net.Http.Headers;
using TreeForXml;

namespace CalDAV.Core
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class CalDav : ICalDav
    {
        #region Standard Namespace
        private readonly Dictionary<string, string> Namespaces = new Dictionary<string, string>
        {
            {"D", @"xmlns:D=""DAV:"""},
            {"C", @"xmlns:C=""urn:ietf:params:xml:ns:caldav"""}
        };

        private readonly Dictionary<string, string> NamespacesSimple = new Dictionary<string, string>
        {
            {"D", "DAV:"},
            {"C", "urn:ietf:params:xml:ns:caldav"}
        };
        #endregion

        private readonly IACLProfind _aclProfind;
        private readonly ICollectionReport _colectionCollectionReport;

        /// <summary>
        ///     DI in the params.
        /// </summary>
        /// <param name="fsManagement"></param>
        /// <param name="_context"></param>
        /// <param name="aclProfind"></param>
        public CalDav(IFileSystemManagement fsManagement, CalDavContext _context,
            IACLProfind aclProfind, ICollectionReport collectionCollectionReport)
        {
            StorageManagement = fsManagement;
            db = _context;
            _aclProfind = aclProfind;
            _colectionCollectionReport = collectionCollectionReport;
        }

        #region Dependencies
        private IFileSystemManagement StorageManagement { get; }

        private IPropfindMethods PropFindMethods { get; set; }
        private IPrecondition PreconditionCheck { get; set; }
        private IPoscondition PosconditionCheck { get; set; }

        private IStartUp StartUp { get; set; }

        private CalDavContext db { get; }

        #endregion

        //TODO: Adriano
        public async Task Report(HttpContext httpContext)
        {
            await _colectionCollectionReport.ProcessRequest(httpContext);
        }

        #region PROPFIND methods

        //TODO: Nacho
        /// <summary>
        ///     This PROFIND is used for the collection and the resources.
        /// </summary>
        /// <param name="propertiesAndHeaders">Put here: resourceURL, depth, calendarResourceId</param>
        /// <param name="body">The request body from the client.</param>
        /// <param name="response">
        ///     The Response property in the controller. We fill up the response object
        ///     with out response.
        /// </param>
        public void PropFind(Dictionary<string, string> propertiesAndHeaders, string body, HttpResponse response)
        {
            #region Extracting Properties

            string calendarResourceId;
            propertiesAndHeaders.TryGetValue("calendarResourceID", out calendarResourceId);

            string url;
            propertiesAndHeaders.TryGetValue("url", out url);

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
            //witch contains a "response" element for each collection and resource analized witch url is included in a "href" element as a child of "response".
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

            response.StatusCode = 207;
            response.ContentType = "application/xml";

            var responseTree = new XmlTreeStructure("multistatus", "DAV:");
            responseTree.Namespaces.Add("D", "DAV:");
            responseTree.Namespaces.Add("C", "urn:ietf:params:xml:ns:caldav");

            //Tool that contains the methods for propfind.
            PropFindMethods = new CalDavPropfind(db);

            //if the body is empty assume that is an allprop request.          
            if (string.IsNullOrEmpty(body))
            {
                PropFindMethods.AllPropMethod(url, calendarResourceId, depth, null, responseTree);

                response.Body.Write(responseTree.ToString());
                return;
            }

            //parsing the body into a xml tree
            var xmlTree = XmlTreeStructure.Parse(body);

            //Managing if the body was ok
            if (xmlTree.NodeName != "propfind")
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }

            //Finding the right method of propfind, it is found in the first child of the tree.
            //This methods take the response tree and they completed it with the necessary values and structure.
            var propType = xmlTree.Children[0];
            switch (propType.NodeName)
            {
                case "prop":
                    var props = ExtractPropertiesNameMainNS((XmlTreeStructure)xmlTree);
                    PropFindMethods.PropMethod(url, calendarResourceId, depth, props, responseTree);
                    break;
                case "allprop":
                    var additionalProperties = ExtractIncludePropertiesNameMainNS((XmlTreeStructure)xmlTree);
                    PropFindMethods.AllPropMethod(url, calendarResourceId,
                        depth, additionalProperties, responseTree);
                    break;
                case "propname":
                    PropFindMethods.PropNameMethod(url, calendarResourceId, depth, responseTree);
                    break;
                default:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return;
            }
            string stringResp = responseTree.ToString();
            response.Body.Write(stringResp);
        }

        /// <summary>
        ///     This method perfoms a profind on a principal.
        /// </summary>
        /// <param name="request">The request from the controller.</param>
        /// <param name="response">The response from the controller.</param>
        /// <param name="data">SOme useful data that could be send from the controller.</param>
        /// <returns></returns>
        public async Task ACLProfind(HttpRequest request, HttpResponse response, Dictionary<string, string> data = null)
        {
            await _aclProfind.Profind(request, response, data);
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
                    props.Children.Select(
                        child =>
                            new KeyValuePair<string, string>(child.NodeName,
                                string.IsNullOrEmpty(child.MainNamespace) ? "DAV:" : child.MainNamespace)));
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

        #endregion

        #region MkCalendar Methods

        public async Task MkCalendar(Dictionary<string, string> propertiesAndHeaders, string body, HttpResponse response)
        {
            #region Extracting Properties

            string principalId;
            propertiesAndHeaders.TryGetValue("principalId", out principalId);

            string url;
            propertiesAndHeaders.TryGetValue("url", out url);

            #endregion

            propertiesAndHeaders.Add("body", body);

            PreconditionCheck = new MKCalendarPrecondition(StorageManagement, db);
            PosconditionCheck = new MKCalendarPosCondition(StorageManagement, db);

            //Checking that all precondition pass

            //Cheking Preconditions
            if (!PreconditionCheck.PreconditionsOK(propertiesAndHeaders, response))
                return;

            //I create here the collection already but i wait for other comprobations before save the database.
            CreateDefaultCalendar(propertiesAndHeaders);
            response.StatusCode = (int)HttpStatusCode.Created;

            //If it has not body and  Posconditions are OK, it is created with default values.
            if (string.IsNullOrEmpty(body))
            {
                if (!PosconditionCheck.PosconditionOk(propertiesAndHeaders, response))
                {
                    DeleteCalendarCollection(propertiesAndHeaders, response);
                    response.StatusCode = (int)HttpStatusCode.Forbidden;
                    await response.WriteAsync("Poscondition Failed");
                    return;
                    //return new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.Forbidden, "Poscondition Failed");
                }


                db.SaveChanges();
                return;
            }

            //If a body exist the it is parsed like an XmlTree
            var mkCalendarTree = XmlTreeStructure.Parse(body);

            //if it does not have set property it is treated as a empty body.
            if (mkCalendarTree.Children.Count == 0)
            {
                if (!PosconditionCheck.PosconditionOk(propertiesAndHeaders, response))
                {
                    DeleteCalendarCollection(propertiesAndHeaders, response);
                    response.StatusCode = (int)HttpStatusCode.Forbidden;
                    await response.WriteAsync("Poscondition Failed");

                    return;
                }


                db.SaveChanges();
                return;
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

            var responseTree = new XmlTreeStructure("response", "DAV:");
            multistatus.AddChild(responseTree);

            var href = new XmlTreeStructure("href", "DAV:");
            href.AddValue(SystemProperties._baseUrl + url);

            #endregion

            //Check if any error occurred during body processing.
            var hasError = BuiltResponseForSet(url, null, false, setTree, responseTree);

            if (hasError)
            {
                DeleteCalendarCollection(propertiesAndHeaders, response);
                response.ContentType = "application/xml";

                ChangeToDependencyError(responseTree);

                response.StatusCode = 207;
                await response.WriteAsync(multistatus.ToString());
                return;
            }

            //Checking Preconditions   
            if (PosconditionCheck.PosconditionOk(propertiesAndHeaders, response))
            {
                db.SaveChanges();
                return;
                // return createdMessage;
            }

            DeleteCalendarCollection(propertiesAndHeaders, response);
            response.StatusCode = (int)HttpStatusCode.Forbidden;
            await response.WriteAsync("Poscondition Failed");


            //return new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.Forbidden, "Poscondition Failed");
        }

        private void CreateDefaultCalendar(Dictionary<string, string> propertiesAndHeaders)
        {
            #region Extracting Properties

            string principalId;
            propertiesAndHeaders.TryGetValue("principalId", out principalId);

            string collectionName;
            propertiesAndHeaders.TryGetValue("collectionName", out collectionName);

            string url;
            propertiesAndHeaders.TryGetValue("url", out url);

            #endregion

            //Adding the collection to the database

            var principal = db.GetPrincipalByName(principalId);
            var collection = new CalendarCollection(url, collectionName);
            var stack = new Stack<string>();
            collection.CreateOrModifyProperty("getctag", "http://calendarserver.org/ns/",
                new XmlTreeStructure("getctag", @"xmlns=""http://calendarserver.org/ns/""") { Value = Guid.NewGuid().ToString() }.ToString(), stack);
            principal.CalendarCollections.Add(collection);

            //Adding the collection folder.
            StorageManagement.AddCalendarCollectionFolder(url);
            db.SaveChanges();
        }

        #endregion

        #region Proppatch

        //TODO:Nacho
        public void PropPatch(Dictionary<string, string> propertiesAndHeaders, string body, HttpResponse response)
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

            string calendarResourceId;
            propertiesAndHeaders.TryGetValue("calendarResourceID", out calendarResourceId);

            string url;
            propertiesAndHeaders.TryGetValue("url", out url);

            #endregion

            //Creating and filling the root of the xml tree response
            //All response of a request is conformed by a "multistatus" element.
            var multistatus = new XmlTreeStructure("multistatus", "DAV:");
            multistatus.Namespaces.Add("D", "DAV:");
            multistatus.Namespaces.Add("C", "urn:ietf:params:xml:ns:caldav");

            response.ContentType = "application/xml";

            //getting the request body structure
            IXMLTreeStructure xmlTree;
            try
            {
                xmlTree = XmlTreeStructure.Parse(body);
            }
            catch (Exception)
            {
                response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }


            //checking that the request has propertyupdate node

            if (xmlTree.NodeName != "propertyupdate")
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Body.Write(@"Body in bad format, body of proppatch must contain ""propertyupdate"" xml element");
                return;
            }

            //throw new ArgumentException(@"Body in bad format, body of proppatch must contain ""propertyupdate"" xml element");

            var propertyupdate = xmlTree;
            //aliasing the list with all "set" and "remove" structures inside "propertyupdate".
            var setsAndRemoves = propertyupdate.Children;

            //propertyupdate must have at least one element
            if (setsAndRemoves.Count == 0)
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Body.Write("propertyupdate must have at least one element");
                return;
                //throw new ArgumentException("propertyupdate must have at least one element");
            }


            //The structure of a response for a proppatch has a "multistatus"
            //as root inside it, there is only one response because depth is not allowed.
            //Inside the "response" is necessary to add a "propstat" for each property.
            //This "propstat" is built with a "prop" element containing just the property name 
            //and a "status" with the exit status code.
            var responseTree = new XmlTreeStructure("response", "DAV:");
            multistatus.AddChild(responseTree);

            #region Adding the <D:href>/api/v1/collections/{userEmail}|{groupName}/{principalId}/{collectionName}/{calendarResourceId}?</D:href>

            var href = new XmlTreeStructure("href", "DAV:");

            href.AddValue(SystemProperties._baseUrl + url);


            responseTree.AddChild(href);

            #endregion

            //Proppatch is atomic, though when an error occurred in one property,
            //all failed, an all other properties received a "424 failed dependency". 
            var hasError = false;

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
                        hasError = BuiltResponseForSet(url, calendarResourceId, hasError,
                            setOrRemove, responseTree);
                    else
                        hasError = BuiltResponseForRemove(url, calendarResourceId, hasError,
                            setOrRemove, responseTree);
                }

                if (hasError)
                {
                    ChangeToDependencyError(responseTree);
                }
                else
                {
                    db.SaveChanges();
                }
            }
            response.StatusCode = 207;
            response.Body.Write(multistatus.ToString());
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

        private bool BuiltResponseForRemove(string url, string calendarResourceId,
            bool errorOccurred, IXMLTreeStructure removeTree, IXMLTreeStructure response)
        {
            CalendarResource resource = null;
            CalendarCollection collection = null;
            if (calendarResourceId != null)
                resource = db.GetCalendarResource(url);
            else
                collection = db.GetCollection(url);

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
                    errorOccurred =
                        !(resource?.RemoveProperty(property.NodeName, property.MainNamespace, errorStack) ??
                          collection.RemoveProperty(property.NodeName, property.MainNamespace, errorStack));
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

        private bool BuiltResponseForSet(string url, string calendarResourceId,
            bool errorOccurred, IXMLTreeStructure setTree, IXMLTreeStructure response)
        {
            CalendarResource resource = null;
            CalendarCollection collection = null;
            if (calendarResourceId != null)
                resource = db.GetCalendarResource(url);
            else
                collection = db.GetCollection(url);

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
                    errorOccurred =
                        !(resource?.CreateOrModifyProperty(property.NodeName, property.MainNamespace,
                            GetValueFromRealProperty(property), errorStack) ??
                          collection.CreateOrModifyProperty(property.NodeName, property.MainNamespace,
                              GetValueFromRealProperty(property), errorStack));
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
        ///     This method only functionality is to take the string representation of a property without
        ///     the first line, witch is the template for xml.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        private string GetValueFromRealProperty(IXMLTreeStructure property)
        {
            var temp = property.ToString();
            return temp.Replace(@"<?xml version=""1.0"" encoding=""utf-8""?>", "").TrimStart();
        }

        #endregion

        #region Delete Methods

        public bool DeleteCalendarObjectResource(Dictionary<string, string> propertiesAndHeaders, HttpResponse response)
        {
            #region Extracting Properties

            string url;
            propertiesAndHeaders.TryGetValue("url", out url);

            #endregion

            ///if the collection doesnt exist in the user folder
            /// the can't do anything
            if (!StorageManagement.ExistCalendarCollection(url.Remove(url.LastIndexOf("/") + 1)))
                return true;

            var resource =
                db.GetCalendarResource(url);
            if (resource != null)
            {
                db.DeleteResource(resource);
                db.SaveChanges();

                return StorageManagement.DeleteCalendarObjectResource(url);
            }
            return StorageManagement.DeleteCalendarObjectResource(url);
            ;
        }

        public bool DeleteCalendarCollection(Dictionary<string, string> propertiesAndHeaders, HttpResponse response)
        {
            #region Extracting Properties

            string url;
            propertiesAndHeaders.TryGetValue("url", out url);

            #endregion

            //The delete method default status code
            response.StatusCode = (int)HttpStatusCode.NoContent;
            //If the collection already is gone it is treated as a successful operation.
            if (!StorageManagement.ExistCalendarCollection(url))
                return true;

            //The collection is retrieve and if something unexpected happened an internal error is reflected.
            var collection = db.GetCollection(url);
            if (collection == null)
            {
                StorageManagement.DeleteCalendarCollection(url);
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return false;
            }


            db.DeleteCollection(collection);
            db.SaveChanges();
            return StorageManagement.DeleteCalendarCollection(url);
        }

        #endregion

        #region Get Methods

        public async Task ReadCalendarObjectResource(Dictionary<string, string> propertiesAndHeaders,
            HttpResponse response)
        {
            #region Extracting Properties 

            string url;
            propertiesAndHeaders.TryGetValue("url", out url);

            #endregion

            //An easy way of accessing the headers of the http response
            var responseHeader = response.GetTypedHeaders();

            //StorageManagement.SetUserAndCollection(principalUrl, collectionName);
            //Must return the Etag header of the COR

            var calendarRes = db.GetCalendarResource(url);

            if (calendarRes == null || !StorageManagement.ExistCalendarObjectResource(url))
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            var resourceBody = StorageManagement.GetCalendarObjectResource(url);

            var etagProperty = calendarRes.Properties.SingleOrDefault(x => x.Name == "getetag");
            if (etagProperty != null)
            {
                var etag = XmlTreeStructure.Parse(etagProperty.Value).Value;
                response.Headers["etag"] = etag;
            }
            responseHeader.ContentType = new MediaTypeHeaderValue("text/calendar");
            await response.WriteAsync(resourceBody.Result);
        }

        public string ReadCalendarCollection(Dictionary<string, string> propertiesAndHeaders)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region PUT resource

        //TODO: Nacho
        public async Task AddCalendarObjectResource(Dictionary<string, string> propertiesAndHeaders,
            HttpResponse response)
        {
            #region Extracting Properties

            string url;
            propertiesAndHeaders.TryGetValue("url", out url);

            string ifmatch;
            var ifMatchEtags = new List<string>();
            propertiesAndHeaders.TryGetValue("If-Match", out ifmatch);
            if (ifmatch != null)
                ifMatchEtags = ifmatch.Split(',').ToList();


            string ifnonematch;
            var ifNoneMatchEtags = new List<string>();
            propertiesAndHeaders.TryGetValue("If-None-Match", out ifnonematch);
            if (ifnonematch != null)
            {
                ifNoneMatchEtags = ifnonematch.Split(',').ToList();
            }
            string body;
            propertiesAndHeaders.TryGetValue("body", out body);

            #endregion

            //Note: calendar object resource = COR

            //CheckAllPreconditions

            PreconditionCheck = new PutPrecondition(StorageManagement, db);
            if (!PreconditionCheck.PreconditionsOK(propertiesAndHeaders, response))
                return;

            var resourceExist = db.CalendarResourceExist(url);
            //If the ifmatch is included i look for the etag in the resource, but first the resource has to exist.
            //If all is ok and the if-match etag matches the etag in the resource then i update the resource.
            //If the if-match dont match then i set that the precondition failed.
            if (ifMatchEtags.Count > 0)
            {
                if (resourceExist)
                {
                    var resource = db.GetCalendarResource(url);
                    var resourceEtag =
                        XmlTreeStructure.Parse(resource.Properties.FirstOrDefault(x => x.Name == "getetag").Value).Value;
                    if (ifMatchEtags.Contains(resourceEtag))
                    {
                        await UpdateCalendarObjectResource(propertiesAndHeaders, response);
                        return;
                    }
                    response.StatusCode = (int)HttpStatusCode.PreconditionFailed;
                    return;
                }
            }

            if (ifNoneMatchEtags.Count > 0 && ifNoneMatchEtags.Contains("*"))
            {
                if (!resourceExist)
                {
                    await CreateCalendarObjectResource(propertiesAndHeaders, response);
                    return;
                }
                response.StatusCode = (int)HttpStatusCode.PreconditionFailed;
                return;
            }

            if (resourceExist && StorageManagement.ExistCalendarObjectResource(url))
            {
                await UpdateCalendarObjectResource(propertiesAndHeaders, response);
                return;
            }
            await CreateCalendarObjectResource(propertiesAndHeaders, response);

            //return HTTP 201 Created 
        }

        /// <summary>
        ///     Creates a new COR from a PUT when a "If-Non-Match" header is included
        /// </summary>
        /// <param name="propertiesAndHeaders"></param>
        /// <param name="response"></param>
        /// <param></param>
        private async Task CreateCalendarObjectResource(Dictionary<string, string> propertiesAndHeaders,
            HttpResponse response)
        {
            #region Extracting Properties

            string url;
            propertiesAndHeaders.TryGetValue("url", out url);

            string body;
            propertiesAndHeaders.TryGetValue("body", out body);

            var headers = response.GetTypedHeaders();

            #endregion

            var iCal = new VCalendar(body);


            //filling the resource
            var resource = FillResource(propertiesAndHeaders, iCal, response);

            //adding the resource to the db
            var collection = db.GetCollection(url.Remove(url.LastIndexOf("/") + 1));
            collection.CalendarResources.Add(resource);

            //adding the file
            await StorageManagement.AddCalendarObjectResourceFile(url, body);

            //setting the content lenght property.
            var errorStack = new Stack<string>();
            resource.CreateOrModifyPropertyAdmin("getcontentlength", "DAV:",
                $"<D:getcontentlength {Namespaces["D"]}>{StorageManagement.GetFileSize(url)}</D:getcontentlength>",
                errorStack);
            db.SaveChanges();
        }

        /// <summary>
        ///     Updates an existing COR from a PUT when a "If-Match" header is included using the corresponding etag.
        /// </summary>
        /// <param name="propertiesAndHeaders"></param>
        /// <param name="response"></param>
        private async Task UpdateCalendarObjectResource(Dictionary<string, string> propertiesAndHeaders,
            HttpResponse response)
        {
            #region Extracting Properties

            string url;
            propertiesAndHeaders.TryGetValue("url", out url);

            string body;
            propertiesAndHeaders.TryGetValue("body", out body);

            //var headers = response.GetTypedHeaders();

            #endregion

            //var iCal = new VCalendar(body);

            //Fill the resource
            //var resource = FillResource(propertiesAndHeaders, iCal, response);

            var etag = Guid.NewGuid().ToString();
            response.Headers["etag"] = etag;
            //headers.ETag = new EntityTagHeaderValue(etag, false);


            var prevResource = db.GetCalendarResource(url);

            var errorStack = new Stack<string>();
            prevResource.CreateOrModifyPropertyAdmin("getetag", "DAV:",
                $"<D:getetag {Namespaces["D"]}>{etag}</D:getetag>",
                errorStack);

            prevResource.CreateOrModifyPropertyAdmin("getlastmodified", "DAV:",
                $"<D:getlastmodified {Namespaces["D"]}>{DateTime.Now}</D:getlastmodified>", errorStack);


            //Removing old File 
            StorageManagement.DeleteCalendarObjectResource(url);
            //Adding New File
            await StorageManagement.AddCalendarObjectResourceFile(url, body);

            prevResource.CreateOrModifyPropertyAdmin("getcontentlength", "DAV:",
                $"<D:getcontentlength {Namespaces["D"]}>{StorageManagement.GetFileSize(url)}</D:getcontentlength>",
                errorStack);

            //Adding to the dataBase
            db.CalendarResources.Update(prevResource);
            db.SaveChanges();
        }

        /// <summary>
        ///     Method in charge of fill a CalendarResource and Return it.
        /// </summary>
        /// <param name="propertiesAndHeaders"></param>
        /// <param name="iCal"></param>
        /// <param name="response"></param>
        /// <param name="s"></param>
        /// <param name="retEtag"></param>
        /// <returns></returns>
        private CalendarResource FillResource(Dictionary<string, string> propertiesAndHeaders, VCalendar iCal,
            HttpResponse response)
        {
            #region Extracting Properties

            string calendarResourceId;
            propertiesAndHeaders.TryGetValue("calendarResourceId", out calendarResourceId);

            string url;
            propertiesAndHeaders.TryGetValue("url", out url);


            //var headers = response.GetTypedHeaders();

            #endregion

            var etag = Guid.NewGuid().ToString();
            response.Headers["etag"] = etag;

            var resource = new CalendarResource(url, calendarResourceId);

            var errorStack = new Stack<string>();
            resource.CreateOrModifyPropertyAdmin("getetag", "DAV:", $"<D:getetag {Namespaces["D"]}>{etag}</D:getetag>",
                errorStack);

            var collection = db.GetCollection(url.Remove(url.LastIndexOf("/") + 1));
            var stack = new Stack<string>();
            collection.CreateOrModifyPropertyAdmin("getctag", "http://calendarserver.org/ns/",
                new XmlTreeStructure("getctag", @"xmlns=""http://calendarserver.org/ns/""") { Value = Guid.NewGuid().ToString() }.ToString(), stack);

            var calendarComponents =
                iCal.CalendarComponents.FirstOrDefault(comp => comp.Key != "VTIMEZONE").Value;
            var calendarComponent = calendarComponents.FirstOrDefault();
            if (calendarComponent != null)
            {
                resource.Uid = calendarComponent.Properties["UID"].StringValue;
            }

            return resource;
        }

        #endregion
    }
}