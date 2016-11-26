using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CalDAV.Core.ConditionsCheck
{
    internal interface IPrecondition
    {
        /// <summary>
        ///     Checks that all preconditions passed.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        Task<bool> PreconditionsOK(HttpContext httpContext);
    }
}