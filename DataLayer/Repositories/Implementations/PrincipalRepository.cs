using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Models.ACL;
using DataLayer.Models.Entities;

namespace DataLayer.Repositories.Implementations
{
    public class PrincipalRepository : IRepository<Principal, string>
    {
        public IList<Principal> GetAll()
        {
            throw new NotImplementedException();
        }

        public Principal Get(string url)
        {
            throw new NotImplementedException();
        }

        public void Add(Principal entity)
        {
            throw new NotImplementedException();
        }

        public void Remove(Principal entity)
        {
            throw new NotImplementedException();
        }

        public void Remove(string url)
        {
            throw new NotImplementedException();
        }

        public int Count()
        {
            throw new NotImplementedException();
        }

        public bool Exist(string url)
        {
            throw new NotImplementedException();
        }

        public IList<Property> GetAllProperties(string url)
        {
            throw new NotImplementedException();
        }

        public IList<Property> GetProperties(string url, List<KeyValuePair<string, string>> propertiesNameandNs)
        {
            throw new NotImplementedException();
        }

        public IList<KeyValuePair<string, string>> GetAllPropname(string url)
        {
            throw new NotImplementedException();
        }

        public bool RemoveProperty(string url, KeyValuePair<string, string> propertyNameNs, Stack<string> errorStack)
        {
            throw new NotImplementedException();
        }

        public bool CreateOrModifyProperty(string url, string propName, string propNs, string propValue, Stack<string> errorStack,
            bool adminPrivilege)
        {
            throw new NotImplementedException();
        }
    }
}
