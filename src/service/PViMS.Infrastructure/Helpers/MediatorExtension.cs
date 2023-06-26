using MediatR;
using PVIMS.Core.Entities;
using PVIMS.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace PViMS.Infrastructure.Helpers
{
    static class MediatorExtension
    {
        public static async Task DispatchDomainEventsAsync(this IMediator mediator, PVIMSDbContext context)
        {
            await HandleEntities(mediator, context);
            await HandleAuditedEntities(mediator, context);
        }

        private static async Task HandleEntities(IMediator mediator, PVIMSDbContext context)
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

        private static async Task HandleAuditedEntities(IMediator mediator, PVIMSDbContext context)
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
