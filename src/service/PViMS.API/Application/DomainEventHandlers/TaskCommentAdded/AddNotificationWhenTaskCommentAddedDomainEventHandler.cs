using MediatR;
using PViMS.Core.Events;
using PVIMS.Core.Aggregates.NotificationAggregate;
using PVIMS.Core.Aggregates.UserAggregate;
using PVIMS.Core.Entities;
using PVIMS.Core.Repositories;
using PVIMS.Core.ValueTypes;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.DomainEventHandlers.TaskCommentAdded
{
    public class AddNotificationWhenTaskCommentAddedDomainEventHandler
                            : INotificationHandler<TaskCommentAddedDomainEvent>
    {
        private readonly IRepositoryInt<Notification> _notificationRepository;
        private readonly IRepositoryInt<Config> _configRepository;
        private readonly IUnitOfWorkInt _unitOfWork;

        public AddNotificationWhenTaskCommentAddedDomainEventHandler(
            IRepositoryInt<Notification> notificationRepository,
            IRepositoryInt<Config> configRepository,
            IUnitOfWorkInt unitOfWork)
        {
            _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
            _configRepository = configRepository ?? throw new ArgumentNullException(nameof(configRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task Handle(TaskCommentAddedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var config = await _configRepository.GetAsync(c => c.ConfigType == ConfigType.ReportInstanceNewAlertCount);
            var alertCount = 1;
            if (config != null)
            {
                alertCount = Convert.ToInt32(config.ConfigValue);
            }

            var summary = $"Task comment has been added for patient {domainEvent.Comment.ReportInstanceTask.ReportInstance.PatientIdentifier}";
            var detail = domainEvent.Comment.Comment.Length > 100 ? $"{domainEvent.Comment.Comment.Substring(0, 100)} ..." : domainEvent.Comment.Comment;

            var notificationType = NotificationType.FromName("Informational");
            var notificationClassification = NotificationClassification.FromName("NewTaskComment");
            
            var newNotification = new Notification(GetDestinationUser(domainEvent), DateTime.Now.AddDays(alertCount), summary, detail, notificationType, notificationClassification, GetDestinationRoute(domainEvent));

            await _notificationRepository.SaveAsync(newNotification);
            await _unitOfWork.CompleteAsync();
        }
        private User GetDestinationUser(TaskCommentAddedDomainEvent domainEvent)
        {
            User destinationUser;
            if (domainEvent.Comment.CreatedById == domainEvent.Comment.ReportInstanceTask.ReportInstance.CreatedById)
            {
                // comment created by clinician, so send to person who created task
                destinationUser = domainEvent.Comment.ReportInstanceTask.CreatedBy;
            }
            else
            {
                // comment created by analyst, so send to person who created report
                destinationUser = domainEvent.Comment.ReportInstanceTask.ReportInstance.CreatedBy;
            }

            return destinationUser;
        }

        private string GetDestinationRoute(TaskCommentAddedDomainEvent domainEvent)
        {
            if (domainEvent.Comment.CreatedById == domainEvent.Comment.ReportInstanceTask.ReportInstance.CreatedById)
            {
                // comment created by clinician, so route user to analytical report search
                return $"/analytical/reporttask/{domainEvent.Comment.ReportInstanceTask.ReportInstance.WorkFlow.WorkFlowGuid}/{domainEvent.Comment.ReportInstanceTask.ReportInstance.Id}";
            }
            else
            {
                // comment created by analyst, so route to clinical feedback
                return $"/clinical/feedbacksearch";
            }
        }
    }
}
