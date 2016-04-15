using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using DataLayer;
using Microsoft.Data.Entity;

namespace CalDAV.Core.ConditionsCheck
{
    interface IPoscondition
    {
        IFileSystemManagement Fs { get; }
        DbContext Db { get; }

        ///  <summary>
        /// Checks that all postconditions passed. 
        ///  </summary>
        /// <param name="propertiesAndHeaders"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        bool PosconditionOk(Dictionary<string, string> propertiesAndHeaders, out KeyValuePair<HttpStatusCode, string> errorMessage);
    }
}
