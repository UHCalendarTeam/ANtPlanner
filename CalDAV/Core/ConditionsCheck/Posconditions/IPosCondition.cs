using System.Collections.Generic;
using System.Threading.Tasks;
using DataLayer;
using Microsoft.AspNetCore.Http;

namespace CalDAV.Core.ConditionsCheck
{
    internal interface IPoscondition
    {
        IFileManagement Fs { get; }

        /// <summary>
        ///     Checks that all postconditions passed.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        Task<bool> PosconditionOk(HttpContext httpContext);
    }
}