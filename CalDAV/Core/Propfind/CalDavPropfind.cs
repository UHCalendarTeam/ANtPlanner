using System.Collections.Generic;
using CalDAV.CALDAV_Properties;
using CalDAV.Models;
using TreeForXml;

namespace CalDAV.Core.Propfind
{
    public class CalDavPropfind : IPropfindMethods
    {
        CalDavContext db;

        public CalDavPropfind(CalDavContext context)
        {
            db = context;
        }

        public void AllPropMethod(string userEmail, string collectionName, string calendarResourceId, int? depth, List<KeyValuePair<string, string>> aditionalProperties, XmlTreeStructure multistatusTree)
        {
            //error flag
            bool errorOcurred = false;

            //Here it is created the response body for the collection or resource
            //It depends if calendarResourceId == null.
            var primaryResponse = AllPropFillTree(userEmail, collectionName, calendarResourceId, aditionalProperties);

            //The response body is added to the result xml tree.
            multistatusTree.AddChild(primaryResponse);

            //check if there was any error
            IXMLTreeStructure errorNode = null;
            errorOcurred = primaryResponse.GetChildAtAnyLevel("responsedescription", out errorNode);

            //Now I start putting all objectResource responses if the primary target was a collection
            //and if depth is greater than depth 0.
            #region Adding the responses for resources.

            if (calendarResourceId != null && depth == 1 || depth == -1)
            {
                CalendarCollection collection;

                //TODO: Trying to get db by dependency injection
                // using (var db = new CalDavContext())
                //   {
                collection = db.GetCollection(userEmail, collectionName);
                //  }
                foreach (var calendarResource in collection.Calendarresources)
                {
                    var resourceResponse = AllPropFillTree(userEmail, collectionName, calendarResource.FileName, aditionalProperties);
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
            }

            #endregion

        }

        public void PropMethod(string userEmail, string collectionName, string calendarResourceId, int? depth, List<KeyValuePair<string, string>> propertiesReq, XmlTreeStructure multistatusTree)
        {
            //error flag
            bool errorOcurred = false;

            //Here it is created the response body for the collection or resource
            //It depends if calendarResourceId == null.
            var primaryResponse = PropFillTree(userEmail, collectionName, calendarResourceId, propertiesReq);

            //The response body is added to the result xml tree.
            multistatusTree.AddChild(primaryResponse);

            //check if there was any error
            IXMLTreeStructure errorNode;
            errorOcurred = primaryResponse.GetChildAtAnyLevel("responsedescription", out errorNode);

            //Now I start putting all objectResource responses if the primary target was a collection
            //and if depth is greater than depth 0.
            #region Adding the responses for resources.

            if (calendarResourceId != null && depth == 1 || depth == -1)
            {
                CalendarCollection collection;
                //TODO: Trying to get db by dependency injection
                //  using (var db = new CalDavContext())
                //   {
                collection = db.GetCollection(userEmail, collectionName);
                //   }
                foreach (var calendarResource in collection.Calendarresources)
                {
                    var resourceResponse = PropFillTree(userEmail, collectionName, calendarResource.FileName, propertiesReq);
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

            }

            #endregion
        }

        public void PropNameMethod(string userEmail, string collectionName, string calendarResourceId, int? depth, XmlTreeStructure multistatusTree)
        {
            //Here it is created the response body for the collection or resource
            //It depends if calendarResourceId == null.
            var primaryResponse = PropNameFillTree(userEmail, collectionName, calendarResourceId);

            //The response body is added to the result xml tree.
            multistatusTree.AddChild(primaryResponse);

            //Now I start putting all objectResource responses if the primary target was a collection
            //and if depth is greater than depth 0.
            #region Adding the responses for resources.

            if (calendarResourceId != null && depth == 1 || depth == -1)
            {
                CalendarCollection collection;
                //TODO: Trying to get db by dependency injection
                //  using (var db = new CalDavContext())
                //  {
                collection = db.GetCollection(userEmail, collectionName);
                //  }
                foreach (var calendarResource in collection.Calendarresources)
                {
                    var resourceResponse = PropNameFillTree(userEmail, collectionName, calendarResource.FileName);
                    multistatusTree.AddChild(resourceResponse);
                }
            }

            #endregion
        }



        #region Calendar Object Resource Propfind Methods.

        /// <summary>
        /// Returns a Response XML element with the name of all properties
        /// of a collection or resource.
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="collectionName"></param>
        /// <param name="calendarResourceId"></param>
        /// <returns></returns>
        private XmlTreeStructure PropNameFillTree(string userEmail, string collectionName, string calendarResourceId)
        {
            #region Adding the response of the collection or resource.
            var treeChild = new XmlTreeStructure("response", "DAV:");

            #region Adding the <D:href>/api/v1/caldav/{userEmail}/calendars/{collectionName}/{calendarResourceId}?</D:href>
            var href = new XmlTreeStructure("href", "DAV:");

            if (calendarResourceId == null)
                href.AddValue("/api/v1/caldav/" + userEmail + "/calendars/" + collectionName + "/");
            else
                href.AddValue("/api/v1/caldav/" + userEmail + "/calendars/" + collectionName + "/" + calendarResourceId);

            treeChild.AddChild(href);
            #endregion

            #region Adding the propstat

            var propstat = new XmlTreeStructure("propstat", "DAV:");

            #region Adding nested status
            var status = new XmlTreeStructure("status", "DAV:");
            status.AddValue("HTTP/1.1 200 OK");
            propstat.AddChild(status);
            #endregion

            #region Adding nested prop
            var prop = new XmlTreeStructure("prop", "DAV:");
            CalendarCollection collection;
            CalendarResource resource;
            List<XmlTreeStructure> properties;
            //TODO: Trying to get db by dependency injection
            // using (var db = new CalDavContext())
            //  {
            if (calendarResourceId == null)
            {
                collection = db.GetCollection(userEmail, collectionName);
                properties = collection.GetAllPropertyNames();
            }
            else
            {
                resource = db.GetCalendarResource(userEmail, collectionName, calendarResourceId);
                properties = resource.GetAllPropertyNames();
            }

            //Here i add all properties to the prop. 
            foreach (var property in properties)
            {
                prop.AddChild(property);
            }
            //  }

            propstat.AddChild(prop);
            #endregion

            treeChild.AddChild(propstat);
            #endregion

            return treeChild;

            #endregion
        }

        /// <summary>
        /// Returns a Response XML element with all the property names
        /// and property values that the allprop method should contain.
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="collectionName"></param>
        /// <param name="calendarResourceId"></param>
        /// <param name="additionalProperties"></param>
        /// <returns></returns>
        private XmlTreeStructure AllPropFillTree(string userEmail, string collectionName, string calendarResourceId, List<KeyValuePair<string, string>> additionalProperties)
        {
            #region Adding the response of the collection or resource.
            var treeChild = new XmlTreeStructure("response", "DAV:");

            #region Adding the <D:href>/api/v1/caldav/{userEmail}/calendars/{collectionName}/{calendarResourceId}?</D:href>
            var href = new XmlTreeStructure("href", "DAV:");

            if (calendarResourceId == null)
                href.AddValue("/api/v1/caldav/" + userEmail + "/calendars/" + collectionName + "/");
            else
                href.AddValue("/api/v1/caldav/" + userEmail + "/calendars/" + collectionName + "/" + calendarResourceId);

            treeChild.AddChild(href);
            #endregion

            #region Adding the propstat

            #region Selecting properties

            var propertiesCol = new List<XmlTreeStructure>();
            List<XmlTreeStructure> propertiesOk;
            var propertiesWrong = new List<XmlTreeStructure>();

            //TODO: Trying to get db by dependency injection
            // using (var db = new CalDavContext())
            // {
            if (calendarResourceId == null)
            {
                var collection = db.GetCollection(userEmail, collectionName);
                propertiesOk = collection.GetAllVisibleProperties();
                if (additionalProperties != null && additionalProperties.Count > 0)
                    foreach (var property in additionalProperties)
                    {
                        propertiesCol.Add(collection.ResolveProperty(property.Key, property.Value));
                    }
            }
            else
            {
                var resource = db.GetCalendarResource(userEmail, collectionName, calendarResourceId);
                propertiesOk = resource.GetAllVisibleProperties();
                if (additionalProperties != null && additionalProperties.Count > 0)
                    foreach (var property in additionalProperties)
                    {
                        propertiesCol.Add(resource.ResolveProperty(property.Key, "DAV:"));
                    }
            }
            // }

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
            var propstatOK = new XmlTreeStructure("propstat", "DAV:");
            var propOk = new XmlTreeStructure("prop", "DAV:");

            //Here i add all properties to the prop. 
            foreach (var property in propertiesOk)
            {
                propOk.AddChild(property);
            }

            propstatOK.AddChild(propOk);

            #region Adding nested status OK
            var statusOK = new XmlTreeStructure("status", "DAV:");
            statusOK.AddValue("HTTP/1.1 200 OK");
            propstatOK.AddChild(statusOK);
            #endregion

            #endregion

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


            if (propertiesOk.Count > 0)
                treeChild.AddChild(propstatOK);
            if (propertiesWrong.Count > 0)
                treeChild.AddChild(propstatWrong);
            #endregion

            return treeChild;

            #endregion
        }

        /// <summary>
        /// Returns a Response XML tree with all the property names
        /// and property values specified in the request.
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="collectionName"></param>
        /// <param name="calendarResourceId"></param>
        /// <param name="propertiesNameNamespace"></param>
        /// <returns></returns>
        private XmlTreeStructure PropFillTree(string userEmail, string collectionName, string calendarResourceId, List<KeyValuePair<string, string>> propertiesNameNamespace)
        {
            #region Adding the response of the collection or resource.
            var treeChild = new XmlTreeStructure("response", "DAV:");

            #region Adding the <D:href>/api/v1/caldav/{userEmail}/calendars/{collectionName}/{calendarResourceId}?</D:href>
            var href = new XmlTreeStructure("href", "DAV:");

            if (calendarResourceId == null)
                href.AddValue("/api/v1/caldav/" + userEmail + "/calendars/" + collectionName + "/");
            else
                href.AddValue("/api/v1/caldav/" + userEmail + "/calendars/" + collectionName + "/" + calendarResourceId);

            treeChild.AddChild(href);
            #endregion

            #region Adding the propstats

            #region Selecting properties
            CalendarCollection collection;
            CalendarResource resource;
            List<XmlTreeStructure> propertiesCol = new List<XmlTreeStructure>();
            List<XmlTreeStructure> propertiesOk = new List<XmlTreeStructure>();
            List<XmlTreeStructure> propertiesWrong = new List<XmlTreeStructure>();

            //TODO: Trying to get db by dependency injection
            //using (var db = new CalDavContext())
            //{
            if (calendarResourceId == null)
            {
                collection = db.GetCollection(userEmail, collectionName);
                if (propertiesNameNamespace != null)
                    foreach (var property in propertiesNameNamespace)
                    {
                        propertiesCol.Add(collection.ResolveProperty(property.Key, property.Value));
                    }
            }
            else
            {
                resource = db.GetCalendarResource(userEmail, collectionName, calendarResourceId);
                if (propertiesNameNamespace != null)
                    foreach (var property in propertiesNameNamespace)
                    {
                        propertiesCol.Add(resource.ResolveProperty(property.Key, "DAV:"));
                    }
            }
            //}

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
