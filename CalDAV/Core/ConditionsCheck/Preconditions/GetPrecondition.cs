using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CalDAV.Core.ConditionsCheck
{
    public class GetPrecondition : IPrecondition
    {
        public bool PreconditionsOK(Dictionary<string, string> propertiesAndHeaders, out KeyValuePair<HttpStatusCode, string> errorMessage)
        {
            throw new NotImplementedException();
        }
    }
}
