using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            Attributes = new Dictionary<string, string>();
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


        public Dictionary<string, string> Attributes { get; }
        public string Value { get; private set; }

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

        public IXMLTreeStructure AddNamespace(string nameSpace)
        {
            Namespaces.Add(nameSpace);
            return this;
        }

        public IXMLTreeStructure AddAttribute(string name, string value)
        {
            Attributes.Add(name, value);
            return this;
        }

        public IXMLTreeStructure AddValue(string value)
        {
            Value = value;
            return this;
        }


        public override string ToString()
        {
            var strCh = new StringBuilder("\t");
            foreach (var child in Children)
            {
                strCh.Append(child.ToString() + "\n\t");
            }
            return NodeName + "\n\t" + strCh.ToString();
        }
    }
}
