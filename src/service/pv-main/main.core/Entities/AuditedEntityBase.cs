using MediatR;
using OpenRIMS.PV.Main.Core.Aggregates.UserAggregate;
using OpenRIMS.PV.Main.Core.SeedWork;
using System.Collections.Generic;

namespace OpenRIMS.PV.Main.Core.Entities
{
    public abstract class AuditedEntityBase : AuditedEntity<int, User>
	{
        private List<INotification> _domainEvents;
        public IReadOnlyCollection<INotification> DomainEvents => _domainEvents?.AsReadOnly();

        public string GetCreatedStamp()
        {
            return string.Format("Created by {0} on {1}", CreatedBy != null ? CreatedBy.FullName : "UNKNOWN", Created.ToString("yyyy-MM-dd"));
        }

        public string GetLastUpdatedStamp()
        {
            if (!LastUpdated.HasValue)
                return "NOT UPDATED";

            return string.Format("Updated by {0} on {1}", UpdatedBy != null ? UpdatedBy.FullName : "UNKNOWN", LastUpdated.Value.ToString("yyyy-MM-dd"));
        }


        public void AddDomainEvent(INotification eventItem)
        {
            _domainEvents = _domainEvents ?? new List<INotification>();
            _domainEvents.Add(eventItem);
        }

        public void RemoveDomainEvent(INotification eventItem)
        {
            _domainEvents?.Remove(eventItem);
        }

        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }
    }
}
