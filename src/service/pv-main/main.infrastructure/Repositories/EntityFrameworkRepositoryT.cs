using OpenRIMS.PV.Main.Core.SeedWork;

namespace OpenRIMS.PV.Main.Infrastructure.Repositories
{
	public class EntityFrameworkRepository<T> : DomainRepository<T> where T : Entity<int>
	{
		public EntityFrameworkRepository(EntityFrameworkUnitOfWork unitOfWork) : base(unitOfWork._dbContext)
		{
		}
	}
}
