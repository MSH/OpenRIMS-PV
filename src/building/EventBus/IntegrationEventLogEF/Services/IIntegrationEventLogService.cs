using Microsoft.EntityFrameworkCore.Storage;
using OpenRIMS.PV.BuildingBlocks.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenRIMS.PV.BuildingBlocks.IntegrationEventLogEF
{
    public interface IIntegrationEventLogService
    {
        Task<IEnumerable<IntegrationEventLogEntry>> RetrieveEventLogsPendingToPublishAsync(Guid transactionId);
        Task SaveEventAsync(IntegrationEvent @event, IDbContextTransaction transaction);
        Task MarkEventAsPublishedAsync(Guid eventId);
        Task MarkEventAsInProgressAsync(Guid eventId);
        Task MarkEventAsFailedAsync(Guid eventId);
    }
}