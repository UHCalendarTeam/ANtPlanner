using System.Collections.Generic;
using System.Net;
using CalDAV.Core.Method_Extensions;
using DataLayer;
using DataLayer.Models.Entities;
using DataLayer.Repositories;
using Microsoft.AspNet.Http;
using Microsoft.Data.Entity;

namespace CalDAV.Core.ConditionsCheck
{
    public class MKCalendarPosCondition : IPoscondition
    {
        public MKCalendarPosCondition(IFileSystemManagement fs, IRepository<CalendarCollection, string> collectionRepository )
        {
           _collectionRepository = collectionRepository as CollectionRepository;
            
            Fs = fs;
        }

        public IFileSystemManagement Fs { get; }
        private readonly CollectionRepository _collectionRepository;

        public bool PosconditionOk(Dictionary<string, string> propertiesAndHeaders, HttpResponse response)
        {
            #region Extracting Properties
            string url;
            propertiesAndHeaders.TryGetValue("url", out url);

            #endregion

            if (!Fs.ExistCalendarCollection(url) || _collectionRepository.Exist(url).Result)
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