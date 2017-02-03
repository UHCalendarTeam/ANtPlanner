using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DataLayer.Contexts;
using DataLayer.Models.Interfaces;
using DataLayer.Models.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Models.Repositories
{
    public abstract class BaseRepository<TEnt, TKey> : IRepository<TEnt, TKey> where TEnt : class,IEntity<TKey>
    {
        protected readonly CalDavContext Context;
        protected DbSet<TEnt> DbSet;

        protected BaseRepository(CalDavContext context)
        {
            Context = context;
            DbSet = Context.Set<TEnt>();
        }

        public virtual void Add(TEnt entity)
        {
            DbSet.Add(entity);
            Context.SaveChanges();
        }

        public void AddRange(IEnumerable<TEnt> entities)
        {
            DbSet.AddRange(entities);
            Context.SaveChanges();
        }

        public virtual async Task Remove(TEnt entity)
        {
            DbSet.Remove(entity);
            await SaveChangesAsync();
        }

        public virtual async Task Remove(TKey id)
        {
            var collection = await FindAsync(id);
            await Remove(collection);
        }

        public virtual TEnt Find(TKey id)
        {
            return DbSet.FirstOrDefault(e => e.Id.Equals(id));
        }

        public virtual async Task<TEnt> FindAsync(TKey id)
        {
            return await DbSet.FirstOrDefaultAsync(e => e.Id.Equals(id));
        }

        public virtual async Task<IList<TEnt>> GetAll()
        {
            return await DbSet.ToListAsync();
        }

        public virtual async Task<bool> Exist(TKey id)
        {
           return await DbSet.AnyAsync(e => e.Id.Equals(id));
        }

        public virtual IQueryable<TEnt> Get(Expression<Func<TEnt, bool>> predicate)
        {
            return DbSet.Where(predicate);
        }

        public virtual async Task<int> Count()
        {
            return await DbSet.CountAsync();
        }

        public virtual Task<int> SaveChanges()
        {
           return Context.SaveChangesAsync();
        }

        public virtual async Task<int> SaveChangesAsync()
        {
            return await SaveChanges();
        }
    }
}
