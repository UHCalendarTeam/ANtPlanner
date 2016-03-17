using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CalDAV.Core.ConditionsCheck
{
    interface IPostcondition
    {
        /// <summary>
        ///Checks that all postconditions passed. 
        /// </summary>
        /// <param name="propertiesAndHeaders"></param>
        /// <returns></returns>
        bool PostconditionOk(Dictionary<string, string> propertiesAndHeaders);
    }
}
