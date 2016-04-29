using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DataLayer.Models.Entities;
using Microsoft.Data.Entity.Scaffolding.Internal;
using TreeForXml;

namespace DataLayer
{
    /// <summary>
    /// Contains useful methods for the creation 
    /// of different properties
    /// </summary>
    public static class PropertyCreation
    {

        private static readonly Dictionary<string, string> Namespaces = new Dictionary<string, string>
        {
            {"D", @"xmlns:D=""DAV:"""},
            {"C", @"xmlns:C=""urn:ietf:params:xml:ns:caldav"""}
        };

        private static readonly Dictionary<string, string> NamespacesSimple = new Dictionary<string, string>
        {
            {"D", "DAV:"},
            {"C", "urn:ietf:params:xml:ns:caldav"}
        };

        /// <summary>
        ///     Create and return a DAV:group-membership property.
        ///  This protected property identifies the groups in 
        ///     which the principal is directly a member.
        /// </summary>
        /// <param name="groupHref"></param>
        /// <returns></returns>
        public static Property CreateGroupMembership(string groupHref)
        {
            var hrefNode = groupHref == "" ? "" : $"<D:href>{groupHref}</D:href>";
            var property = new Property("group-membership", "DAV:")
            {
                Value = $"<D:group-membership xmlns:D=\"DAV:\">{hrefNode}</D:group-membership>"
            };
            return property;
        }

        /// <summary>
        ///     Create and return a  DAV:group-member-set property.
        ///     This property of a group principal identifies the principals that are
        ///     direct members of a group.
        /// </summary>
        /// <returns></returns>
        public static Property CreateGroupMemberSet()
        {
            var property = new Property("group-member-set", "DAV:")
            {
                Value = $"<D:group-member-set xmlns:D=\"DAV:\"></D:group-member-set>"
            };
            return property;
        }

        /// <summary>
        ///  Create the c:calendar-home-set property
        ///     THis property says where to find the principal's calendars.
        /// </summary>
        /// <param name="pType">Says if the principal is gonna represents
        /// a user or a group </param>
        /// <param name="principalId">If the principal represents a group then send
        /// the name of the group, otherwise send the email of the user</param>
        /// <param name="calName">THe name of the calendar to be added in the url.</param>
        /// <returns></returns>
        public static Property CreateCalendarHomeSet(SystemProperties.PrincipalType pType, string principalId, string calName)
        {
            var property = new Property("calendar-home-set", "urn:ietf:params:xml:ns:caldav")
            {
                Value = $"<C:calendar-home-set xmlns:C=\"urn:ietf:params:xml:ns:caldav\" xmlns:D=\"DAV:\">" +
                        $"<D:href>{SystemProperties.BuildHomeSetUrl(pType, principalId)}{calName}/</D:href></C:calendar-home-set>"
            };
            return property;
        }

        /// <summary>
        /// Create the DAV:owner property for the collections
        /// and resources
        /// </summary>
        /// <param name="ownerHref">The owner href value.</param>
        /// <returns></returns>
        public static Property CreateOwner(string ownerHref)
        {
            var property = new Property("owner", "DAV:")
            {
                Value = $@"<D:owner xmlns:D=""DAV:""><D:href>{ownerHref}</D:href></D:owner>",
                IsVisible = true,
                IsMutable = false,
                IsDestroyable = false
            };
            return property;
        }

        /// <summary>
        /// Create a property instance with the given values
        /// </summary>
        /// <param name="nodeName">The name for the node. ej:displayname</param>
        /// <param name="nodePrefix">The ns prefix of the node. ej: the prefix of DAV: is D, so pass D</param>
        /// <param name="nodeValue">The value for the node. Clean value, dont construct the xml.</param>
        /// <param name="isDetr">Says if the property can be destroyed</param>
        /// <param name="isMutable">Says if the property can be modified.</param>
        /// <param name="isVisible">Says if the property is visible.</param>
        /// <returns></returns>
        public static Property CreateProperty(string nodeName, string nodePrefix, string nodeValue,
            bool isDetr=true, bool isMutable=true, bool isVisible = true)
        {
            var xmlValue = $"<{nodePrefix}:{nodeName} {Namespaces[nodePrefix]}>{nodeValue}</{nodePrefix}:{nodeName}>";
            return new Property(nodeName, NamespacesSimple[nodePrefix])
            {
                Value = xmlValue,
                IsMutable = isMutable,
                IsDestroyable = isDetr,
                IsVisible = isVisible
            };
        }

        /// <summary>
        /// Create the protected property DAV:supported-privilege-set
        /// This is a protected property that identifies the privileges 
        /// defined for the resource.
        /// </summary>
        /// <returns></returns>
        public static Property CreateSupportedPrivilegeSetForResources()
        {
            var property = new Property("supported-privilege-set", "DAV:")
            {
                IsDestroyable = false,
                IsVisible = true,
                IsMutable = false,
                Value = @"<D:supported-privilege-set xmlns:D=""DAV:"">
	<D:supported-privilege>
		<D:privilege><D:all/></D:privilege>
		<D:abstract/>
		<D:description xml:lang=""en"">
			Any operation
		</D:description>
		<D:supported-privilege>
			<D:privilege><D:read/></D:privilege>
			<D:description xml:lang=""en"">
				Read any object
			</D:description>
			<D:supported-privilege>
				<D:privilege><D:read-acl/></D:privilege>
				<D:abstract/>
				<D:description xml:lang=""en"">Read ACL</D:description>
			</D:supported-privilege>
			<D:supported-privilege>
				<D:privilege>
				<D:read-current-user-privilege-set/>
				</D:privilege>
				<D:abstract/>
				<D:description xml:lang=""en"">
					Read current user privilege set property
				</D:description>
			</D:supported-privilege>
		</D:supported-privilege>
		<D:supported-privilege>
			<D:privilege><D:write/></D:privilege>
			<D:description xml:lang=""en"">
				Write any object
			</D:description>
			<D:supported-privilege>
				<D:privilege><D:write-acl/></D:privilege>
				<D:description xml:lang=""en"">
					Write ACL
				</D:description>
				<D:abstract/>
			</D:supported-privilege>
			<D:supported-privilege>
				<D:privilege><D:write-properties/></D:privilege>
				<D:description xml:lang=""en"">
					Write properties
				</D:description>
			</D:supported-privilege>
			<D:supported-privilege>
				<D:privilege><D:write-content/></D:privilege>
				<D:description xml:lang=""en"">
				Write resource content
				</D:description>
			</D:supported-privilege>
		</D:supported-privilege>
		<D:supported-privilege>
			<D:privilege><D:unlock/></D:privilege>
			<D:description xml:lang=""en"">
				Unlock resource
			</D:description>
		</D:supported-privilege>
	</D:supported-privilege>
</D:supported-privilege-set>"
            };
            return property;
        }


        /// <summary>
        /// Create the DAV:acl property.
        /// THis is the default acl property for the collections  
        /// that its owner is a user.
        /// </summary>
        /// <param name="principalUrl">The principalUrl that is the owner of the collection</param>
        public static Property CreateAclPropertyForUserCollections(string principalUrl)
        {
            var aclProperty = new Property("acl", "DAV:")
            {
                IsVisible = true,
                IsMutable = false,
                IsDestroyable = false,
                Value = $@"<D:acl xmlns:D=""DAV"">
	<D:ace>
		<D:principal>
			<D:href>{principalUrl}</D:href>
		</D:principal>
		<D:grant>
			<D:privilege><D:write/></D:privilege>
			<D:privilege><D:read/></D:privilege>
		</D:grant>		
	</D:ace>
</D:acl>"
            };

            return aclProperty;
        }


        /// <summary>
        /// Create the DAV:acl property.
        /// THis is the default acl property for 
        /// the collections  that its owner is a group.
        /// </summary>
        /// <param name="editorUrl">The principalUrl that is editor in the system</param>
        public static Property CreateAclPropertyForGroupCollections(string editorUrl)
        {
            var aclProperty = new Property("acl", "DAV:")
            {
                IsVisible = true,
                IsMutable = false,
                IsDestroyable = false,
                Value = $@"<D:acl xmlns:D=""DAV"">
	<D:ace>
		<D:principal>
			<D:href>{editorUrl}</D:href>
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
</D:acl>"
            };

            return aclProperty;
        }

        /// <summary>
        /// Create the DAV:current-user-principal.
        /// If not principalUrl is provided means that the 
        /// user is not authenticated.
        /// </summary>
        /// <param name="principalUrl">The current principal url, or nothing if the principal is not
        /// authenticated.</param>
        /// <returns></returns>
        public static IXMLTreeStructure CreateCurrentUserPrincipal(string principalUrl = "")
        {
            var output = new XmlTreeStructure("current-user-principal", "DAV:");

            IXMLTreeStructure href;
            //if not principal URL is provided means that
            //the principal is not authenticated.
            if (principalUrl != "")
                href = new XmlTreeStructure("href", "DAV:")
                {
                    Value = principalUrl
                };
            
            else
                href = new XmlTreeStructure("unauthenticated", "DAV:");
            output.AddChild(href);
            return href;
        }




        

        /// <summary>
        /// Add a single node to this property
        /// </summary>
        /// <param name="property"></param>
        /// <param name="nodeValue"></param>
        /// <param name="nodeName"></param>
        /// <param name="ns"></param>
        /// <returns></returns>
        //public static bool AddNodeToProperty(this Property property, string nodeValue,string nodeName, string ns)
        //{
        //    var xml = XDocument.Parse(property.Value);
        //}
    }
}
