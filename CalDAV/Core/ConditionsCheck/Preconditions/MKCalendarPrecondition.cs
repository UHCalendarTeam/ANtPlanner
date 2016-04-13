using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DataLayer;
using TreeForXml;

namespace CalDAV.Core.ConditionsCheck
{
    public class MKCalendarPrecondition:IPrecondition
    {
        public IFileSystemManagement fs { get; }
        public CalDavContext db { get; }

        public MKCalendarPrecondition(IFileSystemManagement fileSystemManagement, CalDavContext context)
        {
            fs = fileSystemManagement;
            db = context;
        }

        public bool PreconditionsOK(Dictionary<string, string> propertiesAndHeaders, out KeyValuePair<HttpStatusCode, string> errorMessage)
        {
            #region Extracting Properties
            var userEmail = propertiesAndHeaders["userEmail"];
            var collectionName = propertiesAndHeaders["collectionName"];
            var body = propertiesAndHeaders["body"];
            #endregion

            errorMessage = new KeyValuePair<HttpStatusCode, string>();

            if (fs.SetUserAndCollection(userEmail, collectionName) || db.CollectionExist(userEmail, collectionName))
            {
                errorMessage = new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.Forbidden, @"<?xml version='1.0' encoding='UTF-8'?>
<error xmlns='DAV:'>
  <resource-must-be-null/>  
</error>");
                return false;
            }

            if (!string.IsNullOrEmpty(body))
            {
                var bodyTree = XmlTreeStructure.Parse(body);
                if (bodyTree == null)
                {
                    errorMessage = new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.Forbidden, "Wrong Body");
                    return false;
                }
                if (bodyTree.NodeName != "mkcalendar")
                {
                    errorMessage = new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.Forbidden, "Wrong Body");
                    return false;
                }
            }

            return true;
        }
    }
}
