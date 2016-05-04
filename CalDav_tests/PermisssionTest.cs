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

            var permissions = principal.GetCurrentUserPermissions(aclProperty);
            IXMLTreeStructure child = null;
            Assert.Equal(permissions.NodeName, "current-user-privilege-set");
            Assert.True(permissions.GetChildAtAnyLevel("read", out child));
            Assert.True(permissions.GetChildAtAnyLevel("write", out child));
        }
    }
}