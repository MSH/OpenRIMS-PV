using MediatR;
using PViMS.Core.Events;
using PVIMS.Core.Aggregates.NotificationAggregate;
using PVIMS.Core.Entities;
using PVIMS.Core.Repositories;
using PVIMS.Core.ValueTypes;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.DomainEventHandlers.E2BSubmitted
{
    public class AddNotificationWhenE2BSubmittedDomainEventHandler
                            : INotificationHandler<E2BSubmittedDomainEvent>
    {
        private readonly IRepositoryInt<Notification> _notificationRepository;
        private readonly IRepositoryInt<Config> _configRepository;
        private readonly IUnitOfWorkInt _unitOfWork;

        public AddNotificationWhenE2BSubmittedDomainEventHandler(
            IRepositoryInt<Notification> notificationRepository,
            IRepositoryInt<Config> configRepository,
            IUnitOfWorkInt unitOfWork)
        {
            _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
            _configRepository = configRepository ?? throw new ArgumentNullException(nameof(configRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task Handle(E2BSubmittedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var config = await _configRepository.GetAsync(c => c.ConfigType == ConfigType.ReportInstanceNewAlertCount);
            var alertCount = 1;
            if (config != null)
            {
                alertCount = Convert.ToInt32(config.ConfigValue);
            }

            var summary = $"E2B submitted for report {domainEvent.ReportInstance.PatientIdentifier}";
            var notificationType = NotificationType.FromName("Informational");
            var notificationClassification = NotificationClassification.FromName("E2BSubmitted");
            var contextRoute = "/clinical/feedbacksearch";
            var newNotification = new Notification(domainEvent.ReportInstance.CreatedBy, DateTime.Now.AddDays(alertCount), summary, "", notificationType, notificationClassification, contextRoute);

            await _notificationRepository.SaveAsync(newNotification);
            await _unitOfWork.CompleteAsync();
        }
    }
}
