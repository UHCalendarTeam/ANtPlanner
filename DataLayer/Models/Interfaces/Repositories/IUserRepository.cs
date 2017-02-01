using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Models.Entities.ACL;
using DataLayer.Models.Entities.Users;

namespace DataLayer.Models.Interfaces.Repositories
{
    public interface IUserRepository : IRepository<User, string>
    {
        Task<User> FindByEmail(string email);

        Task<User> FindByDisplayName(string name);

        Task<User> FindByPassword(string password);

        Task<User> FindByPrincipalId(string principalId);

        Task<User> FindByPrincipal(Principal principal);
    }
}
