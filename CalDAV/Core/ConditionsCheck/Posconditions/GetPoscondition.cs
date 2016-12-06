using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataLayer;
using DataLayer.Models.Entities;
using DataLayer.Models.Entities.ResourcesAndCollections;
using DataLayer.Models.Interfaces.Repositories;
using DataLayer.Models.Repositories;
using Microsoft.AspNetCore.Http;

namespace CalDAV.Core.ConditionsCheck
{
    public class GetPoscondition : IPoscondition
    {
        private readonly ICalendarResourceRepository _resourceRespository;

        public GetPoscondition(IRepository<CalendarResource, string> resourceRepository, IFileManagement fs)
        {
            _resourceRespository = resourceRepository as ICalendarResourceRepository;
            Fs = fs;
        }

        public IFileManagement Fs { get; }

        public Task<bool> PosconditionOk(HttpContext httpContext)
        {
            throw new NotImplementedException();
        }
    }
}