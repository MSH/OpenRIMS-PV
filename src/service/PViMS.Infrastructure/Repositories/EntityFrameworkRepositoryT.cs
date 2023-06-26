using PVIMS.Core.SeedWork;

namespace PVIMS.Infrastructure.Repositories
{
	public class EntityFrameworkRepository<T> : DomainRepository<T> where T : Entity<int>
	{
		public EntityFrameworkRepository(EntityFrameworkUnitOfWork unitOfWork) : base(unitOfWork._dbContext)
		{
		}
	}
}
