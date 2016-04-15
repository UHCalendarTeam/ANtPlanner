using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CalDAV.Core.ConditionsCheck
{
    interface IPrecondition
    {
        ///  <summary>
        /// Checks that all preconditions passed. 
        ///  </summary>
        /// <param name="propertiesAndHeaders"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        bool PreconditionsOK(Dictionary<string, string> propertiesAndHeaders, out KeyValuePair<HttpStatusCode, string> errorMessage);
    }
}
