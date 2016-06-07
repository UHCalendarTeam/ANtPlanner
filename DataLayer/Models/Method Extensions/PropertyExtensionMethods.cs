using System.IO;
using System.Linq;
using System.Xml.Linq;
using DataLayer.Models.Entities;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using TreeForXml;

namespace DataLayer
{
    /// <summary>
    ///     This class contains useful methods
    ///     that extend the behavior of the Property.
    /// </summary>
    public static class PropertyExtensionMethods
    {
        /// <summary>
        ///     Returns the real value of the given property.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static string PropertyRealValue(this Property property)
        {
            var temp = XDocument.Parse(property.Value).Root;
            var output = temp.Value;
            if (output != "" && temp.HasElements)
            {
                var s1 = temp.FirstNode.ToString();
                var s2 = temp.FirstNode.ToString(SaveOptions.DisableFormatting);
                var s3 = temp.FirstNode.ToString(SaveOptions.None);
                var s4 = temp.FirstNode.ToString(SaveOptions.OmitDuplicateNamespaces);
                return s1;
                
            }
            return output;
            //var temp = XmlTreeStructure.Parse(property.Value);
            //var output = temp.Value;
            //if (output == "" && temp.Children.Count > 0)
            //    return temp.Children.First().ToString();
            //return output;
        }
    }
}