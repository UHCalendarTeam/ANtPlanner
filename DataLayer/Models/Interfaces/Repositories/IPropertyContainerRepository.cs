using System.Collections.Generic;
using System.Threading.Tasks;
using DataLayer.Models.Entities;

namespace DataLayer.Models.Interfaces.Repositories
{

    public interface IPropertyContainerRepository<TEnt,in TKey> :IRepository<TEnt, TKey> where TEnt : class,IPropertyContainer
    {
        /// <summary>
        ///     Returns all the visible properties
        ///     related to the given url.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<IList<Property>> GetAllProperties(TKey key);

        TEnt FindWithProperties(TKey key);

        Task<TEnt> FindWithPropertiesAsync(TKey key);
        /// <summary>
        ///     Returns the property that match the given
        ///     property name and namespace for the given url;
        /// </summary>
        /// <param name="key"></param>
        /// <param name="propertyNameandNs"></param>
        /// <returns></returns>
        Task<Property> GetProperty(TKey key, KeyValuePair<string, string> propertyNameandNs);

        /// <summary>
        ///     Returns all the properties
        /// </summary>
        /// <returns></returns>
        Task<IList<KeyValuePair<string, string>>> GetAllPropname(TKey key);

        /// <summary>
        ///     Remove a property with the given name and namespace.
        /// </summary>
        /// <param name="key">The object's identifier</param>
        /// <param name="propertyNameNs">The property name and namespace.</param>
        /// <param name="errorStack">The error stack.</param>
        Task<bool> RemoveProperty(TKey key, KeyValuePair<string, string> propertyNameNs, Stack<string> errorStack);

        /// <summary>
        ///     Create a modify a property
        /// </summary>
        /// <param name="key">The property father url.</param>
        /// <param name="propName"></param>
        /// <param name="propNs"></param>
        /// <param name="propValue"></param>
        /// <param name="errorStack"></param>
        /// <param name="adminPrivilege"></param>
        /// <returns></returns>
        Task<bool> CreateOrModifyProperty(TKey key, string propName, string propNs, string propValue,
            Stack<string> errorStack, bool adminPrivilege);
    }
}
