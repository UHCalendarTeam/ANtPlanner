using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CalDAV.Core.ConditionsCheck
{
    public class GetPostcondition : IPostcondition
    {
        public bool PostconditionOk(Dictionary<string, string> propertiesAndHeaders)
        {

            return true;
        }
    }
}
