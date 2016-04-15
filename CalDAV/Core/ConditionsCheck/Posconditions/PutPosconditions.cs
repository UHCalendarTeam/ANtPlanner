using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DataLayer;
using Microsoft.Data.Entity;

namespace CalDAV.Core.ConditionsCheck
{
    public class PutPosconditions : IPoscondition
    {
        public IFileSystemManagement Fs { get; }
        public DbContext Db { get; }

        public bool PosconditionOk(Dictionary<string, string> propertiesAndHeaders, out KeyValuePair<HttpStatusCode, string> errorMessage)
        {
            throw new NotImplementedException();
        }
    }
}
