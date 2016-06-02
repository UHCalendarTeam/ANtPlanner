using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataLayer;
using Microsoft.AspNetCore.Http;

namespace CalDAV.Core.ConditionsCheck
{
    public class PutPosconditions : IPoscondition
    {
        public IFileManagement Fs { get; }


        public async Task<bool> PosconditionOk(Dictionary<string, string> propertiesAndHeaders, HttpResponse response)
        {
            throw new NotImplementedException();
        }
    }
}