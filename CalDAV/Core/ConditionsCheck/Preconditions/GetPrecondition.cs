using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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
