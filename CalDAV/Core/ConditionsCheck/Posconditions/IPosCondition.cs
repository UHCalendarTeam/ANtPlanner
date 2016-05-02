using System.Collections.Generic;
using DataLayer;
using DataLayer.Repositories;
using Microsoft.AspNet.Http;
using Microsoft.Data.Entity;

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
        bool PosconditionOk(Dictionary<string, string> propertiesAndHeaders, HttpResponse response);
    }
}