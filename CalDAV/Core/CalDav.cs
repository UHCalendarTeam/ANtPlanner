using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ACL.Core;
using ACL.Core.Authentication;
using ACL.Core.CheckPermissions;
using CalDAV.ConditionsCheck.Preconditions;
using CalDAV.Core.ConditionsCheck;
using CalDAV.Core.ConditionsCheck.Preconditions;
using CalDAV.Core.Method_Extensions;
using CalDAV.Core.Propfind;
using CalDAV.Method_Extensions;
using DataLayer;
using DataLayer.Models.Entities;
using DataLayer.Models.Entities.ACL;
using DataLayer.Models.Entities.ResourcesAndCollections;
using DataLayer.Models.Interfaces.Repositories;
using DataLayer.Models.Method_Extensions;
using DataLayer.Models.Repositories;
using ICalendar.Calendar;
using ICalendar.GeneralInterfaces;
using Microsoft.AspNetCore.Http;
using TreeForXml;

namespace CalDAV.Core
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class CalDav : ICalDav
    {
        private readonly IACLProfind _aclProfind;
        private readonly ICollectionReport _colectionCollectionReport;
        private readonly ICollectionRepository _collectionRespository;
        private readonly IPrincipalRepository _principalRepository;
        private readonly ICalendarResourceRepository _resourceRespository;
        private readonly IHomeRepository _calendarHomeRepository;
        private readonly IPermissionChecker _permissionChecker;
        private readonly IAuthenticate _authenticate;


        /// <summary>
        ///     DI in the params.
        /// </summary>
        /// <param name="fsManagement"></param>
        /// <param name="aclProfind"></param>
        /// <param name="collectionCollectionReport"></param>
        /// <param name="collectionRespository"></param>
        /// <param name="resourceRespository"></param>
        /// <param name="principalRepository"></param>
        /// <param name="permissionChecker"></param>
        /// <param name="calendarHomeRepository"></param>
        public CalDav(IFileManagement fsManagement, IACLProfind aclProfind,
            ICollectionReport collectionCollectionReport, ICollectionRepository collectionRespository, ICalendarResourceRepository resourceRespository,
            IPrincipalRepository principalRepository, IPermissionChecker permissionChecker, IHomeRepository calendarHomeRepository, IAuthenticate authenticate)
        {
            StorageManagement = fsManagement;
            _aclProfind = aclProfind;
            _colectionCollectionReport = collectionCollectionReport;
            _collectionRespository = collectionRespository;
            _principalRepository = principalRepository;
            _resourceRespository = resourceRespository;
            _calendarHomeRepository = calendarHomeRepository;
            _permissionChecker = permissionChecker;
            _authenticate = authenticate;
        }

        /// <summary>
        ///     Call this method after a client request and
        ///     will handle the REPORT on collections.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public async Task Report(HttpContext httpContext)
        {
            await _colectionCollectionReport.ProcessRequest(httpContext);
        }

        #region Standard Namespace

        private readonly Dictionary<string, string> _namespaces = new Dictionary<string, string>
        {
            {"D", @"xmlns:D=""DAV:"""},
            {"C", @"xmlns:C=""urn:ietf:params:xml:ns:caldav"""},
            {"CS", @"xmlns:CS=""http://calendarserver.org/ns/"""}
        };

        private readonly Dictionary<string, string> _namespacesSimple = new Dictionary<string, string>
        {
            {"D", "DAV:"},
            {"C", "urn:ietf:params:xml:ns:caldav"},
            {"CS", "http://calendarserver.org/ns/"}
        };

        #endregion

        #region Dependencies

        private IFileManagement StorageManagement { get; }
        private IPropfindMethods PropFindMethods { get; set; }
        private IPrecondition PreconditionCheck { get; set; }
        private IPoscondition PosconditionCheck { get; set; }

        //private CalDavContext db { get; }

        #endregion

        #region PROPFIND methods

        //TODO: Nacho
        /// <summary>
        ///     This PROFIND is used for the collection and the resources.
        /// </summary>
        /// <param name="httpContext">
        ///     The Response property in the controller. We fill up the response object
        ///     with out response.
        /// </param>
        public async Task PropFind(HttpContext httpContext)
        {
            #region Extracting Properties

            string calendarResourceId = httpContext.Request.GetResourceId();

            string principalUrl = (await _authenticate.AuthenticateRequestAsync(httpContext))?.PrincipalUrl;

            string url = httpContext.Request.GetRealUrl();

            string body = httpContext.Request.Body.StreamToString();

            //Taking depth form headers.
            //Depth 0 means that it looks for prop only in the collection
            //Depth 1 means that it looks in their childs too.
            //And infinitum that looks in the enly tree.
            int depth;
            string strDepth = httpContext.Request.GetIfMatchValues();
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
            PreconditionCheck = new PropfindPrecondition(_collectionRespository, _resourceRespository, _permissionChecker, _authenticate);
            if (!await PreconditionCheck.PreconditionsOK(httpContext))
                return;

            httpContext.Response.StatusCode = 207;
            httpContext.Response.ContentType = "application/xml";
            var responseTree = new XmlTreeStructure("multistatus", "DAV:");
            responseTree.Namespaces.Add("D", "DAV:");
            responseTree.Namespaces.Add("C", "urn:ietf:params:xml:ns:caldav");
            responseTree.Namespaces.Add("CS", _namespacesSimple["CS"]);

            //Tool that contains the methods for propfind.
            PropFindMethods = new CalDavPropfind(_collectionRespository, _resourceRespository, _calendarHomeRepository);

            //if the body is empty assume that is an allprop request.          
            if (string.IsNullOrEmpty(body))
            {
                await PropFindMethods.AllPropMethod(url, calendarResourceId, depth, null, responseTree);

                await httpContext.Response.WriteAsync(responseTree.ToString());
                return;
            }

            //parsing the body into a xml tree
            var xmlTree = XmlTreeStructure.Parse(body);

            //Managing if the body was ok
            if (xmlTree.NodeName != "propfind")
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }

            //Finding the right method of propfind, it is found in the first child of the tree.
            //This methods take the response tree and they completed it with the necessary values and structure.
            var propType = xmlTree.Children[0];
            switch (propType.NodeName)
            {
                case "prop":
                    var props = ExtractPropertiesNameMainNS((XmlTreeStructure)xmlTree);
                    var principal = _principalRepository.FindUrl(principalUrl);
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
                    httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return;
            }
            var responseText = responseTree.ToString();
            var responseBytes = Encoding.UTF8.GetBytes(responseText);
            httpContext.Response.ContentLength = responseBytes.Length;
            await httpContext.Response.WriteAsync(responseText);
        }

        /// <summary>
        ///     This method perfoms a profind on a principal.
        /// </summary>
        /// <returns></returns>
        public async Task ACLProfind(HttpContext httpContext)
        {
            await _aclProfind.Profind(httpContext);
        }

        public async Task CHSetPropfind(HttpContext httpContext)
        {
            #region Extracting Properties
            string principalUrl = (await _authenticate.AuthenticateRequestAsync(httpContext))?.PrincipalUrl;

            string url = httpContext.Request.GetRealUrl();

            var body = httpContext.Request.Body.StreamToString();
            #endregion

            //Creating and filling the root of the xml tree response
            //All response are composed of a "multistatus" xml element
            //witch contains a "response" element for each collection and resource analized witch url is included in a "href" element as a child of "response".
            //As a child of the "response" there is a list of "propstat". One for each different status obtained
            //trying to get the specified properties.
            //Inside every "propstatus" there are a xml element "prop" 
            //all the properties that match with
            //the given "status" and a "status" xml containing the message of his "propstat".

            ////checking Precondtions
            //PreconditionCheck = new PropfindPrecondition(_collectionRespository, _resourceRespository, _permissionChecker);
            //if (!await PreconditionCheck.PreconditionsOK(propertiesAndHeaders, response))
            //    return;

            httpContext.Response.StatusCode = 207;
            httpContext.Response.ContentType = "application/xml";
            var responseTree = new XmlTreeStructure("multistatus", "DAV:");
            responseTree.Namespaces.Add("D", "DAV:");
            responseTree.Namespaces.Add("C", "urn:ietf:params:xml:ns:caldav");
            responseTree.Namespaces.Add("CS", _namespacesSimple["CS"]);

            //Tool that contains the methods for propfind.
            PropFindMethods = new CalDavPropfind(_collectionRespository, _resourceRespository, _calendarHomeRepository);

            //parsing the body into a xml tree
            var xmlTree = XmlTreeStructure.Parse(body);

            //Managing if the body was ok
            if (xmlTree.NodeName != "propfind")
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
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
                    //TODO:cambiar await
                    var principal = _principalRepository.FindUrl(principalUrl);
                    await PropFindMethods.CHSetPropMethod(url, props, responseTree, principal);
                    break;
                case "allprop":
                    throw new ArgumentException("This should be a prop propfind");
                    break;
                case "propname":
                    throw new ArgumentException("This should be a prop propfind");
                    break;
                default:
                    httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return;
            }
            var responseText = responseTree.ToString();
            var responseBytes = Encoding.UTF8.GetBytes(responseText);
            httpContext.Response.ContentLength = responseBytes.Length;
            await httpContext.Response.WriteAsync(responseText);
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

        public async Task MkCalendar(HttpContext httpContext)
        {
            #region Extracting Properties

            string principalUrl = (await _authenticate.AuthenticateRequestAsync(httpContext))?.PrincipalUrl;
            //propertiesAndHeaders.TryGetValue("principalUrl", out principalUrl);

            string url = httpContext.Request.GetRealUrl();
            //propertiesAndHeaders.TryGetValue("url", out url);
            string collectionName = httpContext.Request.GetCollectionName();

            string body = httpContext.Request.Body.StreamToString();
            #endregion

            //propertiesAndHeaders.Add("body", body);

            PreconditionCheck = new MkCalendarPrecondition(StorageManagement, _collectionRespository, _permissionChecker, _authenticate);
            PosconditionCheck = new MKCalendarPosCondition(StorageManagement, _collectionRespository);

            //Checking that all precondition pass

            //Cheking Preconditions
            if (!await PreconditionCheck.PreconditionsOK(httpContext))
                return;

            //I create here the collection already but i wait for other comprobations before save the database.
            await CreateDefaultCalendar(principalUrl, collectionName, url);
            httpContext.Response.StatusCode = (int)HttpStatusCode.Created;


            //If it has not body and  Posconditions are OK, it is created with default values.
            if (string.IsNullOrEmpty(body))
            {
                if (!await PosconditionCheck.PosconditionOk(httpContext))
                {
                    await DeleteCalendarCollection(url, httpContext);
                    httpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    await httpContext.Response.WriteAsync("Poscondition Failed");
                    return;
                }
                await _collectionRespository.SaveChangesAsync();
                return;
            }

            //If a body exist the it is parsed like an XmlTree
            var mkCalendarTree = XmlTreeStructure.Parse(body);

            //if it does not have set property it is treated as a empty body.
            if (mkCalendarTree.Children.Count == 0)
            {
                if (!await PosconditionCheck.PosconditionOk(httpContext))
                {
                    await DeleteCalendarCollection(url, httpContext);
                    httpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    await httpContext.Response.WriteAsync("Poscondition Failed");

                    return;
                }
                await _collectionRespository.SaveChangesAsync();
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
                await DeleteCalendarCollection(url, httpContext);
                httpContext.Response.ContentType = "application/xml";

                ChangeToDependencyError(responseTree);

                httpContext.Response.StatusCode = 207;
                await httpContext.Response.WriteAsync(multistatus.ToString());
                return;
            }

            //Checking Preconditions   
            if (await PosconditionCheck.PosconditionOk(httpContext))
            {
                await _collectionRespository.SaveChangesAsync();
                return;
                // return createdMessage;
            }

            await DeleteCalendarCollection(url, httpContext);
            httpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            await httpContext.Response.WriteAsync("Poscondition Failed");
        }

        private async Task CreateDefaultCalendar(string principalUrl, string collectionName, string url)
        {
            //Adding the collection to the database
            //TODO: cambiar await
            var principal = _principalRepository.FindUrl(principalUrl);
            var collection = new CalendarCollection(url, collectionName);

            principal.CalendarHome.CalendarCollections.Add(collection);
            await _principalRepository.SaveChangesAsync();

            //Adding the collection folder.
            StorageManagement.CreateFolder(url);
        }

        #endregion

        #region Proppatch

        //TODO:Nacho
        public async Task PropPatch(HttpContext httpContext)
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

            string calendarResourceId = httpContext.Request.GetResourceId();

            string url = httpContext.Request.GetRealUrl();

            string body = httpContext.Request.Body.StreamToString();
            #endregion

            //Checking precondition
            PreconditionCheck = new ProppatchPrecondition(_collectionRespository, _resourceRespository, _permissionChecker, _authenticate);
            if (!await PreconditionCheck.PreconditionsOK(httpContext))
                return;

            //Creating and filling the root of the xml tree response
            //All response of a request is conformed by a "multistatus" element.
            var multistatus = new XmlTreeStructure("multistatus", "DAV:");
            multistatus.Namespaces.Add("D", "DAV:");
            multistatus.Namespaces.Add("C", "urn:ietf:params:xml:ns:caldav");

            httpContext.Response.ContentType = "application/xml";

            //getting the request body structure
            IXMLTreeStructure xmlTree;
            try
            {
               
                xmlTree = XmlTreeStructure.Parse(body);
            }
            catch (Exception)
            {
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }


            //checking that the request has propertyupdate node

            if (xmlTree.NodeName != "propertyupdate")
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await
                    httpContext.Response.WriteAsync(
                        @"Body in bad format, body of proppatch must contain ""propertyupdate"" xml element");
                return;
            }

            //throw new ArgumentException(@"Body in bad format, body of proppatch must contain ""propertyupdate"" xml element");

            var propertyupdate = xmlTree;
            //aliasing the list with all "set" and "remove" structures inside "propertyupdate".
            var setsAndRemoves = propertyupdate.Children;

            //propertyupdate must have at least one element
            if (setsAndRemoves.Count == 0)
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await httpContext.Response.WriteAsync("propertyupdate must have at least one element");
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
                await _collectionRespository.SaveChangesAsync();
            }

            httpContext.Response.StatusCode = 207;
            await httpContext.Response.WriteAsync(multistatus.ToString());
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
                        !(calendarResourceId != null
                            ? await
                                _resourceRespository.RemoveProperty(url,
                                    new KeyValuePair<string, string>(property.NodeName, property.MainNamespace),
                                    errorStack)
                            : await _collectionRespository.RemoveProperty(url,
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
                        !(calendarResourceId != null
                            ? await
                                _resourceRespository.CreateOrModifyProperty(url, property.NodeName,
                                    property.MainNamespace,
                                    GetValueFromRealProperty(property), errorStack, false)
                            : await
                                _collectionRespository.CreateOrModifyProperty(url, property.NodeName,
                                    property.MainNamespace,
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

        public async Task<bool> DeleteCalendarObjectResource(HttpContext httpContext)
        {
            #region Extracting Properties

            string url = httpContext.Request.GetRealUrl();

            string ifmatch = httpContext.Request.GetIfMatchValues();
            var ifMatchEtags = new List<string>();

            if (ifmatch != null)
                ifMatchEtags = ifmatch.Split(',').ToList();

            #endregion
            //Checking precondition
            PreconditionCheck = new DeleteResourcePrecondition(_collectionRespository, _resourceRespository, _permissionChecker, _authenticate);
            if (!await PreconditionCheck.PreconditionsOK(httpContext))
                return false;

            //if the collection doesn't exist in the user folder
            // the can't do anything
            var collectionUrl = url?.Remove(url.LastIndexOf("/", StringComparison.Ordinal) + 1);
            if (!StorageManagement.ExistCalendarCollection(collectionUrl) &&
                 //todo : poner await
                 _collectionRespository.FindUrl(collectionUrl) == null)
                return true;
            //todo poner await
            var resource = _resourceRespository.FindUrl(url);
            //Checking that if exist an IF-Match Header the delete performs its operation
            //avoiding lost updates.
            if (ifMatchEtags.Count > 0)
            {
                if (resource != null)
                {
                    var resourceEtag =
                        XmlTreeStructure.Parse(resource.Properties.FirstOrDefault(x => x.Name == "getetag")?.Value)
                            .Value;
                    if (resourceEtag != null)
                    {
                        //if the ETags match the perform delete
                        if (ifMatchEtags.Contains(resourceEtag))
                        {
                            httpContext.Response.StatusCode = (int)HttpStatusCode.NoContent;
                            await _resourceRespository.Remove(resource);


                            //updating the ctag
                            var stack = new Stack<string>();
                            await
                                _collectionRespository.CreateOrModifyProperty(collectionUrl, "getctag",
                                    _namespacesSimple["CS"],
                                    $@"<CS:getctag {_namespaces["CS"]} >{Guid.NewGuid()}</CS:getctag>", stack, true);


                            return StorageManagement.DeleteCalendarObjectResource(url);
                        }
                        //if the comparison fails the an error is returned.
                        httpContext.Response.StatusCode = (int)HttpStatusCode.PreconditionFailed;
                        return false;
                    }

                }
            }


            if (resource != null)
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.NoContent;
                await _resourceRespository.Remove(resource);

                //updating the ctag
                var stack = new Stack<string>();
                await _collectionRespository.CreateOrModifyProperty(collectionUrl, "getctag", _namespacesSimple["CS"],
                    $@"<CS:getctag {_namespaces["CS"]} >{Guid.NewGuid()}</CS:getctag>", stack, true);


                return StorageManagement.DeleteCalendarObjectResource(url);
            }

            return StorageManagement.DeleteCalendarObjectResource(url);
        }

        public async Task<bool> DeleteCalendarCollection(string url, HttpContext httpContext)
        {
            //Checking precondition
            PreconditionCheck = new DeleteCollectionPrecondition(_collectionRespository, _resourceRespository, _permissionChecker, _authenticate);
            if (!await PreconditionCheck.PreconditionsOK(httpContext))
                return false;


            //The delete method default status code
            httpContext.Response.StatusCode = (int)HttpStatusCode.NoContent;
            //If the collection already is gone it is treated as a successful operation.
            if (!StorageManagement.ExistCalendarCollection(url))
                return true;

            //The collection is retrieve and if something unexpected happened an internal error is reflected.
            //TODO:cambiar await
            var collection = _collectionRespository.FindUrl(url);
            if (collection == null)
            {
                StorageManagement.DeleteCalendarCollection(url);
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return false;
            }


            await _collectionRespository.Remove(collection);

            return StorageManagement.DeleteCalendarCollection(url);
        }

        #endregion

        #region Get Methods

        public async Task ReadCalendarObjectResource(HttpContext httpContext)
        {
            #region Extracting Properties 

            string url = httpContext.Request.GetRealUrl();

            #endregion

            //An easy way of accessing the headers of the http response
            httpContext.Response.GetTypedHeaders();

            //StorageManagement.SetUserAndCollection(principalUrl, collectionName);
            //Must return the Etag header of the COR

            //TODO:cambiar await
            var calendarRes = _resourceRespository.FindUrl(url);

            if (calendarRes == null || !StorageManagement.ExistCalendarObjectResource(url))
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            var resourceBody = await StorageManagement.GetCalendarObjectResource(url);

            var etagProperty = calendarRes.Properties.FirstOrDefault(x => x.Name == "getetag");
            if (etagProperty != null)
            {
                var etag = XmlTreeStructure.Parse(etagProperty.Value).Value;
                httpContext.Response.Headers["etag"] = etag;
            }

            await httpContext.Response.WriteAsync(resourceBody);
        }

        public string ReadCalendarCollection(HttpContext httpContext)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region PUT resource

        //TODO: Poner esto en la capa de datos
        public async Task AddCalendarObjectResource(HttpContext httpContext)
        {
            #region Extracting Properties

            string url = httpContext.Request.GetRealUrl();

            string ifmatch = httpContext.Request.GetIfMatchValues();
            var ifMatchEtags = new List<string>();

            if (ifmatch != null)
                ifMatchEtags = ifmatch.Split(',').ToList();


            string ifnonematch = httpContext.Request.GetIfNoneMatchValues();
            var ifNoneMatchEtags = new List<string>();

            if (ifnonematch != null)
            {
                ifNoneMatchEtags = ifnonematch.Split(',').ToList();
            }
            //string body = httpContext.Request.Body.StreamToString();

            #endregion

            //Note: calendar object resource = COR

            //CheckAllPreconditions

            PreconditionCheck = new PutPrecondition(StorageManagement, _collectionRespository, _resourceRespository, _permissionChecker, _authenticate);
            if (!await PreconditionCheck.PreconditionsOK(httpContext))
                return;
            //TODO: cambiar por await
            var resourceExist = _resourceRespository.FindUrl(url) != null;
            //If the ifmatch is included i look for the etag in the resource, but first the resource has to exist.
            //If all is ok and the if-match etag matches the etag in the resource then i update the resource.
            //If the if-match dont match then i set that the precondition failed.
            if (ifMatchEtags.Count > 0)
            {
                if (resourceExist)
                {
                    var resource = _resourceRespository.FindUrl(url);
                    var resourceEtag =
                        XmlTreeStructure.Parse(resource.Properties.FirstOrDefault(x => x.Name == "getetag")?.Value)
                            .Value;
                    if (ifMatchEtags.Contains(resourceEtag))//TODO: ERROR HERE!!!
                    {
                        await UpdateCalendarObjectResource(httpContext);
                        return;
                    }
                    httpContext.Response.StatusCode = (int)HttpStatusCode.PreconditionFailed;
                    return;
                }
            }

            if (ifNoneMatchEtags.Count > 0 && ifNoneMatchEtags.Contains("*"))
            {
                if (!resourceExist)
                {
                    await CreateCalendarObjectResource(httpContext);
                    return;
                }
                httpContext.Response.StatusCode = (int)HttpStatusCode.PreconditionFailed;
                return;
            }

            if (resourceExist && StorageManagement.ExistCalendarObjectResource(url))
            {
                await UpdateCalendarObjectResource(httpContext);
                return;
            }
            await CreateCalendarObjectResource(httpContext);

            //return HTTP 201 Created 
        }

        /// <summary>
        ///     Creates a new COR from a PUT when a "If-Non-Match" header is included
        /// </summary>
        /// <param name="propertiesAndHeaders"></param>
        /// <param name="response"></param>
        /// <param></param>
        private async Task CreateCalendarObjectResource(HttpContext httpContext)
        {
            #region Extracting Properties

            string url = httpContext.Request.GetRealUrl();

            //string body = httpContext.Request.Body.StreamToString();
            string body = SystemProperties.BODY_TEM;

            httpContext.Response.GetTypedHeaders();

            #endregion

            var iCal = new VCalendar(body);


            //filling the resource
            var resource = await FillResource(iCal, httpContext);

            //adding the resource to the db
            var collection = _collectionRespository.FindUrl(url?.Remove(url.LastIndexOf("/", StringComparison.Ordinal) + 1));
            resource.CalendarCollectionId = collection.Id;
            collection.CalendarResources.Add(resource);

            await _collectionRespository.SaveChangesAsync();

            //adding the file
            await StorageManagement.AddCalendarObjectResourceFile(url, body);

            httpContext.Response.StatusCode = (int)HttpStatusCode.Created;

            //setting the content lenght property.
            var errorStack = new Stack<string>();
            await _resourceRespository.CreateOrModifyProperty(resource.Href, "getcontentlength", "DAV:",
                $"<D:getcontentlength {_namespaces["D"]}>{StorageManagement.GetFileSize(url)}</D:getcontentlength>",
                errorStack, true);
            await _collectionRespository.SaveChangesAsync();
        }

        /// <summary>
        ///     Updates an existing COR from a PUT when a "If-Match" header is included using the corresponding etag.
        /// </summary>
        /// <param name="propertiesAndHeaders"></param>
        /// <param name="response"></param>
        private async Task UpdateCalendarObjectResource(HttpContext httpContext)
        {
            #region Extracting Properties

            string url = httpContext.Request.GetRealUrl();

            //string body = httpContext.Request.Body.StreamToString();
            string body = SystemProperties.BODY_TEM;

            //var headers = response.GetTypedHeaders();

            #endregion

            //var iCal = new VCalendar(body);

            //Fill the resource
            //var resource = FillResource(propertiesAndHeaders, iCal, response);

            var etag = $"\"{Guid.NewGuid()}\"";
            httpContext.Response.Headers["etag"] = etag;
            //headers.ETag = new EntityTagHeaderValue(etag, false);

            var errorStack = new Stack<string>();

            //updating the etag
            await _resourceRespository.CreateOrModifyProperty(url, "getetag", _namespacesSimple["D"],
                $"<D:getetag {_namespaces["D"]}>{etag}</D:getetag>",
                errorStack, true);

            //updating the ctag
            await
                _collectionRespository.CreateOrModifyProperty(
                    url?.Remove(url.LastIndexOf("/", StringComparison.Ordinal) + 1), "getctag", _namespacesSimple["CS"],
                    $@"<CS:getctag {_namespaces["CS"]} >{Guid.NewGuid()}</CS:getctag>", errorStack, true);

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
            httpContext.Response.StatusCode = (int)HttpStatusCode.NoContent;

            //Adding to the dataBase
            await _resourceRespository.SaveChangesAsync();
        }

        /// <summary>
        ///     Method in charge of fill a CalendarResource and Return it.
        /// </summary>
        /// <param name="propertiesAndHeaders"></param>
        /// <param name="iCal"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        private async Task<CalendarResource> FillResource(ICalendarComponentsContainer iCal, HttpContext httpContext)
        {
            #region Extracting Properties

            string calendarResourceId = httpContext.Request.GetResourceId();

            string url = httpContext.Request.GetRealUrl();

            string principalUrl = (await _authenticate.AuthenticateRequestAsync(httpContext))?.PrincipalUrl;

            //var headers = response.GetTypedHeaders();

            #endregion

            // calculate etag that will notice a change in the resource
            var etag = $"\"{Guid.NewGuid()}\"";
            httpContext.Response.Headers["etag"] = etag;

            var resource = new CalendarResource(url, calendarResourceId);

            //add the owner property  
            //TODO:CAMBIaR await         
            var principal = _principalRepository.FindUrl(principalUrl);


            resource.Properties.Add(PropertyCreation.CreateOwner(principalUrl));
            resource.Properties.Add(PropertyCreation.CreateAclPropertyForUserCollections(principalUrl));
            resource.Properties.Add(PropertyCreation.CreateSupportedPrivilegeSetForResources());

            // await _resourceRespository.Add(resource);

            //adding the calculated etag in the getetag property of the resource
            var errorStack = new Stack<string>();
            await
                _resourceRespository.CreatePropertyForResource(resource, "getetag", "DAV:",
                    $"<D:getetag {_namespaces["D"]}>{etag}</D:getetag>",
                    errorStack, true);

            //updating the ctag of the collection noticing this way that the collection has changed.
            var stack = new Stack<string>();
            await
                _collectionRespository.CreateOrModifyProperty(
                    url?.Remove(url.LastIndexOf("/", StringComparison.Ordinal) + 1), "getctag", _namespacesSimple["CS"],
                    $@"<CS:getctag {_namespaces["CS"]} >{Guid.NewGuid()}</CS:getctag>", stack, true);

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