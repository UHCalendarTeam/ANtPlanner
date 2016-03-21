using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalDAV.CALDAV_Properties;
using CalDAV.Models;
using CalDAV.Utils.XML_Processors;
using CalDAV.XML_Processors;
using TreeForXml;

namespace CalDAV.Core.Propfind
{
    public class CalDavPropfind : IPropfindMethods
    {

        public void AllPropMethod(string userEmail, string collectionName, string calendarResourceId, int? depth, XmlTreeStructure multistatusTree)
        {
            if (calendarResourceId == null)
            {
                //Here it is created the response body for the collection.
                var collectionResponse = AllPropFillTree(userEmail, collectionName, null);

                //The response body is added to the result xml tree.
                multistatusTree.AddChild(collectionResponse);

                //Now I start putting all objectResource responses.
                #region Adding the responses for resources.

                if (depth == 1 || depth == -1)
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
                else
                {
                    var resourceResponse = AllPropFillTree(userEmail, collectionName, calendarResourceId);

                    multistatusTree.AddChild(resourceResponse);
                }

                #endregion
            }


        }

        public void PropMethod(string userEmail, string collectionName, int? depth, XmlTreeStructure propFindBody, XmlTreeStructure result)
        {
            throw new NotImplementedException();
        }

        public void PropNameMethod(string userEmail, string collectionName, int? depth, XmlTreeStructure result)
        {
            throw new NotImplementedException();
        }

        #region Calendar Object Resource Propfind Methods.
        public void PropNameObjectResource(string userEmail, string collectionName, string calendarResourceId, XmlTreeStructure result)
        {
            throw new NotImplementedException();
        }

        public void PropObjectResource(string userEmail, string collectionName, string calendarResourceId, XmlTreeStructure propFindBody, XmlTreeStructure result)
        {
            throw new NotImplementedException();
        }

        private XmlTreeStructure AllPropFillTree(string userEmail, string collectionName, string calendarResourceId)
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
            List<KeyValuePair<string, string>> properties;
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

                //Here i put flat propeties in the tree structure
                //TODO: Place not flat properties. 
                foreach (var property in properties)
                {
                    var nestedProp = new XmlTreeStructure(property.Key, "DAV:");
                    nestedProp.AddValue(property.Value);
                    prop.AddChild(nestedProp);
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
