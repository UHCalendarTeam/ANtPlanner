using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using CalDAV.Core.Method_Extensions;
using DataLayer;
using DataLayer.Models.Entities;
using DataLayer.Repositories;
using Microsoft.AspNetCore.Http;

namespace CalDAV.Core.ConditionsCheck
{
    public class MKCalendarPosCondition : IPoscondition
    {
        private readonly CollectionRepository _collectionRepository;

        public MKCalendarPosCondition(IFileManagement fs,
            IRepository<CalendarCollection, string> collectionRepository)
        {
            _collectionRepository = collectionRepository as CollectionRepository;

            Fs = fs;
        }

        public IFileManagement Fs { get; }

        public async Task<bool> PosconditionOk(HttpContext httpContext)
        {
            #region Extracting Properties

            string url;
            propertiesAndHeaders.TryGetValue("url", out url);

            #endregion

            if (!Fs.ExistCalendarCollection(url) || await _collectionRepository.Exist(url))
            {
                httpContext.Response.StatusCode = (int) HttpStatusCode.Forbidden;
                httpContext.Response.Body.Write(@"<?xml version='1.0' encoding='UTF-8'?>
<error xmlns:D='DAV:' xmlns:C='urn:ietf:params:xml:ns:caldav'>
  <C:initialize-calendar-collection/>  
</error>");
                return await Task.FromResult(false);
            }

            return await Task.FromResult(true);
        }
    }
}