using OpenRIMS.PV.BuildingBlocks.EventBus.Events;
using System;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Application.IntegrationEvents
{
    public interface IIntegrationEventService
    {
        Task PublishEventsThroughEventBusAsync(Guid transactionId);
        Task AddAndSaveEventAsync(IntegrationEvent evt);
    }
}