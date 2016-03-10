using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CalDAV.CALDAV_Properties
{
    public interface ICALDAVProperties
    {
        /// <summary>
        /// Identifies the set of collations supported by 
        /// the server for text matching operations.
        /// </summary>
        /// <returns></returns>
        string SupportedCollationSet();

    }
}
