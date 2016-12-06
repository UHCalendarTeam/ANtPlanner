using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DataLayer.Models.Interfaces.Repositories
{
    public interface IRepository<TEnt, in TPk> where TEnt : class
    {
        Task<IList<TEnt>> GetAll();
        TEnt Find(TPk id);
        Task<TEnt> FindAsync(TPk id);
        void Add(TEnt entity);
        Task Remove(TEnt entity);

        Task Remove(TPk id);

        Task<int> Count();

        Task<bool> Exist(TPk id);

        IQueryable<TEnt> Get(Expression<Func<TEnt, bool>> predicate);

        Task<int> SaveChanges();

        Task<int> SaveChangesAsync();
    }
}