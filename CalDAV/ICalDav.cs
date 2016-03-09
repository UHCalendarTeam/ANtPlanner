using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CalDAV
{
    public interface ICalDav
    {
        string MkCalendar(string userName, string collectionName,string body);
        string PropFind(string body);
        string Request(string body);

        void AddCOR(string username, string collectionName, string resourceId,string body);

        string ReadCOR();
    }
}
