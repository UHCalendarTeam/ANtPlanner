using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CalDAV.Core.Method_Extensions;
using DataLayer;
using Microsoft.AspNet.Http;
using TreeForXml;

namespace CalDAV.Core.ConditionsCheck
{
    public class MKCalendarPrecondition : IPrecondition
    {
        public IFileSystemManagement fs { get; }
        public CalDavContext db { get; }

        public MKCalendarPrecondition(IFileSystemManagement fileSystemManagement, CalDavContext context)
        {
            fs = fileSystemManagement;
            db = context;
        }

        public bool PreconditionsOK(Dictionary<string, string> propertiesAndHeaders, HttpResponse response)
        {
            #region Extracting Properties
            var userEmail = propertiesAndHeaders["userEmail"];
            var collectionName = propertiesAndHeaders["collectionName"];
            var body = propertiesAndHeaders["body"];
            #endregion



            if (fs.SetUserAndCollection(userEmail, collectionName) || db.CollectionExist(userEmail, collectionName))
            {
                response.Body.Write(@"<?xml version='1.0' encoding='UTF-8'?>
<error xmlns='DAV:'>
  <resource-must-be-null/>  
</error>");
                response.StatusCode = (int)HttpStatusCode.Forbidden;
                return false;
            }

            if (!string.IsNullOrEmpty(body))
            {
                var bodyTree = XmlTreeStructure.Parse(body);
                if (bodyTree == null)
                {
                    response.StatusCode = (int) HttpStatusCode.Forbidden;
                    response.Body.Write("Wrong Body");
                    return false;
                }
                if (bodyTree.NodeName != "mkcalendar")
                {
                    response.StatusCode = (int)HttpStatusCode.Forbidden;
                    response.Body.Write("Wrong Body");
                    
                    return false;
                }
            }

            return true;
        }
    }
}
