using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer;

namespace CalDAV.Core.ConditionsCheck
{
    public class MKCalendarPrecondition:IPrecondition
    {
        public IFileSystemManagement fs { get; }
        public CalDavContext db { get; }

        public MKCalendarPrecondition(IFileSystemManagement fileSystemManagement, CalDavContext context)
        {
            fs = fileSystemManagement;
            db = context;
        }

        public bool PreconditionsOK(Dictionary<string, string> propertiesAndHeaders)
        {
            #region Extracting Properties
            var userEmail = propertiesAndHeaders["userEmail"];
            var collectionName = propertiesAndHeaders["collectionName"];
            var body = propertiesAndHeaders["body"];
            #endregion

            if (fs.SetUserAndCollection(userEmail, collectionName))
            {
                return false;
            }

            



            return true;
        }
    }
}
