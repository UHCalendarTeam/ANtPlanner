using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ACL.Interfaces
{
    /// <summary>
    /// This specification defines a number of new properties for WebDAV
    ///resources.Access control properties may be retrieved just like
    ///other WebDAV properties, using the PROPFIND method.
    /// </summary>
   public interface IAcl
    {
        /// <summary>
        /// This property identifies a particular principal as being the "owner"
        /// of the resource.
        /// </summary>
        /// <returns>
        /// Should return the URL of the principal,
        /// or string.Empty if there is no owner.
        /// </returns>
        string GetOwner();

        /// <summary>
        /// This property identifies a particular principal as being the "group"
        /// of the resource.
        /// </summary>
        /// <returns>
        /// The URL of a principal that identify a group.
        /// string.Empty if not exist.
        /// </returns>
        string GetGroup();

        /// <summary>
        /// This is a protected property that identifies the privileges defined
        ///for the resource.
        /// </summary>
        /// <returns></returns>
        string GetSupportedPriviledSet();
    }
}
