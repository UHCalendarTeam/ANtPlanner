using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalDAV
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class CalDav:ICalDav
    {
        public string MkCalendar(string user, string collection, string body)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("This Method will create a new collection named " + collection + "\r");
            strBuilder.Append("The owner of this calendar will be " + user + "\r");
            strBuilder.Append("The body of the request containing calendar data is " + body);

            return strBuilder.ToString();
        }

        public string PropFind(string uri, string body)
        {
            throw new NotImplementedException();
        }

        public string Request(string uri, string body)
        {
            throw new NotImplementedException();
        }
    }
}
