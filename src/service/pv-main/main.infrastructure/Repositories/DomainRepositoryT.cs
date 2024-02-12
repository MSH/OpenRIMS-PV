using Microsoft.EntityFrameworkCore;
using OpenRIMS.PV.Main.Core.Paging;
using OpenRIMS.PV.Main.Core.Repositories;
using OpenRIMS.PV.Main.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.Infrastructure.Repositories
{
    public class DomainRepository<TEntity> : IRepositoryInt<TEntity> where TEntity : Entity<int>
    {
        private readonly MainDbContext context;
        private readonly DbSet<TEntity> dbSet;

        public DomainRepository(MainDbContext dbContext)
        {
            context = dbContext;
            dbSet = context.Set<TEntity>();
        }

        public IQueryable<TEntity> Queryable()
        {
            return dbSet;
        }

        public ICollection<TEntity> List()
        {
            return List(null, null, (string)null);
        }

        public async Task<ICollection<TEntity>> ListAsync()
        {
            return await ListAsync(null, null, (string)null);
        }

        public ICollection<TEntity> List(Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
            params string[] includeExpressions)
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var include in includeExpressions.Where(s => !string.IsNullOrEmpty(s)))
            {
                query = query.Include(include);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
                return query.ToList();
        }

        public async Task<ICollection<TEntity>> ListAsync(Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
            params string[] includeExpressions)
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var include in includeExpressions.Where(s => !string.IsNullOrEmpty(s)))
            {
                query = query.Include(include);
            }

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }
            else
                return await query.ToListAsync();
        }

        public ICollection<TEntity> List<TProperty>(Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
            Expression<Func<TEntity, TProperty>> includeExpressions)
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            query = query.Include(includeExpressions);

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
                return query.ToList();
        }

        public async Task<ICollection<TEntity>> ListAsync<TProperty>(Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
            Expression<Func<TEntity, TProperty>> includeExpressions)
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            query = query.Include(includeExpressions);

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }
            else
                return await query.ToListAsync();
        }

        public PagedCollection<TEntity> List(IPagingInfo pagingInfo,
            Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
            params string[] includeExpressions)
        {
            IQueryable<TEntity> queryBeforePaging = dbSet;

            if (filter != null)
            {
                queryBeforePaging = queryBeforePaging.Where(filter);
            }

            foreach (var include in includeExpressions.Where(s => !string.IsNullOrEmpty(s)))
            {
                queryBeforePaging = queryBeforePaging.Include(include);
            }

            var count = queryBeforePaging.Count();

            if (orderBy != null)
            {
                queryBeforePaging = orderBy(queryBeforePaging);
            }
            else
                queryBeforePaging = queryBeforePaging.OrderBy(h => h.Id);

            return PagedCollection<TEntity>.Create(queryBeforePaging,
                pagingInfo.PageNumber,
                pagingInfo.PageSize);
        }

        public async Task<PagedCollection<TEntity>> ListAsync(IPagingInfo pagingInfo,
            Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
            params string[] includeExpressions)
        {
            IQueryable<TEntity> queryBeforePaging = dbSet;

            if (filter != null)
            {
                queryBeforePaging = queryBeforePaging.Where(filter);
            }

            foreach (var include in includeExpressions.Where(s => !string.IsNullOrEmpty(s)))
            {
                queryBeforePaging = queryBeforePaging.Include(include);
            }

            var count = await queryBeforePaging.CountAsync();

            if (orderBy != null)
            {
                queryBeforePaging = orderBy(queryBeforePaging);
            }
            else
                queryBeforePaging = queryBeforePaging.OrderBy(h => h.Id);

            return PagedCollection<TEntity>.Create(queryBeforePaging,
                pagingInfo.PageNumber,
                pagingInfo.PageSize);
        }

        public PagedCollection<TEntity> List<TProperty>(IPagingInfo pagingInfo,
            Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
            Expression<Func<TEntity, TProperty>> includeExpressions)
        {
            IQueryable<TEntity> queryBeforePaging = dbSet;

            if (filter != null)
            {
                queryBeforePaging = queryBeforePaging.Where(filter);
            }

            queryBeforePaging = queryBeforePaging.Include(includeExpressions);

            var count = queryBeforePaging.Count();

            if (orderBy != null)
            {
                queryBeforePaging = orderBy(queryBeforePaging);
            }
            else
                queryBeforePaging = queryBeforePaging.OrderBy(h => h.Id);

            return PagedCollection<TEntity>.Create(queryBeforePaging,
                pagingInfo.PageNumber,
                pagingInfo.PageSize);
        }

        public TEntity Get(object entityId)
        {
            return dbSet.Find(entityId);
        }

        public async Task<TEntity> GetAsync(object entityId)
        {
            return await dbSet.FindAsync(entityId);
        }

        public TEntity Get(Expression<Func<TEntity, bool>> filter)
        {
            IQueryable<TEntity> query = dbSet;

            return query.SingleOrDefault(filter);
        }

        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> filter)
        {
            IQueryable<TEntity> query = dbSet;

            return await query.SingleOrDefaultAsync(filter);
        }

        public TEntity Get(object entityId, string[] includeExpressions)
        {
            var query = Queryable();

            foreach (var include in includeExpressions.Where(s => !string.IsNullOrEmpty(s)))
            {
                query = query.Include(include);
            }

            return query.SingleOrDefault(q => q.Id == (long)entityId);
        }

        public async Task<TEntity> GetAsync(object entityId, string[] includeExpressions)
        {
            var query = Queryable();

            foreach (var include in includeExpressions.Where(s => !string.IsNullOrEmpty(s)))
            {
                query = query.Include(include);
            }

            return await query.SingleOrDefaultAsync(q => q.Id == (int)entityId);
        }

        public TEntity Get(Expression<Func<TEntity, bool>> filter, string[] includeExpressions)
        {
            var query = Queryable();

            foreach (var include in includeExpressions.Where(s => !string.IsNullOrEmpty(s)))
            {
                query = query.Include(include);
            }

            return query.SingleOrDefault(filter);
        }

        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> filter, string[] includeExpressions)
        {
            var query = Queryable();

            foreach (var include in includeExpressions.Where(s => !string.IsNullOrEmpty(s)))
            {
                query = query.Include(include);
            }

            return await query.AsSplitQuery().SingleOrDefaultAsync(filter);
        }

        public void Save(TEntity entityToSave)
        {
            dbSet.Add(entityToSave);

            // Need the Id to be returned.
            context.SaveChanges();
        }

        public async Task SaveAsync(TEntity entityToSave)
        {
            dbSet.Add(entityToSave);

            // Need the Id to be returned.
            await context.SaveEntitiesAsync();
        }

        public void Save(TEntity[] entitiesToSave)
        {
            dbSet.AddRange(entitiesToSave);
        }

        public void Update(TEntity entityToUpdate)
        {
            try
            {
                dbSet.Attach(entityToUpdate);
                context.Entry(entityToUpdate).State = EntityState.Modified;
            }
            finally
            {
            }
        }

        public void Update(TEntity[] entitiesToUpdate)
        {
            foreach (var entityToUpdate in entitiesToUpdate)
            {
                Update(entityToUpdate);
            }
        }

        public void Delete(TEntity entityToDelete)
        {
            if (context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }

            dbSet.Remove(entityToDelete);
        }

        public void Delete(object entityToDeleteId)
        {
            TEntity entityToDelete = dbSet.Find(entityToDeleteId);
            Delete(entityToDelete);
        }

        public void Delete(Expression<Func<TEntity, bool>> filter = null)
        {
            var entitiesToDelete = dbSet.Where(filter);
            dbSet.RemoveRange(entitiesToDelete);
        }

        public bool Exists(Expression<Func<TEntity, bool>> filter = null)
        {
            return filter == null
                ? dbSet.Any()
                : dbSet.Any(filter);
        }

        public async Task<int> ExecuteSqlCommandAsync(string sql, params SqlParameter[] parameters)
        {
            return await context.Database.ExecuteSqlRawAsync(sql, parameters);
        }

        public int ExecuteSqlCommand(string sql, params SqlParameter[] parameters)
        {
            return context.Database.ExecuteSqlRaw(sql, parameters);
        }
    }
}
