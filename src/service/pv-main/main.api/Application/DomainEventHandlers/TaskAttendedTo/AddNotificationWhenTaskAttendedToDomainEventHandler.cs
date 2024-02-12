using MediatR;
using OpenRIMS.PV.Main.Core.Events;
using OpenRIMS.PV.Main.Core.Aggregates.NotificationAggregate;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Repositories;
using OpenRIMS.PV.Main.Core.ValueTypes;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.DomainEventHandlers.TaskAttendedTo
{
    public class AddNotificationWhenTaskAttendedToDomainEventHandler
                            : INotificationHandler<TaskAttendedToDomainEvent>
    {
        private readonly IRepositoryInt<Notification> _notificationRepository;
        private readonly IRepositoryInt<Config> _configRepository;
        private readonly IUnitOfWorkInt _unitOfWork;

        public AddNotificationWhenTaskAttendedToDomainEventHandler(
            IRepositoryInt<Notification> notificationRepository,
            IRepositoryInt<Config> configRepository,
            IUnitOfWorkInt unitOfWork)
        {
            _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
            _configRepository = configRepository ?? throw new ArgumentNullException(nameof(configRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task Handle(TaskAttendedToDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var config = await _configRepository.GetAsync(c => c.ConfigType == ConfigType.ReportInstanceNewAlertCount);
            var alertCount = 1;
            if (config != null)
            {
                alertCount = Convert.ToInt32(config.ConfigValue);
            }

            var summary = $"A task for a report for patient {domainEvent.Task.ReportInstance.PatientIdentifier} has been attended to";
            var notificationType = NotificationType.FromName("Informational");
            var notificationClassification = NotificationClassification.FromName("AttendedToTask");
            var contextRoute = $"/analytical/reporttask/{domainEvent.Task.ReportInstance.WorkFlow.WorkFlowGuid}/{domainEvent.Task.ReportInstance.Id}";
            var newNotification = new Notification(domainEvent.Task.CreatedBy, DateTime.Now.AddDays(alertCount), summary, "", notificationType, notificationClassification, contextRoute);

            await _notificationRepository.SaveAsync(newNotification);
            await _unitOfWork.CompleteAsync();
        }
    }
}
