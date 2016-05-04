using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataLayer;
using DataLayer.Models.Entities;
using DataLayer.Repositories;
using Microsoft.AspNet.Http;

namespace CalDAV.Core.ConditionsCheck
{
    public class GetPoscondition : IPoscondition
    {
        private readonly ResourceRespository _resourceRespository;

        public GetPoscondition(IRepository<CalendarResource, string> resourceRepository, IFileSystemManagement fs)
        {
            _resourceRespository = resourceRepository as ResourceRespository;
            Fs = fs;
        }

        public IFileSystemManagement Fs { get; }

        public Task<bool> PosconditionOk(Dictionary<string, string> propertiesAndHeaders, HttpResponse response)
        {
            throw new NotImplementedException();
        }
    }
}