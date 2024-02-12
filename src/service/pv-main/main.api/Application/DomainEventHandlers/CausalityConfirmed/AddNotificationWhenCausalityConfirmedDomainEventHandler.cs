using MediatR;
using OpenRIMS.PV.Main.Core.Events;
using OpenRIMS.PV.Main.Core.Aggregates.NotificationAggregate;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.Repositories;
using OpenRIMS.PV.Main.Core.ValueTypes;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.DomainEventHandlers.TaskAdded
{
    public class AddNotificationWhenCausalityConfirmedDomainEventHandler
                            : INotificationHandler<CausalityConfirmedDomainEvent>
    {
        private readonly IRepositoryInt<Notification> _notificationRepository;
        private readonly IRepositoryInt<Config> _configRepository;
        private readonly IUnitOfWorkInt _unitOfWork;

        public AddNotificationWhenCausalityConfirmedDomainEventHandler(
            IRepositoryInt<Notification> notificationRepository,
            IRepositoryInt<Config> configRepository,
            IUnitOfWorkInt unitOfWork)
        {
            _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
            _configRepository = configRepository ?? throw new ArgumentNullException(nameof(configRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task Handle(CausalityConfirmedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var config = await _configRepository.GetAsync(c => c.ConfigType == ConfigType.ReportInstanceFeedbackAlertCount);
            var alertCount = 1;
            if (config != null)
            {
                alertCount = Convert.ToInt32(config.ConfigValue);
            }

            var summary = $"Causality and terminology set for report {domainEvent.ReportInstance.PatientIdentifier}";
            var notificationType = NotificationType.FromName("Informational");
            var notificationClassification = NotificationClassification.FromName("CausalityAndTerminologySet");
            var contextRoute = "/clinical/feedbacksearch";
            var newNotification = new Notification(domainEvent.ReportInstance.CreatedBy, DateTime.Now.AddDays(alertCount), summary, "", notificationType, notificationClassification, contextRoute);

            await _notificationRepository.SaveAsync(newNotification);
            await _unitOfWork.CompleteAsync();
        }
    }
}
