using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;

namespace CalDAV.Core.ConditionsCheck
{
    internal interface IPrecondition
    {
        /// <summary>
        ///     Checks that all preconditions passed.
        /// </summary>
        /// <param name="propertiesAndHeaders"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        Task<bool> PreconditionsOK(Dictionary<string, string> propertiesAndHeaders, HttpResponse response);
    }
}