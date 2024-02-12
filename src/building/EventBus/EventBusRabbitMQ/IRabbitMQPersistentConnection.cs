using RabbitMQ.Client;
using System;

namespace OpenRIMS.PV.BuildingBlocks.EventBusRabbitMQ
{
    public interface IRabbitMQPersistentConnection
        : IDisposable
    {
        bool IsConnected { get; }

        bool TryConnect();

        IModel CreateModel();
    }
}