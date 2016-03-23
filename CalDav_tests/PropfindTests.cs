using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CalDAV.XML_Processors;
using TreeForXml;
using Xunit;

namespace CalDav_tests
{
    public class PropfindTests
    {
        [Fact]
        public void CreateRootWithNamespace()
        {
            ///Nachi cuando vayas a construir el primer nodo de XmlTreeStrucure
            ///le tienes que pasar el ns principal que es el que apunta el namePrefix del nodo
            /// en este caso fijate q el prefix de multistatus es D, so como este apunta a DAV:
            /// se le pasa como segundo parametro. Como tercer parametro le pasas un Dict con los 
            /// namespaces del nodo, las llaves seran los prefijos.
            XmlTreeStructure root = new XmlTreeStructure("multistatus","DAV:", new Dictionary<string, string>()
            {
                {"D", "DAV:" },
                {"C", "urn:ietf:params:xml:ns:caldav" }
            });
           
        }
    }
}
