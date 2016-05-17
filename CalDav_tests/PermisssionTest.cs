using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ACL.Core.Extension_Method;
using DataLayer;
using DataLayer.Models.ACL;
using TreeForXml;
using Xunit;

namespace CalDav_tests
{
    public class PermisssionTest
    {
        [Fact]
        public void TakingCurrentUserPrivilegeSet()
        {
            var principal = new Principal
            {
                PrincipalURL = "principals/users/principal1"
            };
            var aclProperty = PropertyCreation.CreateAclPropertyForUserCollections(principal.PrincipalURL);

            var permissions = principal.GetCurrentUserPermissionProperty(aclProperty);
            IXMLTreeStructure child = null;
            Assert.Equal(permissions.NodeName, "current-user-privilege-set");
            Assert.True(permissions.GetChildAtAnyLevel("read", out child));
            Assert.True(permissions.GetChildAtAnyLevel("write", out child));
        }

        [Fact]
        public void CheckReadPermission()
        {
            var list = new List<string>();
            var acesToCheck = new List<XElement>();
            XDocument xdoc = XDocument.Parse(@"<D:acl xmlns:D=""DAV:"">
           <D:ace>
             <D:principal>
               <D:href>adriano</D:href>
             </D:principal>
             <D:grant>
               <D:privilege><D:write/></D:privilege>
             </D:grant>
           </D:ace>
           <D:ace>
             <D:principal>
               <D:all/>
             </D:principal>
             <D:grant>
               <D:privilege><D:read/></D:privilege>
             </D:grant>
           </D:ace>
         </D:acl>");
            IEnumerable<XElement> principalGrantPermissions = null;
            XName aceName = "ace";

            //take the permission for the principal if any
            var descendants = xdoc.Descendants();
            var aces = descendants.Where(x => x.Name.LocalName == aceName).ToArray();

            //take the ACEs with the given principal
            acesToCheck.Add(aces.FirstOrDefault(ace => ace.Descendants()
                .FirstOrDefault(x => x.Name.LocalName == "href")?.Value == "adriano"));
            //take the ACEs granted to all the principals
            acesToCheck.Add(aces.FirstOrDefault(ace => ace.Descendants().FirstOrDefault(x => x.Name.LocalName == "principal")
                .Descendants().FirstOrDefault(x => x.Name.LocalName == "all") != null));
            var output = new List<string>();

            foreach (var ace in acesToCheck)
            {
                output.AddRange(ace.Descendants()
             .FirstOrDefault(x => x.Name.LocalName == "grant")
             .Descendants().Where(x => x.Name.LocalName == "privilege")
             .Descendants().Select(x => x.Name.LocalName));
            }


        }

    }
}