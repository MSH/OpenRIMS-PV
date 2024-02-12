using OpenRIMS.PV.Main.Core.Repositories;
using OpenRIMS.PV.Main.Core.SeedWork;
using System;
using System.Collections;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.Infrastructure.Repositories
{
    public class EntityFrameworkUnitOfWork : IUnitOfWorkInt, IDisposable
    {
        public MainDbContext _dbContext;

        public EntityFrameworkUnitOfWork(MainDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        //public EntityFrameworkUnitOfWork(string connectionString)
        //{
        //    this.connectionString = connectionString;
        //}

        private Hashtable repositories;
        private bool disposed;

        /// <summary>
        /// Returns a repository for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>A repository for the specified entity type.</returns>
        public IRepositoryInt<TEntity> Repository<TEntity>() where TEntity : Entity<int>
        {
            //if (dbContext == null)
            //{
            //    Start();
            //}

            if (repositories == null)
                repositories = new Hashtable();

            var entityTypeName = typeof(TEntity).Name;

            if (!repositories.ContainsKey(entityTypeName))
            {
                var repositoryType = typeof(DomainRepository<>);

                // Could replace this with call to DependencyDepency
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), _dbContext);

                repositories.Add(entityTypeName, repositoryInstance);
            }

            return (IRepositoryInt<TEntity>)repositories[entityTypeName];
        }

        /// <summary>
        /// Starts a Unit of Work if one isn't already running.
        /// </summary>
        public void Start()
        {
            //if (dbContext == null)
            //{
            //    dbContext = String.IsNullOrWhiteSpace(connectionString) ? new MainDbContext() : new MainDbContext(connectionString);
            //}
        }

        /// <summary>
        /// Flushes all changes to the data store.
        /// </summary>
        public async Task<bool> CompleteAsync()
        {
            return await _dbContext.SaveEntitiesAsync();
        }

        /// <summary>
        /// Force execution on db seed
        /// </summary>
        public void Seed()
        {
            //dbContext.Seed(dbContext);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                    // Clean up any references to objects that need disposal (loop through hash set)
                    // If not using DepencyResovler.Resolve for Repo's
                }
            }
            disposed = true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
