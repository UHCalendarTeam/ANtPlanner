using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CalDAV.Utils.XML_Processors
{
    /// <summary>
    /// Tree structure class for parsing and 
    /// building the xml of the request and responses
    /// </summary>
    public class XmlTreeStructure :IXMLTreeStructure
    {
        /// <summary>
        /// Used for building an XMLTreeStrucure from a Xml doc.
        /// </summary>
        /// <param name="doc">The string represnetation of the xml.</param>
        public  XmlTreeStructure(string doc)
        {
            var temp = XMLParsers.GenericParser(doc);
            Value = temp.Value;
            Attributes = temp.Attributes;
            Children = temp.Children;
            Namespaces = temp.Namespaces;
            NodeName = temp.NodeName;
        }

        /// <summary>
        /// Used for construct an object with the given ns.
        /// </summary>
        /// <param name="name">Name of the node.</param>
        /// <param name="nodeNamespace">Ns of the node.</param>
        public XmlTreeStructure(string name, string nodeNamespace)
        {
            NodeName = name;
            MainNamespace = nodeNamespace;
            Children = new List<IXMLTreeStructure>();
            Attributes = new Dictionary<string, string>();
            Namespaces = new Dictionary<string, string>();
        }
        
        /// <summary>
        /// Used for the first node of the object.  
        /// </summary>
        /// <param name="name">Name of the node</param>
        /// <param name="nodeNamespace">The main ns of the nonde(the one that the prefix points to).</param>
        /// <param name="namespaces">Namespaces of the node.</param>
        public XmlTreeStructure(string name,string nodeNamespace, Dictionary<string, string> namespaces)
        {
            NodeName = name;
            Namespaces = namespaces ?? new Dictionary<string, string>();
            MainNamespace = nodeNamespace;
            Children = new List<IXMLTreeStructure>();
            Attributes = new Dictionary<string, string>();
        }

       
        #region Properties
        /// <summary>
        /// The name of the XML node.
        /// </summary>
        public string NodeName { get; set; }
        
        /// <summary>
        /// This is for the ns that points the 
        /// prefix of the node.
        /// </summary>
        public string MainNamespace { get; set; }

        /// <summary>
        /// The namespaces of the node.
        /// </summary>
        public Dictionary<string,string> Namespaces { get; set; }

        /// <summary>
        /// The children of the node.
        /// </summary>
        public List<IXMLTreeStructure> Children { get; set; }


        public Dictionary<string, string> Attributes { get; set;  }
        public string Value { get;  set; }

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

        public IXMLTreeStructure AddNamespace(string nsLocal, string namespaceStr)
        {
            Namespaces.Add(nsLocal, namespaceStr);
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
            XDocument doc =new XDocument( ToXml(this));
            doc.Declaration = new XDeclaration("1.0", "utf-8", null);
            StringWriter writer = new Utf8StringWriter();
            doc.Save(writer, SaveOptions.None);
            return writer.ToString();
        }

      

        public XElement ToXml(IXMLTreeStructure parent)
        {
            XNamespace thisXNamespace = MainNamespace;
            XElement output = new XElement(thisXNamespace + NodeName, Value != string.Empty ? Value : null) ;
            foreach (var child in Children)
            {
                output.Add(child.ToXml(child));
            }
            foreach (var ns in Namespaces)
            {
                output.Add(new XAttribute(XNamespace.Xmlns+ns.Key,ns.Value));
            }
            foreach (var attribute in Attributes)
            {
                output.Add(new XAttribute(attribute.Key, attribute.Value ));
            }
            return output;
        }


        public static IXMLTreeStructure Parse(string doc)
        {
            var xml = XDocument.Parse(doc);

            var output = xmlWalker(xml.Root);
            return output;
        }

        private static IXMLTreeStructure xmlWalker(XElement node)
        {
            var output = new XmlTreeStructure(node.Name.LocalName, null)
            {
                Namespaces = node.Attributes().Where(x => x.IsNamespaceDeclaration).
                    ToDictionary(x => x.Name.LocalName, x => x.Value),
                Value = node.Value,
                MainNamespace = node.Name.NamespaceName
            };
            foreach (var attribute in node.Attributes().Where(x => !output.Namespaces.Keys.Contains(x.Name.LocalName)))
            {
                output.AddAttribute(attribute.Name.LocalName, attribute.Value);
            }
            var descendants = node.Elements();
            var descNodes = node.DescendantNodes();
            foreach (var descendant in descendants)
            {
                output.AddChild(xmlWalker(descendant));
            }
            return output;
        }



    }

    internal class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
}
