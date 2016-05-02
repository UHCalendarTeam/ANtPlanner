using System;
using System.Collections.Generic;
using DataLayer;
using DataLayer.Models.Entities;
using DataLayer.Repositories;
using Microsoft.AspNet.Http;
using Microsoft.Data.Entity;

namespace CalDAV.Core.ConditionsCheck
{
    public class GetPoscondition : IPoscondition
    {
        public GetPoscondition(IRepository<CalendarResource, string> resourceRepository , IFileSystemManagement fs)
        {
           _resourceRespository = resourceRepository as ResourceRespository;
            Fs = fs;
        }

        public IFileSystemManagement Fs { get; }

        private readonly ResourceRespository _resourceRespository;

        public bool PosconditionOk(Dictionary<string, string> propertiesAndHeaders, HttpResponse response)
        {
            throw new NotImplementedException();
        }
    }
}