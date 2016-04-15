using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DataLayer;
using Microsoft.Data.Entity;

namespace CalDAV.Core.ConditionsCheck
{
    public class MKCalendarPosCondition : IPoscondition
    {
        public IFileSystemManagement Fs { get; }
        public DbContext Db { get; }

        public MKCalendarPosCondition(IFileSystemManagement fs, CalDavContext db)
        {
            Db = db;
            Fs = fs;
        }

        public bool PosconditionOk(Dictionary<string, string> propertiesAndHeaders, out KeyValuePair<HttpStatusCode, string> errorMessage)
        {
            #region Extracting Properties
            string userEmail;
            propertiesAndHeaders.TryGetValue("userEmail", out userEmail);

            string collectionName;
            propertiesAndHeaders.TryGetValue("collectionName", out collectionName);

            string url;
            propertiesAndHeaders.TryGetValue("url", out url);
            #endregion

            errorMessage = new KeyValuePair<HttpStatusCode, string>();

            if (!Fs.SetUserAndCollection(userEmail, collectionName) ||
                !((CalDavContext) Db).CollectionExist(userEmail, collectionName))
            {
                errorMessage = new KeyValuePair<HttpStatusCode, string>(HttpStatusCode.Forbidden, @"<?xml version='1.0' encoding='UTF-8'?>
<error xmlns:D='DAV:' xmlns:C='urn:ietf:params:xml:ns:caldav'>
  <C:initialize-calendar-collection/>  
</error>");
                return false;
            }

            return true;
        }
    }
}
