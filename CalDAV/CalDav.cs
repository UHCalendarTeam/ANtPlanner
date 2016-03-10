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
        /// <summary>
        /// CalDAV HTTP Method for create a new collection of COR (new calendar)
        /// </summary>
        /// <param name="user"></param>
        /// <param name="collection"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public string MkCalendar(string user, string collection, string body)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("This Method will create a new collection named " + collection + "\r");
            strBuilder.Append("The owner of this calendar will be " + user + "\r");
            strBuilder.Append("The body of the request containing calendar data is " + body);

            return strBuilder.ToString();
        }

        /// <summary>
        /// WebDAV HTTP Method
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public string PropFind(string body)
        {
            //getetag property return the etag of the COR for sincro options.
            throw new NotImplementedException();
        }

        /// <summary>
        /// CalDAV Request HTTP Method
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public string Request(string body)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// CalDAV PUT for create a new COR
        /// </summary>
        /// <param name="username"></param>
        /// <param name="collectionName"></param>
        /// <param name="resourceId"></param>
        /// <param name="body"></param>
        public void AddCOR(string username, string collectionName, string resourceId,string body)
        {
            //Note: calendar object resource = COR

            //check that resourceId don't exist but the collection does.
            //other case it updates the specified COR.
            //if the client intend to create (not update) a new COR it SHOULD add a HTTP header "If-None-Match : *" 
            // it secures that it will not update an COR in case that the id already exist without notice the client.
            //for an update use "If-Match" and the specific Etag
            //when created or updated the response header MUST have the Etag

            //body is a clean(without XML) iCalendar
            
            //return HTTP 201 Created 

            throw new NotImplementedException();
        }

        /// <summary>
        /// CalDAv HTTP Method Get for a COR
        /// </summary>
        public string ReadCOR()
        {
            //Must return the Etag header of the COR
            throw new NotImplementedException();
        }

        /// <summary>
        /// CalDav Move
        /// </summary>
        public void Move ()
        { }
       
    }
}
