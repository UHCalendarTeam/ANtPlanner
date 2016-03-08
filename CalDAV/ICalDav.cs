using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CalDAV
{
    public interface ICalDav
    {
        string MkCalendar(string userName, string collectionName,string body);
        string PropFind(string uri, string body);
        string Request(string uri, string body);
    }
}
