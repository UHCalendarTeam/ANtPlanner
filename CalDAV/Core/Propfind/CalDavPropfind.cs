using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalDAV.CALDAV_Properties;
using CalDAV.Models;
using CalDAV.Utils.XML_Processors;

namespace CalDAV.Core.Propfind
{
    public class CalDavPropfind : IPropfindMethods
    {

        public void AllPropMethod(string userEmail, string collectionName, string calendarResourceId, int? depth, XmlTreeStructure multistatusTree)
        {

            //Here it is created the response body for the collection or resource
            //It depends if calendarResourceId == null.
            var primaryResponse = AllPropFillTree(userEmail, collectionName, calendarResourceId);

            //The response body is added to the result xml tree.
            multistatusTree.AddChild(primaryResponse);

            //Now I start putting all objectResource responses if the primary target was a collection
            //and if depth is greater than depth 0.
            #region Adding the responses for resources.

            if (calendarResourceId != null && depth == 1 || depth == -1)
            {
                CalendarCollection collection;
                using (var db = new CalDavContext())
                {
                    collection = db.GetCollection(userEmail, collectionName);
                }
                foreach (var calendarResource in collection.CalendarResources)
                {
                    var resourceResponse = AllPropFillTree(userEmail, collectionName, calendarResource.FileName);
                    multistatusTree.AddChild(resourceResponse);
                }
            }

            #endregion

        }

        public void PropMethod(string userEmail, string collectionName, int? depth, XmlTreeStructure propFindBody, XmlTreeStructure result)
        {
            throw new NotImplementedException();
        }

        public void PropNameMethod(string userEmail, string collectionName, string calendarResourceId, int? depth, XmlTreeStructure multistatusTree)
        {
            //Here it is created the response body for the collection or resource
            //It depends if calendarResourceId == null.
            var primaryResponse = PropNameFillTree(userEmail, collectionName, calendarResourceId);

            //The response body is added to the result xml tree.
            multistatusTree.AddChild(primaryResponse);

            //Now I start putting all objectResource responses if the primary target wwas a collection
            //and if depth is greater than depth 0.
            #region Adding the responses for resources.

            if (calendarResourceId != null && depth == 1 || depth == -1)
            {
                CalendarCollection collection;
                using (var db = new CalDavContext())
                {
                    collection = db.GetCollection(userEmail, collectionName);
                }
                foreach (var calendarResource in collection.CalendarResources)
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
            var treeChild = new XmlTreeStructure("response", "D");

            #region Adding the <D:href>/api/v1/caldav/{userEmail}/calendars/{collectionName}/{calendarResourceId}?</D:href>
            var href = new XmlTreeStructure("href", "D");

            if (calendarResourceId == null)
                href.AddValue("/api/v1/caldav/" + userEmail + "/calendars/" + collectionName + "/");
            else
                href.AddValue("/api/v1/caldav/" + userEmail + "/calendars/" + collectionName + "/" + calendarResourceId);

            treeChild.AddChild(href);
            #endregion

            #region Adding the propstat

            var propstat = new XmlTreeStructure("propstat", "D");

            #region Adding nested status
            var status = new XmlTreeStructure("status", "D");
            status.AddValue("HTTP/1.1 200 OK");
            propstat.AddChild(status);
            #endregion

            #region Adding nested prop
            var prop = new XmlTreeStructure("prop", "D");
            CalendarCollection collection;
            CalendarResource resource;
            List<XmlTreeStructure> properties;
            using (var db = new CalDavContext())
            {
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
            }

            propstat.AddChild(prop);
            #endregion

            treeChild.AddChild(propstat);
            #endregion

            return treeChild;

            #endregion
        }

        public void PropObjectResource(string userEmail, string collectionName, string calendarResourceId, XmlTreeStructure propFindBody, XmlTreeStructure result)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a Response XML element with all the property names
        /// and property values that the allprop method should contain.
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="collectionName"></param>
        /// <param name="calendarResourceId"></param>
        /// <returns></returns>
        private XmlTreeStructure AllPropFillTree(string userEmail, string collectionName, string calendarResourceId)
        {
            #region Adding the response of the collection or resource.
            var treeChild = new XmlTreeStructure("response", "D");

            #region Adding the <D:href>/api/v1/caldav/{userEmail}/calendars/{collectionName}/{calendarResourceId}?</D:href>
            var href = new XmlTreeStructure("href", "D");

            if (calendarResourceId == null)
                href.AddValue("/api/v1/caldav/" + userEmail + "/calendars/" + collectionName + "/");
            else
                href.AddValue("/api/v1/caldav/" + userEmail + "/calendars/" + collectionName + "/" + calendarResourceId);

            treeChild.AddChild(href);
            #endregion

            #region Adding the propstat

            var propstat = new XmlTreeStructure("propstat", "D");

            #region Adding nested status
            var status = new XmlTreeStructure("status", "D");
            status.AddValue("HTTP/1.1 200 OK");
            propstat.AddChild(status);
            #endregion

            #region Adding nested prop
            var prop = new XmlTreeStructure("prop", "D");
            CalendarCollection collection;
            CalendarResource resource;
            List<XmlTreeStructure> properties;
            using (var db = new CalDavContext())
            {
                if (calendarResourceId == null)
                {
                    collection = db.GetCollection(userEmail, collectionName);
                    properties = collection.GetAllVisibleProperties();
                }
                else
                {
                    resource = db.GetCalendarResource(userEmail, collectionName, calendarResourceId);
                    properties = resource.GetAllVisibleProperties();
                }

                //Here i add all properties to the prop. 
                foreach (var property in properties)
                {
                    prop.AddChild(property);
                }
            }

            propstat.AddChild(prop);
            #endregion

            treeChild.AddChild(propstat);
            #endregion

            return treeChild;

            #endregion
        }
        #endregion
    }
}
