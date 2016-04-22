using System.Collections.Generic;

namespace DataLayer
{
    internal interface ICollectionResourceProperties
    {
        string NameSpace { get; }

        /// <summary>
        ///     Returns all the properties of a resource that must be returned for
        ///     an "allprop" property method of Propfind.
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        List<KeyValuePair<string, string>> GetAllVisibleProperties();
    }
}