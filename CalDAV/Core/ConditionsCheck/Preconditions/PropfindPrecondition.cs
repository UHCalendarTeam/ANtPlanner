using System.Collections.Generic;
using System.Net;
using DataLayer;
using DataLayer.Models.Entities;
using DataLayer.Repositories;
using Microsoft.AspNet.Http;

namespace CalDAV.Core.ConditionsCheck.Preconditions
{
    public class PropfindPrecondition : IPrecondition
    {
        public PropfindPrecondition(IRepository<CalendarCollection, string>  collectionRepository, IRepository<CalendarResource, string> resourceRepository)
        {
            _collectionRepository = collectionRepository as CollectionRepository;
            _resourceRespository = resourceRepository as ResourceRespository;
        }

        private readonly CollectionRepository _collectionRepository;
        private readonly ResourceRespository _resourceRespository;


        public bool PreconditionsOK(Dictionary<string, string> propertiesAndHeaders, HttpResponse response)
        {
            string calendarResourceId;
            propertiesAndHeaders.TryGetValue("calendarResourceID", out calendarResourceId);

            string url;
            propertiesAndHeaders.TryGetValue("url", out url);

            if (calendarResourceId == null && !_collectionRepository.Exist(url).Result)
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
                return false;
            }
            if (calendarResourceId != null && !_resourceRespository.Exist(url).Result)
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
                return false;
            }
            return true;
        }
    }
}
