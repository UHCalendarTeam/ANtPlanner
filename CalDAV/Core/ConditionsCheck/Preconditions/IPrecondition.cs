using System.Collections.Generic;
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
        bool PreconditionsOK(Dictionary<string, string> propertiesAndHeaders, HttpResponse response);
    }
}