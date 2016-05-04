using System.Collections.Generic;
using System.Threading.Tasks;
using DataLayer;
using Microsoft.AspNet.Http;

namespace CalDAV.Core.ConditionsCheck
{
    internal interface IPoscondition
    {
        IFileSystemManagement Fs { get; }

        /// <summary>
        ///     Checks that all postconditions passed.
        /// </summary>
        /// <param name="propertiesAndHeaders"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        Task<bool> PosconditionOk(Dictionary<string, string> propertiesAndHeaders, HttpResponse response);
    }
}