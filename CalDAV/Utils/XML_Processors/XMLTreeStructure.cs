using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CalDAV.Utils.XML_Processors
{
    /// <summary>
    /// Tree structure class for parsing and 
    /// building the xml of the request and responses
    /// </summary>
    public class XMLTreeStructure :IXMLTreeStructure
    {
        #region Properties
        /// <summary>
        /// The name of the XML node.
        /// </summary>
        public string NodeName { get; set; }

        /// <summary>
        /// The namespaces of the node.
        /// </summary>
        public List<string> Namespaces { get; }

        /// <summary>
        /// The children of the node.
        /// </summary>
        public List<IXMLTreeStructure> Children { get; set; }
        #endregion

        public void AddChild(IXMLTreeStructure child)
        {
            Children.Add(child);
        }

        public IXMLTreeStructure GetChild(string childName)
        {
            throw new NotImplementedException();
        }

        public IXMLTreeStructure GetChildAtAnyLevel(string childName)
        {
            throw new NotImplementedException();
        }

       
      
    }
}
