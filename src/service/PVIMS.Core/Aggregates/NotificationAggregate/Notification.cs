using PVIMS.Core.Aggregates.UserAggregate;
using PVIMS.Core.Entities;
using PVIMS.Core.SeedWork;
using System;

namespace PVIMS.Core.Aggregates.NotificationAggregate
{
    public class Notification 
        : AuditedEntityBase, IAggregateRoot
    {
        public int? DestinationUserId { get; private set; }
        public virtual User DestinationUser { get; private set; }

        public DateTime? ValidUntilDate { get; private set; }
        public string Summary { get; private set; }
        public string Detail { get; private set; }

        public int NotificationTypeId { get; private set; }
        public int NotificationClassificationId { get; private set; }

        public string ContextRoute { get; private set; }

        protected Notification()
        {
        }

        public Notification(User destinationUser, DateTime? validUntilDate, string summary, string detail, NotificationType notificationType, NotificationClassification notificationClassification, string contextRoute)
        {
            DestinationUser = destinationUser;
            DestinationUserId = destinationUser.Id;
            ValidUntilDate = validUntilDate;
            Summary = summary;
            Detail = detail;
            NotificationTypeId = notificationType.Id;
            NotificationClassificationId = notificationClassification.Id;
            ContextRoute = contextRoute;
        }
    }
}