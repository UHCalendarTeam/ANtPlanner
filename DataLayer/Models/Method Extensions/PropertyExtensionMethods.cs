using DataLayer.Models.Entities;
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
            return XmlTreeStructure.Parse(property.Value).Value;
        }
    }
}