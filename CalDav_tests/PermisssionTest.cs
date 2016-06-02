//using System.Collections.Generic;
//using System.Linq;
//using System.Xml.Linq;
//using ACL.Core.Extension_Method;
//using DataLayer;
//using DataLayer.Models.ACL;
//using TreeForXml;
//using Xunit;

//namespace CalDav_tests
//{
//    public class PermisssionTest
//    {
//        [Fact]
//        public void TakingCurrentUserPrivilegeSet()
//        {
//            var principal = new Principal
//            {
//                PrincipalURL = "principals/users/principal1"
//            };
//            var aclProperty = PropertyCreation.CreateAclPropertyForUserCollections(principal.PrincipalURL);

//            var permissions = principal.GetCurrentUserPermissionProperty(aclProperty);
//            IXMLTreeStructure child = null;
//            Assert.Equal(permissions.NodeName, "current-user-privilege-set");
//            Assert.True(permissions.GetChildAtAnyLevel("read", out child));
//            Assert.True(permissions.GetChildAtAnyLevel("write", out child));
//        }

//        [Fact]
//        public void CheckReadPermission()
//        {
//            var list = new List<string>();
//            var acesToCheck = new List<XElement>();
//            XDocument aclP = XDocument.Parse(@"<D:acl xmlns:D=""DAV:"">
//           <D:ace>
//             <D:principal>
//               <D:href>adriano</D:href>
//             </D:principal>
//             <D:grant>
//               <D:privilege><D:write/></D:privilege>
//             </D:grant>
//           </D:ace>
//           <D:ace>
//             <D:principal>
//               <D:all/>
//             </D:principal>
//             <D:grant>
//               <D:privilege><D:read/></D:privilege>
//             </D:grant>
//           </D:ace>
//         </D:acl>");
            
//            var principalGrantPermissions = new List<IEnumerable<XElement>>();
//            XName aceName = "ace";

//            //take the permission for the principal if any
//            var descendants = aclP?.Descendants();
//            var aces = descendants.Where(x => x.Name.LocalName == aceName);
//            var principalAce = aces.Where(ace => ace.Descendants()
//                .FirstOrDefault(x => x.Name.LocalName == "href")?.Value == "adriano" ||
//                ace.Descendants().FirstOrDefault(x => x.Name.LocalName == "principal")
//                    .Descendants().FirstOrDefault(x => x.Name.LocalName == "all") != null).ToArray();

//            if (principalAce.Any())
//                principalGrantPermissions.AddRange(principalAce.Select(element => element.Descendants().FirstOrDefault(x => x.Name.LocalName == "grant")?.Elements()));


//            //take the permission for all users if any
//            var output = new XElement("current-user-privilege-set", new XAttribute(XNamespace.Xmlns + "D", "DAV:"));


//            //add the permission to the response
//            if (principalGrantPermissions != null)
//                foreach (var permission in principalGrantPermissions)
//                {
//                    output.Add(permission);
//                }


//        }

//    }
//}