using System.Collections.Generic;
using System.Net;
using CalDAV.Core.Method_Extensions;
using DataLayer;
using Microsoft.AspNet.Http;
using Microsoft.Data.Entity;

namespace CalDAV.Core.ConditionsCheck
{
    public class MKCalendarPosCondition : IPoscondition
    {
        public MKCalendarPosCondition(IFileSystemManagement fs, CalDavContext db)
        {
            Db = db;
            Fs = fs;
        }

        public IFileSystemManagement Fs { get; }
        public DbContext Db { get; }

        public bool PosconditionOk(Dictionary<string, string> propertiesAndHeaders, HttpResponse response)
        {
            #region Extracting Properties

            string userEmail;
            propertiesAndHeaders.TryGetValue("userEmail", out userEmail);

            string collectionName;
            propertiesAndHeaders.TryGetValue("collectionName", out collectionName);

            string url;
            propertiesAndHeaders.TryGetValue("url", out url);

            #endregion

            if (!Fs.ExistCalendarCollection(url) ||
                !((CalDavContext) Db).CollectionExist(userEmail, collectionName))
            {
                response.StatusCode = (int) HttpStatusCode.Forbidden;
                response.Body.Write(@"<?xml version='1.0' encoding='UTF-8'?>
<error xmlns:D='DAV:' xmlns:C='urn:ietf:params:xml:ns:caldav'>
  <C:initialize-calendar-collection/>  
</error>");
                return false;
            }

            return true;
        }
    }
}