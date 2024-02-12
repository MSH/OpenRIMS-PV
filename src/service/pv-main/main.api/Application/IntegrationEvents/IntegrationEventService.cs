using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenRIMS.PV.Main.API;
using OpenRIMS.PV.BuildingBlocks.EventBus.Abstractions;
using OpenRIMS.PV.BuildingBlocks.EventBus.Events;
using OpenRIMS.PV.BuildingBlocks.IntegrationEventLogEF;
using OpenRIMS.PV.Main.Infrastructure;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.IntegrationEvents
{
    public class IntegrationEventService : IIntegrationEventService
    {
        private readonly IEventBus _eventBus;
        private readonly MainDbContext _dbContext;
        private readonly Func<DbConnection, IIntegrationEventLogService> _integrationEventLogServiceFactory;
        private readonly IIntegrationEventLogService _eventLogService;
        private readonly ILogger<IntegrationEventService> _logger;

        public IntegrationEventService(
            IEventBus eventBus,
            MainDbContext dbContext,
            Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory,
            IntegrationEventLogContext eventLogContext,
            ILogger<IntegrationEventService> logger)
        {
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _integrationEventLogServiceFactory = integrationEventLogServiceFactory ?? throw new ArgumentNullException(nameof(integrationEventLogServiceFactory));
            _eventLogService = _integrationEventLogServiceFactory(_dbContext.Database.GetDbConnection());
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task PublishEventsThroughEventBusAsync(Guid transactionId)
        {
            var pendingLogEvents = await _eventLogService.RetrieveEventLogsPendingToPublishAsync(transactionId);

            foreach (var logEvt in pendingLogEvents)
            {
                _logger.LogInformation("----- Publishing integration event: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", logEvt.EventId, Program.AppName, logEvt.IntegrationEvent);

                try
                {
                    await _eventLogService.MarkEventAsInProgressAsync(logEvt.EventId);
                    _eventBus.Publish(logEvt.IntegrationEvent);
                    await _eventLogService.MarkEventAsPublishedAsync(logEvt.EventId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "ERROR publishing integration event: {IntegrationEventId} from {AppName}", logEvt.EventId, Program.AppName);

                    await _eventLogService.MarkEventAsFailedAsync(logEvt.EventId);
                }
            }
        }

        public async Task AddAndSaveEventAsync(IntegrationEvent evt)
        {
            _logger.LogInformation("----- Enqueuing integration event {IntegrationEventId} to repository ({@IntegrationEvent})", evt.TransactionId, evt);

            await _eventLogService.SaveEventAsync(evt, _dbContext.GetCurrentTransaction());
        }
    }
}