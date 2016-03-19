using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CalDAV.CALDAV_Properties
{
    interface ICollectionResourceProperties
    {
        string NameSpace { get; }

        /// <summary>
        /// Returns all the properties of a resource that must be returned for
        /// an "allprop" property method of Propfind. 
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        List<KeyValuePair<string, string>> GetAllVisibleProperties();
    }
}
