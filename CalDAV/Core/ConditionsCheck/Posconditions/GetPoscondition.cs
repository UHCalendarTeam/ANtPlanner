using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DataLayer;
using Microsoft.AspNet.Http;
using Microsoft.Data.Entity;

namespace CalDAV.Core.ConditionsCheck
{
    public class GetPoscondition : IPoscondition
    {
        public IFileSystemManagement Fs { get; }
        public DbContext Db { get; }

        public bool PosconditionOk(Dictionary<string, string> propertiesAndHeaders, HttpResponse response)
        {
           throw new NotImplementedException();
        }
    }
}
