using System;
using System.Collections.Generic;
using System.IO;
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
        public XMLTreeStructure(string name, List<string> namespaces = null)
        {
            NodeName = name;
            Namespaces = namespaces ?? new List<string>();
            Children = new List<IXMLTreeStructure>();
        }

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

        public IXMLTreeStructure AddChild(IXMLTreeStructure child)
        {
            Children.Add(child);
            return this;
        }

        /// <summary>
        /// Get a child of this node by the given name.
        /// </summary>
        /// <param name="childName"></param>
        /// <returns></returns>
        public IXMLTreeStructure GetChild(string childName)
        {
            return Children.FirstOrDefault(x => x.NodeName == childName);
        }
        /// <summary>
        /// return the node at any a level with the given name.
        /// </summary>
        /// <param name="childName">The name of the node to be returned.</param>
        /// <returns></returns>
        public IXMLTreeStructure GetChildAtAnyLevel(string childName)
        {
            if (Children.Count == 0)
                return null;
            var result = GetChild(childName);
            if (result != null)
                return result;
            //search in the children and the children of them.
            return Children.Select(child => child.GetChildAtAnyLevel(childName)).FirstOrDefault(childResult => childResult != null);
        }

        public void AddNamespace(string nameSpace)
        {
            Namespaces.Add(nameSpace);
        }

      
       
      
    }
}
