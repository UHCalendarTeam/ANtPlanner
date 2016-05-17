using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DataLayer.Models.ACL;
using DataLayer.Models.Entities;
using TreeForXml;

namespace ACL.Core.Extension_Method
{
    /// <summary>
    /// Contains some useful extension methods
    /// for the Principals.
    /// </summary>
    public static class PrincipalExtensionsMethods
    {
        /// <summary>
        ///     Get the permission for the given principal
        ///     in some resource.
        /// </summary>
        /// <param name="principal">The principal that wants to know his permissions.</param>
        /// <param name="property">The resource or collection's DAV:acl property</param>
        /// <returns>Return an I</returns>
        public static XmlTreeStructure GetCurrentUserPermissionProperty(this Principal principal, Property property)
        {
            var pUrl = principal.PrincipalURL;
            var aclP = XDocument.Parse(property.Value).Root;
            IEnumerable<XElement> principalGrantPermissions = null;
            XName aceName = "ace";

            //take the permission for the principal if any
            var descendants = aclP?.Descendants();
            var aces = descendants.Where(x => x.Name.LocalName == aceName);
            var principalAce = aces.FirstOrDefault(ace => ace.Descendants()
                .FirstOrDefault(x => x.Name.LocalName == "href")?.Value == pUrl);
            if (principalAce != null)
                principalGrantPermissions = principalAce.Descendants()
                    .FirstOrDefault(x => x.Name.LocalName == "grant")?.Elements();

            //take the permission for all users if any

            var output = new XElement("current-user-privilege-set", new XAttribute(XNamespace.Xmlns + "D", "DAV:"));


            //add the permission to the response
            if (principalGrantPermissions != null)
                foreach (var permission in principalGrantPermissions)
                {
                    output.Add(permission);
                }
            var outputStr = output.ToString();
            var xmlTree = XmlTreeStructure.Parse(outputStr) as XmlTreeStructure;
            return xmlTree;
        }


       
    }
}