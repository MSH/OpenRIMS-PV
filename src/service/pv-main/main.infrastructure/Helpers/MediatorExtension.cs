using MediatR;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.Infrastructure.Helpers
{
    static class MediatorExtension
    {
        public static async Task DispatchDomainEventsAsync(this IMediator mediator, MainDbContext context)
        {
            await HandleEntities(mediator, context);
            await HandleAuditedEntities(mediator, context);
        }

        private static async Task HandleEntities(IMediator mediator, MainDbContext context)
        {
            var domainEntities = context.ChangeTracker
                .Entries<EntityBase>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();

            domainEntities.ToList()
                .ForEach(entity => entity.Entity.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
                await mediator.Publish(domainEvent);
        }

        private static async Task HandleAuditedEntities(IMediator mediator, MainDbContext context)
        {
            var domainEntities = context.ChangeTracker
                .Entries<AuditedEntityBase>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();

            domainEntities.ToList()
                .ForEach(entity => entity.Entity.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
                await mediator.Publish(domainEvent);
        }
    }
}
