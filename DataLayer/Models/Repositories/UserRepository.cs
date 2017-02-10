using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Models.Entities.ACL;
using DataLayer.Models.Entities.Users;
using DataLayer.Models.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Models.Repositories
{
    public class UserRepository:BaseRepository<User,string>,IUserRepository
    {
        public UserRepository(CalDavContext context) : base(context)
        {
        }

        public Task<User> FindByEmail(string email)
        {
          return  DbSet.FirstOrDefaultAsync(u => u.Email.Equals(email));
        }

        public Task<User> FindByDisplayName(string name)
        {
            return DbSet.FirstOrDefaultAsync(u => u.DisplayName.Equals(name)); 
        }

        public Task<User> FindByPassword(string password)
        {
            return DbSet.FirstOrDefaultAsync(u => u.Password.Equals(password));
        }

        public Task<User> FindByPrincipalId(string principalId)
        {
            return DbSet.FirstOrDefaultAsync(u => u.PrincipalId.Equals(principalId));
        }

        public Task<User> FindByPrincipal(Principal principal)
        {
            return FindByPrincipalId(principal.Id);
        }
    }
}
