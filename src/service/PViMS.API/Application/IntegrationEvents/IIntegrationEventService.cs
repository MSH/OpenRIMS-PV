using PViMS.BuildingBlocks.EventBus.Events;
using System;
using System.Threading.Tasks;

namespace PVIMS.API.Application.IntegrationEvents
{
    public interface IIntegrationEventService
    {
        Task PublishEventsThroughEventBusAsync(Guid transactionId);
        Task AddAndSaveEventAsync(IntegrationEvent evt);
    }
}