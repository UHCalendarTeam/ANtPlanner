using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ACL.Core;
using CalDAV.Core.ConditionsCheck;
using CalDAV.Core.ConditionsCheck.Preconditions;
using CalDAV.Core.Method_Extensions;
using CalDAV.Core.Propfind;
using DataLayer;
using DataLayer.Models.ACL;
using DataLayer.Models.Entities;
using DataLayer.Repositories;
using ICalendar.Calendar;
using ICalendar.GeneralInterfaces;
using Microsoft.AspNet.Http;
using TreeForXml;

namespace CalDAV.Core
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class CalDav : ICalDav
    {
        #region Standard Namespace
        private readonly Dictionary<string, string> _namespaces = new Dictionary<string, string>
        {
            {"D", @"xmlns:D=""DAV:"""},
            {"C", @"xmlns:C=""urn:ietf:params:xml:ns:caldav"""},
            {"S", @"xmlns:S=""http://calendarserver.org/ns/"""}
        };

        private readonly Dictionary<string, string> _namespacesSimple = new Dictionary<string, string>
        {
            {"D", "DAV:"},
            {"C", "urn:ietf:params:xml:ns:caldav"},
            {"S", "http://calendarserver.org/ns/"}
        };
        #endregion

        private readonly IACLProfind _aclProfind;
        private readonly ICollectionReport _colectionCollectionReport;
        private readonly CollectionRepository _collectionRespository;
        private readonly ResourceRespository _resourceRespository;
        private readonly PrincipalRepository _principalRepository;


        /// <summary>
        ///     DI in the params.
        /// </summary>
        /// <param name="fsManagement"></param>
        /// <param name="aclProfind"></param>
        /// <param name="collectionCollectionReport"></param>
        /// <param name="collectionRespository"></param>
        /// <param name="resourceRespository"></param>
        /// <param name="principalRepository"></param>
        public CalDav(IFileSystemManagement fsManagement, IACLProfind aclProfind, ICollectionReport collectionCollectionReport, IRepository<CalendarCollection,
                string> collectionRespository, IRepository<CalendarResource, string> resourceRespository, IRepository<Principal, string> principalRepository)
        {
            StorageManagement = fsManagement;
            _aclProfind = aclProfind;
            _colectionCollectionReport = collectionCollectionReport;
            _collectionRespository = collectionRespository as CollectionRepository;
            _principalRepository = principalRepository as PrincipalRepository;
            _resourceRespository = resourceRespository as ResourceRespository;
        }

        #region Dependencies
        private IFileSystemManagement StorageManagement { get; }
        private IPropfindMethods PropFindMethods { get; set; }
        private IPrecondition PreconditionCheck { get; set; }
        private IPoscondition PosconditionCheck { get; set; }

        //private CalDavContext db { get; }

        #endregion

        /// <summary>
        /// Call this method after a client request and 
        /// will handle the REPORT on collections.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
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
        public async Task PropFind(Dictionary<string, string> propertiesAndHeaders, string body, HttpResponse response)
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
            int depth;
            string strDepth;
            propertiesAndHeaders.TryGetValue("depth", out strDepth);
            try
            {
                depth = strDepth != null ? int.Parse(strDepth) : 0;
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

            //checking Precondtions
            PreconditionCheck = new PropfindPrecondition(_collectionRespository, _resourceRespository);
            if (!await PreconditionCheck.PreconditionsOK(propertiesAndHeaders, response))
                return;

            response.StatusCode = 207;
            response.ContentType = "application/xml";
            var responseTree = new XmlTreeStructure("multistatus", "DAV:");
            responseTree.Namespaces.Add("D", "DAV:");
            responseTree.Namespaces.Add("C", "urn:ietf:params:xml:ns:caldav");
            responseTree.Namespaces.Add("S", _namespacesSimple["S"]);

            //Tool that contains the methods for propfind.
            PropFindMethods = new CalDavPropfind(_collectionRespository, _resourceRespository);

            //if the body is empty assume that is an allprop request.          
            if (string.IsNullOrEmpty(body))
            {
                await PropFindMethods.AllPropMethod(url, calendarResourceId, depth, null, responseTree);

                await response.WriteAsync(responseTree.ToString());
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

                    //take the principalId from the properties
                    var principalId = propertiesAndHeaders["principalId"];
                    var principal =  _principalRepository.GetByIdentifier(principalId);
                    await PropFindMethods.PropMethod(url, calendarResourceId, depth, props, responseTree, principal);
                    break;
                case "allprop":
                    var additionalProperties = ExtractIncludePropertiesNameMainNS((XmlTreeStructure)xmlTree);
                    await PropFindMethods.AllPropMethod(url, calendarResourceId,
                        depth, additionalProperties, responseTree);
                    break;
                case "propname":
                    await PropFindMethods.PropNameMethod(url, calendarResourceId, depth, responseTree);
                    break;
                default:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return;
            }
            var responseText = responseTree.ToString();
            byte[] responseBytes = Encoding.UTF8.GetBytes(responseText);
            response.ContentLength = responseBytes.Length;
            await response.WriteAsync(responseText);
        }

        /// <summary>
        ///     This method perfoms a profind on a principal.
        /// </summary>
        /// <returns></returns>
        public async Task ACLProfind(HttpContext httpContext)
        {
            await _aclProfind.Profind(httpContext);
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

            PreconditionCheck = new MKCalendarPrecondition(StorageManagement, _collectionRespository);
            PosconditionCheck = new MKCalendarPosCondition(StorageManagement, _collectionRespository);

            //Checking that all precondition pass

            //Cheking Preconditions
            if (!await PreconditionCheck.PreconditionsOK(propertiesAndHeaders, response))
                return;

            //I create here the collection already but i wait for other comprobations before save the database.
            await CreateDefaultCalendar(propertiesAndHeaders);
            response.StatusCode = (int)HttpStatusCode.Created;



            //If it has not body and  Posconditions are OK, it is created with default values.
            if (string.IsNullOrEmpty(body))
            {
                if (!await PosconditionCheck.PosconditionOk(propertiesAndHeaders, response))
                {
                    await DeleteCalendarCollection(propertiesAndHeaders, response);
                    response.StatusCode = (int)HttpStatusCode.Forbidden;
                    await response.WriteAsync("Poscondition Failed");
                    return;
                }
                await _collectionRespository.SaveChangeAsync();
                return;
            }

            //If a body exist the it is parsed like an XmlTree
            var mkCalendarTree = XmlTreeStructure.Parse(body);

            //if it does not have set property it is treated as a empty body.
            if (mkCalendarTree.Children.Count == 0)
            {
                if (!await PosconditionCheck.PosconditionOk(propertiesAndHeaders, response))
                {
                    await DeleteCalendarCollection(propertiesAndHeaders, response);
                    response.StatusCode = (int)HttpStatusCode.Forbidden;
                    await response.WriteAsync("Poscondition Failed");

                    return;
                }
                await _collectionRespository.SaveChangeAsync();
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
            var hasError = await BuiltResponseForSet(url, null, false, setTree, responseTree);

            if (hasError)
            {
                await DeleteCalendarCollection(propertiesAndHeaders, response);
                response.ContentType = "application/xml";

                ChangeToDependencyError(responseTree);

                response.StatusCode = 207;
                await response.WriteAsync(multistatus.ToString());
                return;
            }

            //Checking Preconditions   
            if (await PosconditionCheck.PosconditionOk(propertiesAndHeaders, response))
            {
                await _collectionRespository.SaveChangeAsync();
                return;
                // return createdMessage;
            }

            await DeleteCalendarCollection(propertiesAndHeaders, response);
            response.StatusCode = (int)HttpStatusCode.Forbidden;
            await response.WriteAsync("Poscondition Failed");
        }

        private async Task CreateDefaultCalendar(Dictionary<string, string> propertiesAndHeaders)
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

            var principal =  _principalRepository.GetByIdentifier(principalId);
            var collection = new CalendarCollection(url, collectionName);
            var stack = new Stack<string>();
            await _collectionRespository.CreateOrModifyProperty(url, "getctag", "http://calendarserver.org/ns/",
               new XmlTreeStructure("getctag", @"xmlns=""http://calendarserver.org/ns/""")
               {
                   Value = Guid.NewGuid().ToString()
               }.ToString(), stack, true);
            principal.CalendarCollections.Add(collection);

            //Adding the collection folder.
            StorageManagement.AddCalendarCollectionFolder(url);
        }

        #endregion

        #region Proppatch

        //TODO:Nacho
        public async Task PropPatch(Dictionary<string, string> propertiesAndHeaders, string body, HttpResponse response)
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

            //Checking precondition
            PreconditionCheck = new ProppatchPrecondition(_collectionRespository, _resourceRespository);
            if (!await PreconditionCheck.PreconditionsOK(propertiesAndHeaders, response))
                return;

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
                await response.WriteAsync(@"Body in bad format, body of proppatch must contain ""propertyupdate"" xml element");
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
                await response.WriteAsync("propertyupdate must have at least one element");
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


            //For each set and remove try to execute the operation if something fails 
            //put the Failed Dependency Error to every property before and after the error
            //even if the operation for the property was succesfully changed.
            foreach (var setOrRemove in setsAndRemoves)
            {
                if (setOrRemove.NodeName == "set")
                    hasError = await BuiltResponseForSet(url, calendarResourceId, hasError,
                        setOrRemove, responseTree);
                else
                    hasError = await BuiltResponseForRemove(url, calendarResourceId, hasError,
                        setOrRemove, responseTree);
            }

            if (hasError)
            {
                ChangeToDependencyError(responseTree);
            }
            else
            {
                await _collectionRespository.SaveChangeAsync();
            }

            response.StatusCode = 207;
            await response.WriteAsync(multistatus.ToString());
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

        private async Task<bool> BuiltResponseForRemove(string url, string calendarResourceId,
            bool errorOccurred, IXMLTreeStructure removeTree, IXMLTreeStructure response)
        {

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
                        !(calendarResourceId != null ? await _resourceRespository.RemoveProperty(url, new KeyValuePair<string, string>(property.NodeName, property.MainNamespace), errorStack) :
                          await _collectionRespository.RemoveProperty(url,
                              new KeyValuePair<string, string>(property.NodeName, property.MainNamespace), errorStack));
                    //collection.RemoveProperty(property.NodeName, property.MainNamespace, errorStack));
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

        private async Task<bool> BuiltResponseForSet(string url, string calendarResourceId,
            bool errorOccurred, IXMLTreeStructure setTree, IXMLTreeStructure response)
        {

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
                        !(calendarResourceId != null ? await _resourceRespository.CreateOrModifyProperty(url, property.NodeName, property.MainNamespace,
                            GetValueFromRealProperty(property), errorStack, false) :
                          await _collectionRespository.CreateOrModifyProperty(url, property.NodeName, property.MainNamespace,
                              GetValueFromRealProperty(property), errorStack, false));
                    //collection.CreateOrModifyProperty(property.NodeName, property.MainNamespace,
                    //    GetValueFromRealProperty(property), errorStack));
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

        public async Task<bool> DeleteCalendarObjectResource(Dictionary<string, string> propertiesAndHeaders, HttpResponse response)
        {
            #region Extracting Properties

            string url;
            propertiesAndHeaders.TryGetValue("url", out url);

            string ifmatch;
            var ifMatchEtags = new List<string>();
            propertiesAndHeaders.TryGetValue("If-Match", out ifmatch);
            if (ifmatch != null)
                ifMatchEtags = ifmatch.Split(',').ToList();

            #endregion

            //if the collection doesnt exist in the user folder
            // the can't do anything

            var collectionUrl = url?.Remove(url.LastIndexOf("/", StringComparison.Ordinal) + 1);
            if (!StorageManagement.ExistCalendarCollection(collectionUrl) && !await _collectionRespository.Exist(collectionUrl))
                return true;

            var resource =
                await _resourceRespository.Get(url);

            if (ifMatchEtags.Count > 0)
            {
                if (resource != null)
                {
                    var resourceEtag =
                        XmlTreeStructure.Parse(resource.Properties.FirstOrDefault(x => x.Name == "getetag")?.Value).Value;
                    if (resourceEtag != null && ifMatchEtags.Contains(resourceEtag))
                    {
                        response.StatusCode = (int)HttpStatusCode.NoContent;
                        await _resourceRespository.Remove(resource);


                        //updating the ctag
                        var stack = new Stack<string>();
                        await _collectionRespository.CreateOrModifyProperty(collectionUrl, "getctag", _namespacesSimple["S"],
                        $@"<S:getctag {_namespaces["S"]} >{Guid.NewGuid()}</S:getctag>", stack, true);


                        return StorageManagement.DeleteCalendarObjectResource(url);
                    }
                }
            }


            if (resource != null)
            {
                response.StatusCode = (int)HttpStatusCode.NoContent;
                await _resourceRespository.Remove(resource);

                //updating the ctag
                var stack = new Stack<string>();
                await _collectionRespository.CreateOrModifyProperty(collectionUrl, "getctag", _namespacesSimple["S"],
                $@"<S:getctag {_namespaces["S"]} >{Guid.NewGuid()}</S:getctag>", stack, true);


                return StorageManagement.DeleteCalendarObjectResource(url);
            }

            return StorageManagement.DeleteCalendarObjectResource(url);
        }

        public async Task<bool> DeleteCalendarCollection(Dictionary<string, string> propertiesAndHeaders, HttpResponse response)
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
            var collection = await _collectionRespository.Get(url);
            if (collection == null)
            {
                StorageManagement.DeleteCalendarCollection(url);
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return false;
            }


            await _collectionRespository.Remove(collection);

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
            response.GetTypedHeaders();

            //StorageManagement.SetUserAndCollection(principalUrl, collectionName);
            //Must return the Etag header of the COR

            var calendarRes = await _resourceRespository.Get(url);

            if (calendarRes == null || !StorageManagement.ExistCalendarObjectResource(url))
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            var resourceBody = await StorageManagement.GetCalendarObjectResource(url);

            var etagProperty = calendarRes.Properties.FirstOrDefault(x => x.Name == "getetag");
            if (etagProperty != null)
            {
                var etag = XmlTreeStructure.Parse(etagProperty.Value).Value;
                response.Headers["etag"] = etag;
            }

            await response.WriteAsync(resourceBody);
        }

        public string ReadCalendarCollection(Dictionary<string, string> propertiesAndHeaders)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region PUT resource

        //TODO: Poner esto en la capa de datos
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

            PreconditionCheck = new PutPrecondition(StorageManagement, _collectionRespository, _resourceRespository);
            if (!await PreconditionCheck.PreconditionsOK(propertiesAndHeaders, response))
                return;

            var resourceExist = await _resourceRespository.Exist(url);
            //If the ifmatch is included i look for the etag in the resource, but first the resource has to exist.
            //If all is ok and the if-match etag matches the etag in the resource then i update the resource.
            //If the if-match dont match then i set that the precondition failed.
            if (ifMatchEtags.Count > 0)
            {
                if (resourceExist)
                {
                    var resource = await _resourceRespository.Get(url);
                    var resourceEtag =
                        XmlTreeStructure.Parse(resource.Properties.FirstOrDefault(x => x.Name == "getetag")?.Value).Value;
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


            response.GetTypedHeaders();

            #endregion

            var iCal = new VCalendar(body);


            //filling the resource
            var resource = await FillResource(propertiesAndHeaders, iCal, response);

            //adding the resource to the db
            var collection = await _collectionRespository.Get(url?.Remove(url.LastIndexOf("/", StringComparison.Ordinal) + 1));
            collection.CalendarResources.Add(resource);

            //adding the file
            await StorageManagement.AddCalendarObjectResourceFile(url, body);

            response.StatusCode = (int)HttpStatusCode.Created;

            //setting the content lenght property.
            var errorStack = new Stack<string>();
            await _resourceRespository.CreatePropertyForResource(resource, "getcontentlength", "DAV:",
                $"<D:getcontentlength {_namespaces["D"]}>{StorageManagement.GetFileSize(url)}</D:getcontentlength>",
                errorStack, true);
            await _collectionRespository.SaveChangeAsync();
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

            var etag = $"\"{Guid.NewGuid()}\"";
            response.Headers["etag"] = etag;
            //headers.ETag = new EntityTagHeaderValue(etag, false);

            var errorStack = new Stack<string>();

            //updating the etag
            await _resourceRespository.CreateOrModifyProperty(url, "getetag", _namespacesSimple["D"],
                 $"<D:getetag {_namespaces["D"]}>{etag}</D:getetag>",
                 errorStack, true);

            //updating the ctag
            await _collectionRespository.CreateOrModifyProperty(url?.Remove(url.LastIndexOf("/", StringComparison.Ordinal) + 1), "getctag", _namespacesSimple["S"],
                 $@"<S:getctag {_namespaces["S"]} >{Guid.NewGuid()}</S:getctag>", errorStack, true);

            //updating the lastmodified
            await _resourceRespository.CreateOrModifyProperty(url, "getlastmodified", _namespacesSimple["D"],
                $"<D:getlastmodified {_namespaces["D"]}>{DateTime.Now}</D:getlastmodified>", errorStack, true);


            //Removing old File 
            StorageManagement.DeleteCalendarObjectResource(url);
            //Adding New File
            await StorageManagement.AddCalendarObjectResourceFile(url, body);

            await _resourceRespository.CreateOrModifyProperty(url, "getcontentlength", "DAV:",
                 $"<D:getcontentlength {_namespaces["D"]}>{StorageManagement.GetFileSize(url)}</D:getcontentlength>",
                 errorStack, true);

            //the response for this methos is NO CONTENT
            response.StatusCode = (int)HttpStatusCode.NoContent;

            //Adding to the dataBase
            await _resourceRespository.SaveChangeAsync();
        }

        /// <summary>
        ///     Method in charge of fill a CalendarResource and Return it.
        /// </summary>
        /// <param name="propertiesAndHeaders"></param>
        /// <param name="iCal"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        private async Task<CalendarResource> FillResource(Dictionary<string, string> propertiesAndHeaders, ICalendarComponentsContainer iCal,
            HttpResponse response)
        {
            #region Extracting Properties

            string calendarResourceId;
            propertiesAndHeaders.TryGetValue("calendarResourceId", out calendarResourceId);

            string url;
            propertiesAndHeaders.TryGetValue("url", out url);

            string principalId;
            propertiesAndHeaders.TryGetValue("principalId", out principalId);


            //var headers = response.GetTypedHeaders();

            #endregion

            // calculate etag that will notice a change in the resource
            var etag = $"\"{Guid.NewGuid()}\"";
            response.Headers["etag"] = etag;

            var resource = new CalendarResource(url, calendarResourceId);

            //add the owner property           
            var principal = _principalRepository.GetByIdentifier(principalId);
            var principalUrl = principal == null ? "" : principal.PrincipalURL;

            resource.Properties.Add(PropertyCreation.CreateOwner(principalUrl));
            resource.Properties.Add(PropertyCreation.CreateAclPropertyForUserCollections(principalUrl));
            resource.Properties.Add(PropertyCreation.CreateSupportedPrivilegeSetForResources());

            // await _resourceRespository.Add(resource);

            //adding the calculated etag in the getetag property of the resource
            var errorStack = new Stack<string>();
            await _resourceRespository.CreatePropertyForResource(resource, "getetag", "DAV:", $"<D:getetag {_namespaces["D"]}>{etag}</D:getetag>",
                errorStack, true);

            //updating the ctag of the collection noticing this way that the collection has changed.
            var stack = new Stack<string>();
            await _collectionRespository.CreateOrModifyProperty(url?.Remove(url.LastIndexOf("/", StringComparison.Ordinal) + 1), "getctag", _namespacesSimple["S"],
                $@"<S:getctag {_namespaces["S"]} >{Guid.NewGuid()}</S:getctag>", stack, true);

            //getting the uid
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