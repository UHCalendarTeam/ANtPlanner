using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using DataLayer.Models.Entities;
using DataLayer.Repositories;
using Microsoft.AspNet.Http;

namespace CalDAV.Core.ConditionsCheck.Preconditions
{
    public class PropfindPrecondition : IPrecondition
    {
        private readonly CollectionRepository _collectionRepository;
        private readonly ResourceRespository _resourceRespository;

        public PropfindPrecondition(IRepository<CalendarCollection, string> collectionRepository,
            IRepository<CalendarResource, string> resourceRepository)
        {
            _collectionRepository = collectionRepository as CollectionRepository;
            _resourceRespository = resourceRepository as ResourceRespository;
        }


        public async Task<bool> PreconditionsOK(Dictionary<string, string> propertiesAndHeaders, HttpResponse response)
        {
            string calendarResourceId;
            propertiesAndHeaders.TryGetValue("calendarResourceID", out calendarResourceId);

            string url;
            propertiesAndHeaders.TryGetValue("url", out url);

            if (calendarResourceId == null && !await _collectionRepository.Exist(url))
            {
                response.StatusCode = (int) HttpStatusCode.NotFound;
                return false;
            }
            if (calendarResourceId != null && !await _resourceRespository.Exist(url))
            {
                response.StatusCode = (int) HttpStatusCode.NotFound;
                return false;
            }
            return true;
        }
    }
}