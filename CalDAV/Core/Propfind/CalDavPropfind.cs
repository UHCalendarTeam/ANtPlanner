using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ACL.Core.Extension_Method;
using DataLayer;
using DataLayer.Models.Entities;
using DataLayer.Models.Entities.ACL;
using DataLayer.Models.Entities.ResourcesAndCollections;
using DataLayer.Models.Interfaces.Repositories;
using DataLayer.Models.Repositories;
using TreeForXml;

namespace CalDAV.Core.Propfind
{
    /// <summary>
    ///     Propfind es el metodo de webdav encargado de lidiar con las propiedades de colecciones y recursos.
    ///     Este metodo puede tener un comportamiento recursivo si el depth especificado en el header del llamado es
    ///     depth=1 o depth=infinitum, el otro valor es 0.
    ///     depth=infinitum puede ocasionar problemas de eficiencia si las colecciones contienen muchos recursos ou otras
    ///     colecciones(esto
    ///     no es permitido en CalDav ya que las colecciones de calendario no pueden tener anidadas otras colecciones de
    ///     calendario).
    ///     Propfind se componen de tres pedidos diferentes:
    ///     *prop: Donde se especifican las propiedades a buscar y se devuelve el valor de la misma.
    ///     *allprop: Devuelve todas las propiedades visibles (propiedades muertas y algunas vivas --ver rfc4918)
    ///     *propname: Devuelve el nommbre de todas las propiedades implementadas.
    ///     Estas propiedades estan almacendas en los modelos de collection y resource
    ///     Las mutables o vivas directamente especificadas en los modelos
    ///     y las que se mantienen invariables o muertas en un diccionario statico de donde son
    ///     llamadas.
    /// </summary>
    public class CalDavPropfind : IPropfindMethods
    {
        private readonly ICollectionRepository _collectionRepository;
        private readonly ICalendarResourceRepository _resourceRespository;
        private readonly IHomeRepository _calendarHomeRepository;

        public CalDavPropfind(ICollectionRepository collectionRepository,
            ICalendarResourceRepository resourceRepository, IHomeRepository calendarHomeRepository )
        {
            _collectionRepository = collectionRepository;
            _resourceRespository = resourceRepository;
            _calendarHomeRepository = calendarHomeRepository;
        }

        #region Home Set Collection Propfind Methods

        /// <summary>
        /// Method in cha
        /// </summary>
        /// <param name="url"></param>
        /// <param name="propertiesReq"></param>
        /// <param name="multistatusTree"></param>
        /// <param name="principal"></param>
        /// <returns></returns>
        public async Task CHSetPropMethod(string url, List<KeyValuePair<string, string>> propertiesReq, XmlTreeStructure multistatusTree, Principal principal)
        {
            //Here it is created the response body for the collection or resource
            //It depends if calendarResourceId == null.
            var primaryResponse = await CHSetPropFillTree(url, propertiesReq, principal);

            //The response body is added to the result xml tree.
            multistatusTree.AddChild(primaryResponse);

            //check if there was any error
            IXMLTreeStructure errorNode;
            var errorOcurred = primaryResponse.GetChildAtAnyLevel("responsedescription", out errorNode);

            //Now I start putting all objectResource responses if the primary target was a collection
            //and if depth is greater than depth 0.

            #region Adding the responses for resources.

            //TODO:Take the calendar home set instead
            var calendarHome = _calendarHomeRepository.FindWihtCalendarCollections(url);

            foreach (var calendarCollection in calendarHome.CalendarCollections)
            {
                var resourceResponse =
                    await PropFillTree(calendarCollection.Url, null, propertiesReq, principal);
                multistatusTree.AddChild(resourceResponse);

                //error check
                if (!errorOcurred)
                {
                    errorOcurred = resourceResponse.GetChildAtAnyLevel("responsedescription", out errorNode);
                }
            }

            if (errorOcurred)
            {
                errorNode = new XmlTreeStructure("responsedescription", "DAV:");
                errorNode.AddValue("There has been an error");
                multistatusTree.AddChild(errorNode);
            }

            #endregion
        }

        #endregion

        #region Calendar Collections Propfind Methods

        public async Task AllPropMethod(string url, string calendarResourceId, int? depth,
            List<KeyValuePair<string, string>> aditionalProperties, XmlTreeStructure multistatusTree)
        {
            //error flag

            //Here it is created the response body for the collection or resource
            //It depends if calendarResourceId == null.
            var primaryResponse = await AllPropFillTree(url, calendarResourceId, aditionalProperties);

            //The response body is added to the result xml tree.
            multistatusTree.AddChild(primaryResponse);

            //check if there was any error
            IXMLTreeStructure errorNode;
            var errorOcurred = primaryResponse.GetChildAtAnyLevel("responsedescription", out errorNode);

            //Now I start putting all objectResource responses if the primary target was a collection
            //and if depth is greater than depth 0.

            #region Adding the responses for resources.

            if (calendarResourceId == null && depth == 1 || depth == -1)
            {
                var collection = _collectionRepository.FindUrl(url);

                foreach (var calendarResource in collection.CalendarResources)
                {
                    //For every resource in the collection it is added a new xml "response"
                    var resourceResponse = await AllPropFillTree(url + calendarResource.Name, calendarResource.Name,
                        aditionalProperties);
                    multistatusTree.AddChild(resourceResponse);

                    //error check
                    if (!errorOcurred)
                    {
                        errorOcurred = resourceResponse.GetChildAtAnyLevel("responsedescription", out errorNode);
                    }
                }
            }
            if (errorOcurred)
            {
                //if an error occured it is added a new "responsedescription" with a message of the error
                //to the root of the tree. That is the "multistatus" xml.
                errorNode = new XmlTreeStructure("responsedescription", "DAV:");
                errorNode.AddValue("There has been an error");
                multistatusTree.AddChild(errorNode);
            }

            #endregion
        }

        public async Task PropMethod(string url, string calendarResourceId, int? depth,
            List<KeyValuePair<string, string>> propertiesReq, XmlTreeStructure multistatusTree, Principal principal)
        {
            //error flag

            //Here it is created the response body for the collection or resource
            //It depends if calendarResourceId == null.
            var primaryResponse = await PropFillTree(url, calendarResourceId, propertiesReq, principal);

            //The response body is added to the result xml tree.
            multistatusTree.AddChild(primaryResponse);

            //check if there was any error
            IXMLTreeStructure errorNode;
            var errorOcurred = primaryResponse.GetChildAtAnyLevel("responsedescription", out errorNode);

            //Now I start putting all objectResource responses if the primary target was a collection
            //and if depth is greater than depth 0.

            #region Adding the responses for resources.

            if (calendarResourceId == null && depth == 1 || depth == -1)
            {
                var collection = _collectionRepository.FindUrl(url);

                foreach (var calendarResource in collection.CalendarResources)
                {
                    var resourceResponse =
                        await PropFillTree(url + calendarResource.Name, calendarResource.Name, propertiesReq, principal);
                    multistatusTree.AddChild(resourceResponse);

                    //error check
                    if (!errorOcurred)
                    {
                        errorOcurred = resourceResponse.GetChildAtAnyLevel("responsedescription", out errorNode);
                    }
                }
            }
            if (errorOcurred)
            {
                errorNode = new XmlTreeStructure("responsedescription", "DAV:");
                errorNode.AddValue("There has been an error");
                multistatusTree.AddChild(errorNode);
            }

            #endregion
        }

        public async Task PropNameMethod(string url, string calendarResourceId, int? depth,
            XmlTreeStructure multistatusTree)
        {
            //Here it is created the response body for the collection or resource
            //It depends if calendarResourceId == null.
            var primaryResponse = await PropNameFillTree(url);

            //The response body is added to the result xml tree.
            multistatusTree.AddChild(primaryResponse);

            //Now I start putting all objectResource responses if the primary target was a collection
            //and if depth is greater than depth 0.

            #region Adding the responses for resources.

            if (calendarResourceId == null && depth == 1 || depth == -1)
            {
                var collection = _collectionRepository.FindUrl(url);

                foreach (var calendarResource in collection.CalendarResources)
                {
                    var resourceResponse = await PropNameFillTree(url + calendarResource.Name, calendarResource.Name);
                    multistatusTree.AddChild(resourceResponse);
                }
            }

            #endregion
        }
        #endregion

        #region Fill Response Propfind Methods.

        /// <summary>
        ///     Returns a Response XML element with the name of all properties
        ///     of a collection or resource.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="calendarResourceId"></param>
        /// <returns></returns>
        private async Task<XmlTreeStructure> PropNameFillTree(string url, string calendarResourceId = null)
        {
            #region Adding the response of the collection or resource.

            //A "response" structure with all its children is build in this method.
            var treeChild = new XmlTreeStructure("response", "DAV:");

            #region Adding the <D:href>/api/v1/caldav/{userEmail}/calendars/{collectionName}/{calendarResourceId}?</D:href>

            var href = new XmlTreeStructure("href", "DAV:");
            href.AddValue(SystemProperties._baseUrl + url);

            treeChild.AddChild(href);

            #endregion

            #region Adding the propstat

            //in this section is where the "propstat" structure its build.
            var propstat = new XmlTreeStructure("propstat", "DAV:");

            #region Adding nested status

            //each "propstat" has a "status" with the message that define it.
            //"propname" is always "200 OK" because you are only accessing the name of the established properties.
            var status = new XmlTreeStructure("status", "DAV:");
            status.AddValue("HTTP/1.1 200 OK");
            propstat.AddChild(status);

            #endregion

            #region Adding nested prop

            var prop = new XmlTreeStructure("prop", "DAV:");
            List<XmlTreeStructure> properties;

            //Depending if the target is a collection or a resource this section
            //will find the object in the database and get from there all names of properties.
            if (calendarResourceId == null)
            {
                //var collection = _collectionRepository.Get(url).Result;
                properties =
                    (await _collectionRepository.GetAllPropname(url)).Select(p => new XmlTreeStructure(p.Key, p.Value))
                        .ToList();
                //properties = collection.GetAllPropertyNames();
            }
            else
            {
                properties =
                    (await _resourceRespository.GetAllPropname(url)).Select(p => new XmlTreeStructure(p.Key, p.Value))
                        .ToList();
            }

            //Here i add all properties to the prop. 
            foreach (var property in properties)
            {
                prop.AddChild(property);
            }

            propstat.AddChild(prop);

            #endregion

            treeChild.AddChild(propstat);

            #endregion

            return treeChild;

            #endregion
        }

        /// <summary>
        ///     Returns a Response XML element with all the property names
        ///     and property values of the visible properties.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="calendarResourceId">Name of the resource</param>
        /// <param name="additionalProperties">List of additional requested properties (key=name; value=namespace)</param>
        /// <returns></returns>
        private async Task<XmlTreeStructure> AllPropFillTree(string url, string calendarResourceId,
            List<KeyValuePair<string, string>> additionalProperties)
        {
            #region Adding the response of the collection or resource.

            //Adding standard structure for a "response" element.
            var treeChild = new XmlTreeStructure("response", "DAV:");

            #region Adding the <D:href>/api/v1/collections/users|groups/principalId/{collectionName}/{calendarResourceId}?</D:href>

            var href = new XmlTreeStructure("href", "DAV:");
            href.AddValue(SystemProperties._baseUrl + url);

            treeChild.AddChild(href);

            #endregion

            #region Adding the propstat

            #region Selecting properties

            var propertiesCol = new List<XmlTreeStructure>();
            var propertiesOk = new List<XmlTreeStructure>();
            var propertiesWrong = new List<XmlTreeStructure>();
      

            //Here all visible properties are retrieve plus a collection of extra properties that can be 
            //defined in the request body.  

            if (calendarResourceId == null)
            {
                var properties = await _collectionRepository.GetAllProperties(url);

                foreach (var property in properties)
                {
                    //TODO: Check that the property is accessible beyond its visibility.
                    var tempTree = property.Value == null
                        ? new XmlTreeStructure(property.Name, property.Namespace) { Value = "" }
                        : XmlTreeStructure.Parse(property.Value);

                    propertiesCol.Add((XmlTreeStructure)tempTree);
                }

                //looking for additional properties
                if (additionalProperties != null && additionalProperties.Count > 0)
                    foreach (var addProperty in additionalProperties)
                    {
                        //gets the property from database
                        var property = await _collectionRepository.GetProperty(url, addProperty);
                        //Builds the xmlTreeExtructure checking that if the value is null thats because 
                        //the property was not found.
                        IXMLTreeStructure prop;
                        if (property != null)
                            prop = property.Value == null
                                ? new XmlTreeStructure(property.Name, property.Namespace) { Value = "" }
                                : XmlTreeStructure.Parse(property.Value);
                        else
                        {
                            prop = new XmlTreeStructure(addProperty.Key, addProperty.Value);
                        }
                        propertiesCol.Add((XmlTreeStructure)prop);
                    }
            }
            else
            {
                var properties = await _resourceRespository.GetAllProperties(url);

                foreach (var property in properties)
                {
                    //TODO: Check that the property is accessible beyond its visibility.
                    var tempTree = property.Value == null
                        ? new XmlTreeStructure(property.Name, property.Namespace) { Value = "" }
                        : XmlTreeStructure.Parse(property.Value);

                    propertiesCol.Add((XmlTreeStructure)tempTree);
                }

                //looking for additional properties
                if (additionalProperties != null && additionalProperties.Count > 0)
                    foreach (var addProperty in additionalProperties)
                    {
                        //gets the property from database
                        var property = await _resourceRespository.GetProperty(url, addProperty);
                        //Builds the xmlTreeExtructure checking that if the value is null thats because 
                        //the property was not found.
                        IXMLTreeStructure prop;
                        if (property != null)
                            prop = property.Value == null
                                ? new XmlTreeStructure(property.Name, property.Namespace) { Value = "" }
                                : XmlTreeStructure.Parse(property.Value);
                        else
                        {
                            prop = new XmlTreeStructure(addProperty.Key, addProperty.Value);
                        }
                        propertiesCol.Add((XmlTreeStructure)prop);
                    }
            }

            //Here there are divided all properties between recovered and error recovering
            foreach (var propTree in propertiesCol)
            {
                if (propTree.Value != null)
                    propertiesOk.Add(propTree);
                else
                    propertiesWrong.Add(propTree);
            }

            #endregion

            #region Adding nested propOK

            //This procedure has been explained in another method.
            //Here the retrieve properties are grouped.

            var propstatOk = new XmlTreeStructure("propstat", "DAV:");
            var propOk = new XmlTreeStructure("prop", "DAV:");

            //Here i add all properties to the prop. 
            foreach (var property in propertiesOk)
            {
                propOk.AddChild(property);
            }

            propstatOk.AddChild(propOk);

            #region Adding nested status OK

            var statusOk = new XmlTreeStructure("status", "DAV:");
            statusOk.AddValue("HTTP/1.1 200 OK");
            propstatOk.AddChild(statusOk);

            #endregion

            #endregion

            #region Adding nested propWrong

            //Here the properties that could not be retrieved are grouped.
            var propstatWrong = new XmlTreeStructure("propstat", "DAV:");
            var propWrong = new XmlTreeStructure("prop", "DAV:");

            //Here i add all properties to the prop. 
            foreach (var property in propertiesWrong)
            {
                propWrong.AddChild(property);
            }

            propstatWrong.AddChild(propWrong);

            #region Adding nested status Not Found

            var statusWrong = new XmlTreeStructure("status", "DAV:");
            statusWrong.AddValue("HTTP/1.1 400 Not Found");
            propstatWrong.AddChild(statusWrong);

            #endregion

            #region Adding responseDescription when wrong

            //Here i add an description for explain the errors.
            //This should be aplied in all method with an similar structure but for the moment is only used here.
            //However this is not required. 
            var responseDescrpWrong = new XmlTreeStructure("responsedescription", "DAV:");
            responseDescrpWrong.AddValue("The properties doesn't  exist");
            propstatWrong.AddChild(responseDescrpWrong);

            #endregion

            #endregion

            //If any of the "status" group is empty, it is not included.
            if (propertiesOk.Count > 0)
                treeChild.AddChild(propstatOk);
            if (propertiesWrong.Count > 0)
                treeChild.AddChild(propstatWrong);

            #endregion

            return treeChild;

            #endregion
        }

        /// <summary>
        ///     Returns a Response XML tree for a prop request with all the property names
        ///     and property values specified in the request.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="calendarResourceId">Name of the resource</param>
        /// <param name="propertiesNameNamespace">List of requested properties (key=name; value=namespace)</param>
        /// <param name="principal"></param>
        /// <returns></returns>
        private async Task<XmlTreeStructure> PropFillTree(string url, string calendarResourceId,
            List<KeyValuePair<string, string>> propertiesNameNamespace, Principal principal)
        {
            //a "response xml element is added for each collection or resource"

            #region Adding the response of the collection or resource.

            var treeChild = new XmlTreeStructure("response", "DAV:");

            #region Adding the <D:href>/api/v1/caldav/{userEmail}/calendars/{collectionName}/{calendarResourceId}?</D:href>

            //an href with the corresponding url is added to the response
            var href = new XmlTreeStructure("href", "DAV:");
            href.AddValue(SystemProperties._baseUrl + url);

            treeChild.AddChild(href);

            #endregion

            #region Adding the propstats

            #region Selecting properties

            CalendarCollection collection;
            CalendarResource resource;
            var propertiesCol = new List<XmlTreeStructure>();
            var propertiesOk = new List<XmlTreeStructure>();
            var propertiesWrong = new List<XmlTreeStructure>();
           
            //the current-user-privilege-set is generated per request
            //it needs the DAV:acl property and the principalID
            Property aclProperty = null;

            //It take the list of requested properties and tries to get the corresponding property from db.
            //The methods are called for a resource or a collection accordingly its circumstances.
            //The properties are stored inside the propertiesCol. Where if the value is null it means that the collection could not be 
            //retrieve.
            if (calendarResourceId == null)
            {
                collection = _collectionRepository.FindUrl(url);
                if (propertiesNameNamespace != null)
                {
                    foreach (var addProperty in propertiesNameNamespace)
                    {
                        //gets the property from database
                        var property = await _collectionRepository.GetProperty(collection.Id, addProperty);
                        //Builds the xmlTreeExtructure checking that if the value is null thats because 
                        //the property was not found.
                        IXMLTreeStructure prop;
                        if (property != null)
                            prop = property.Value == null
                                ? new XmlTreeStructure(property.Name, property.Namespace) { Value = "" }
                                : XmlTreeStructure.Parse(property.Value);
                        else
                        {
                            prop = new XmlTreeStructure(addProperty.Key, addProperty.Value);
                        }
                        propertiesCol.Add((XmlTreeStructure)prop);
                    }
                    //take the acl property
                    aclProperty = collection.Properties.FirstOrDefault(x => x.Name == "acl");
                }
            }
            else
            {
                resource = _resourceRespository.FindUrl(url);
                if (propertiesNameNamespace != null)
                {
                    foreach (var addProperty in propertiesNameNamespace)
                    {
                        //gets the property from database
                        var property = await _resourceRespository.GetProperty(url, addProperty);
                        //Builds the xmlTreeExtructure checking that if the value is null thats because 
                        //the property was not found.
                        IXMLTreeStructure prop;
                        if (property != null)
                            prop = property.Value == null
                                ? new XmlTreeStructure(property.Name, property.Namespace) { Value = "" }
                                : XmlTreeStructure.Parse(property.Value);
                        else
                        {
                            prop = new XmlTreeStructure(addProperty.Key, addProperty.Value);
                        }
                        propertiesCol.Add((XmlTreeStructure)prop);
                    }
                    //take the acl property
                    aclProperty = resource.Properties.FirstOrDefault(x => x.Name == "acl");
                }
            }

            //add the additional properties that are generated per request
            if (propertiesNameNamespace != null)
                foreach (var pair in propertiesNameNamespace)
                {
                    switch (pair.Key)
                    {
                        case "current-user-privilege-set":
                            propertiesCol.RemoveAll(x => x.NodeName == pair.Key);
                            propertiesCol.Add(principal.GetCurrentUserPermissionProperty(aclProperty));
                            break;
                    }
                }


            //Here, properties are divided between recovered and error recovering
            foreach (var propTree in propertiesCol)
            {
                if (propTree.Value != null)
                    propertiesOk.Add(propTree);
                else
                    propertiesWrong.Add(propTree);
            }

            #endregion

            //For each returned status a "propstat" is created, containing a "prop" with all properties that belong to that current status.
            // And a "status" containing the message of the corresponding status.
            //Right Now there are only two "propstat" taking place OK and Wrong an therefore only two "status"
            //200 OK and 400 Not Found.
            //More should be added when ACL is working entairly.

            //TODO: Add the status forbidden for authentication permissions problems.

            #region Adding nested propOK

            var propstatOk = new XmlTreeStructure("propstat", "DAV:");
            var propOk = new XmlTreeStructure("prop", "DAV:");

            //Here i add all properties to the prop. 
            foreach (var property in propertiesOk)
            {
                propOk.AddChild(property);
            }

            propstatOk.AddChild(propOk);
            //This when i group the OK properties

            #region Adding nested status OK

            var statusOk = new XmlTreeStructure("status", "DAV:");
            statusOk.AddValue("HTTP/1.1 200 OK");
            propstatOk.AddChild(statusOk);

            #endregion

            #endregion

            //Here the same is made. The Wrong properties are grouped. 

            #region Adding nested propWrong

            var propstatWrong = new XmlTreeStructure("propstat", "DAV:");
            var propWrong = new XmlTreeStructure("prop", "DAV:");

            //Here i add all properties to the prop. 
            foreach (var property in propertiesWrong)
            {
                propWrong.AddChild(property);
            }

            propstatWrong.AddChild(propWrong);

            #region Adding nested status Not Found

            var statusWrong = new XmlTreeStructure("status", "DAV:");
            statusWrong.AddValue("HTTP/1.1 400 Not Found");
            propstatWrong.AddChild(statusWrong);

            #endregion

            #region Adding responseDescription when wrong

            var responseDescrpWrong = new XmlTreeStructure("responsedescription", "DAV:");
            responseDescrpWrong.AddValue("The properties doesn't  exist");
            propstatWrong.AddChild(responseDescrpWrong);

            #endregion

            #endregion

            //If anyone of the property groups is empty it is not included in the response.
            if (propertiesOk.Count > 0)
                treeChild.AddChild(propstatOk);
            if (propertiesWrong.Count > 0)
                treeChild.AddChild(propstatWrong);

            #endregion

            return treeChild;

            #endregion
        }

        /// <summary>
        /// Returns a Response XML tree for a prop request with all the property names
        ///     and property values specified in the request.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="propertiesNameNamespace"></param>
        /// <param name="principal"></param>
        /// <returns></returns>
        private async Task<IXMLTreeStructure> CHSetPropFillTree(string url, List<KeyValuePair<string, string>> propertiesNameNamespace, Principal principal)
        {
            //a "response xml element is added for each collection or resource"

            #region Adding the response of the collection or resource.

            var treeChild = new XmlTreeStructure("response", "DAV:");

            #region Adding the <D:href>/api/v1/caldav/{userEmail}/calendars/</D:href>

            //an href with the corresponding url is added to the response
            var href = new XmlTreeStructure("href", "DAV:");
            href.AddValue(SystemProperties._baseUrl + url);

            treeChild.AddChild(href);

            #endregion

            #region Adding the propstats

            #region Selecting properties

            var propertiesCol = new List<XmlTreeStructure>();
            var propertiesOk = new List<XmlTreeStructure>();
            var propertiesWrong = new List<XmlTreeStructure>();

            //the current-user-privilege-set is generated per request
            //it needs the DAV:acl property and the principalID
            Property aclProperty = null;

            //It take the list of requested properties and tries to get the corresponding property from db.
            //The methods are called for a resource or a collection accordingly its circumstances.
            //The properties are stored inside the propertiesCol. Where if the value is null it means that the collection could not be 
            //retrieve.

            //this is the calendar home set collection
            var calendarHome = _calendarHomeRepository.FindWithProperties(url);
            if (propertiesNameNamespace != null)
            {
                foreach (var addProperty in propertiesNameNamespace)
                {
                    //gets the property from database
                    var property = await _calendarHomeRepository.GetProperty(calendarHome.Id, addProperty);
                    //Builds the xmlTreeExtructure checking that if the value is null thats because 
                    //the property was not found.
                    IXMLTreeStructure prop;
                    if (property != null)
                        prop = property.Value == null
                            ? new XmlTreeStructure(property.Name, property.Namespace) { Value = "" }
                            : XmlTreeStructure.Parse(property.Value);
                    else
                    {
                        prop = new XmlTreeStructure(addProperty.Key, addProperty.Value);
                    }
                    propertiesCol.Add((XmlTreeStructure)prop);
                }
                //take the acl property
                aclProperty = calendarHome.Properties.FirstOrDefault(x => x.Name == "acl");
            }


            //add the additional properties that are generated per request
            if (propertiesNameNamespace != null)
                foreach (var pair in propertiesNameNamespace)
                {
                    switch (pair.Key)
                    {
                        case "current-user-privilege-set":
                            propertiesCol.RemoveAll(x => x.NodeName == pair.Key);
                            propertiesCol.Add(principal.GetCurrentUserPermissionProperty(aclProperty));
                            break;
                    }
                }


            //Here, properties are divided between recovered and error recovering
            foreach (var propTree in propertiesCol)
            {
                if (propTree.Value != null)
                    propertiesOk.Add(propTree);
                else
                    propertiesWrong.Add(propTree);
            }

            #endregion

            //For each returned status a "propstat" is created, containing a "prop" with all properties that belong to that current status.
            // And a "status" containing the message of the corresponding status.
            //Right Now there are only two "propstat" taking place OK and Wrong an therefore only two "status"
            //200 OK and 400 Not Found.
            //More should be added when ACL is working entairly.

            //TODO: Add the status forbidden for authentication permissions problems.

            #region Adding nested propOK

            var propstatOk = new XmlTreeStructure("propstat", "DAV:");
            var propOk = new XmlTreeStructure("prop", "DAV:");

            //Here i add all properties to the prop. 
            foreach (var property in propertiesOk)
            {
                propOk.AddChild(property);
            }

            propstatOk.AddChild(propOk);
            //This when i group the OK properties

            #region Adding nested status OK

            var statusOk = new XmlTreeStructure("status", "DAV:");
            statusOk.AddValue("HTTP/1.1 200 OK");
            propstatOk.AddChild(statusOk);

            #endregion

            #endregion

            //Here the same is made. The Wrong properties are grouped. 

            #region Adding nested propWrong

            var propstatWrong = new XmlTreeStructure("propstat", "DAV:");
            var propWrong = new XmlTreeStructure("prop", "DAV:");

            //Here i add all properties to the prop. 
            foreach (var property in propertiesWrong)
            {
                propWrong.AddChild(property);
            }

            propstatWrong.AddChild(propWrong);

            #region Adding nested status Not Found

            var statusWrong = new XmlTreeStructure("status", "DAV:");
            statusWrong.AddValue("HTTP/1.1 400 Not Found");
            propstatWrong.AddChild(statusWrong);

            #endregion

            #region Adding responseDescription when wrong

            var responseDescrpWrong = new XmlTreeStructure("responsedescription", "DAV:");
            responseDescrpWrong.AddValue("The properties doesn't  exist");
            propstatWrong.AddChild(responseDescrpWrong);

            #endregion

            #endregion

            //If anyone of the property groups is empty it is not included in the response.
            if (propertiesOk.Count > 0)
                treeChild.AddChild(propstatOk);
            if (propertiesWrong.Count > 0)
                treeChild.AddChild(propstatWrong);

            #endregion

            return treeChild;

            #endregion
        }

        #endregion
    }
}