using System;
using System.Collections.Generic;
using Microsoft.AspNet.Http;

namespace CalDAV.Core.ConditionsCheck
{
    public class GetPrecondition : IPrecondition
    {
        public bool PreconditionsOK(Dictionary<string, string> propertiesAndHeaders, HttpResponse response)
        {
            throw new NotImplementedException();
        }
    }
}