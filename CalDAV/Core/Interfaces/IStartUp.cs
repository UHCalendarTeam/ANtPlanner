using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CalDAV.Core
{
    public interface IStartUp
    {
        bool CreateUserInSystem(string userEmail, string userName, string userLastName);

        bool CreateCollectionForUser(string userEmail, string collectionName);


    }
}
