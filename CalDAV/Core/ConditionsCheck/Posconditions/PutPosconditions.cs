using System;
using System.Collections.Generic;
using DataLayer;
using Microsoft.AspNet.Http;
using Microsoft.Data.Entity;

namespace CalDAV.Core.ConditionsCheck
{
    public class PutPosconditions : IPoscondition
    {
        public IFileSystemManagement Fs { get; }
     

        public bool PosconditionOk(Dictionary<string, string> propertiesAndHeaders, HttpResponse response)
        {
            throw new NotImplementedException();
        }
    }
}