using OpenRIMS.PV.Main.Core.Paging;
using OpenRIMS.PV.Main.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.Core.Repositories
{
    /// <summary>
    /// Generic interface for Entity repositories.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public interface IRepositoryInt<TEntity> where TEntity : Entity<int>
    {
        /// <summary>
        /// LINQ Queryable interface.
        /// </summary>
        /// <returns></returns>
        IQueryable<TEntity> Queryable();

        /// <summary>
        /// Returns a collection of entities.
        /// </summary>
        /// <returns></returns>
        ICollection<TEntity> List();

        /// <summary>
        /// Returns a collection of entities asynchronously.
        /// </summary>
        /// <returns></returns>
        Task<ICollection<TEntity>> ListAsync();

        /// <summary>
        /// Returns a collection of entities, optionally filtered and sorted with specified associated entities eagerly loaded.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="includeExpressions">The related entities to eagerly load.</param>
        /// <returns></returns>
        ICollection<TEntity> List(Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            params string[] includeExpressions);

        /// <summary>
        /// Returns a collection of entities, optionally filtered and sorted with specified associated entities eagerly loaded asynchronously.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="includeExpressions">The related entities to eagerly load.</param>
        /// <returns></returns>
        Task<ICollection<TEntity>> ListAsync(Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            params string[] includeExpressions);

        /// <summary>
        /// Returns a list entities, optionally filtered and sorted with specified associated entities eagerly loaded asynchronously.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="includeExpressions">Property expressions representing related entities to be eagerly loaded.</param>
        /// <returns></returns>
        ICollection<TEntity> List<TProperty>(Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
            Expression<Func<TEntity, TProperty>> includeExpressions);

        /// <summary>
        /// Returns a list entities, optionally filtered and sorted with specified associated entities eagerly loaded.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="includeExpressions">Property expressions representing related entities to be eagerly loaded.</param>
        /// <returns></returns>
        Task<ICollection<TEntity>> ListAsync<TProperty>(Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
            Expression<Func<TEntity, TProperty>> includeExpressions);

        /// <summary>
        /// Returns a paged collection of entities, optionally filtered and sorted with specified associated entities eagerly loaded.
        /// </summary>
        /// <param name="pagingInfo">The paging information.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="includeExpressions">The related entities to eagerly load.</param>
        /// <returns></returns>
        PagedCollection<TEntity> List(IPagingInfo pagingInfo,
            Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
            params string[] includeExpressions);

        /// <summary>
        /// Returns a paged collection of entities, optionally filtered and sorted with specified associated entities eagerly loaded.
        /// </summary>
        /// <param name="pagingInfo">The paging information.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="includeExpressions">The related entities to eagerly load.</param>
        /// <returns></returns>
        Task<PagedCollection<TEntity>> ListAsync(IPagingInfo pagingInfo,
            Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
            params string[] includeExpressions);

        /// <summary>
        /// Returns a paged collection of entities, optionally filtered and sorted with specified associated entities eagerly loaded.
        /// </summary>
        /// <param name="pagingInfo">The paging information.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="includeExpressions">Property expressions representing related entities to be eagerly loaded.</param>
        /// <returns></returns>
        PagedCollection<TEntity> List<TProperty>(IPagingInfo pagingInfo,
            Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
            Expression<Func<TEntity, TProperty>> includeExpressions);

        /// <summary>
        /// Gets the specified entity by identifier.
        /// </summary>
        /// <param name="entityId">The entity identifier.</param>
        /// <returns></returns>
        TEntity Get(object entityId);

        /// <summary>
        /// Gets the specified entity by identifier asynchronously.
        /// </summary>
        /// <param name="entityId">The entity identifier.</param>
        /// <returns></returns>
        Task<TEntity> GetAsync(object entityId);

        /// <summary>
        /// Gets the specified entity by expression.
        /// </summary>
        /// <param name="filter">Filter expression</param>
        /// <returns></returns>
        TEntity Get(Expression<Func<TEntity, bool>> filter);

        /// <summary>
        /// Gets the specified entity by identifier asynchronously.
        /// </summary>
        /// <param name="filter">Filter expression</param>
        /// <returns></returns>
        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> filter);

        /// <summary>
        /// Gets the specified entity by identifier and allows for eager loading of entities.
        /// </summary>
        /// <param name="entityId">The entity identifier.</param>
        /// <param name="includeExpressions">The related entities to eagerly load.</param>
        /// <returns></returns>
        TEntity Get(object entityId, params string[] includeExpressions);

        /// <summary>
        /// Gets the specified entity by identifier asynchronously and allows for eager loading of entities.
        /// </summary>
        /// <param name="entityId">The entity identifier.</param>
        /// <param name="includeExpressions">The related entities to eagerly load.</param>
        /// <returns></returns>
        Task<TEntity> GetAsync(object entityId, params string[] includeExpressions);

        /// <summary>
        /// Gets the specified entity by expression and allows for eager loading of entities.
        /// </summary>
        /// <param name="filter">Filter expression</param>
        /// <param name="includeExpressions">The related entities to eagerly load.</param>
        /// <returns></returns>
        TEntity Get(Expression<Func<TEntity, bool>> filter, params string[] includeExpressions);

        /// <summary>
        /// Gets the specified entity by expression asynchronously and allows for eager loading of entities.
        /// </summary>
        /// <param name="filter">Filter expression</param>
        /// <param name="includeExpressions">The related entities to eagerly load.</param>
        /// <returns></returns>
        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> filter, string[] includeExpressions);

        /// <summary>
        /// Saves the specified entity.
        /// </summary>
        /// <param name="entityToSave">The entity to save.</param>
        void Save(TEntity entityToSave);

        /// <summary>
        /// Saves the specified entity asynchronously.
        /// </summary>
        /// <param name="entityToSave">The entity to save.</param>
        Task SaveAsync(TEntity entityToSave);

        /// <summary>
        /// Saves the specified entities.
        /// </summary>
        /// <param name="entitiesToSave">The entities to save.</param>
        void Save(TEntity[] entitiesToSave);

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entityToUpdate">The entity to update.</param>
        void Update(TEntity entityToUpdate);

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entityToDelete">The entity to delete.</param>
        void Delete(TEntity entityToDelete);

        /// <summary>
        /// Updates the specified entities.
        /// </summary>
        /// <param name="entitiesToUpdate">The entities to update.</param>
        void Update(TEntity[] entitiesToUpdate);

        /// <summary>
        /// Deletes the entity represented by the entityToDeleteId.
        /// </summary>
        /// <param name="entityToDeleteId">The entity to delete identifier.</param>
        void Delete(object entityToDeleteId);

        /// <summary>
        /// Remove all entities that match the filter
        /// </summary>
        /// <param name="filter">The filter.</param>
        void Delete(Expression<Func<TEntity, bool>> filter = null);

        /// <summary>
        /// Checks if an instance of the TEntity exists that, optionally, matches the specified filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        bool Exists(Expression<Func<TEntity, bool>> filter = null);

        /// <summary>
        /// Executes the SQL.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        int ExecuteSqlCommand(string sql, params SqlParameter[] parameters);
    }
}
