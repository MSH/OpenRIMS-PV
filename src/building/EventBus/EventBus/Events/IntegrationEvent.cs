using System;
using System.Text.Json.Serialization;

namespace PViMS.BuildingBlocks.EventBus.Events
{
    public record IntegrationEvent
    {
        public IntegrationEvent()
        {
            TransactionId = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
        }

        public IntegrationEvent(Guid acknowledgeId)
        {
            AcknowledgeId = acknowledgeId;
            TransactionId = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
            SenderId = Guid.Parse("E9B71820-76EC-4009-8078-47FCEC14F95C");
        }

        [JsonConstructor]
        public IntegrationEvent(Guid id, DateTime createDate)
        {
            TransactionId = id;
            CreationDate = createDate;
        }

        [JsonInclude]
        public Guid TransactionId { get; private init; }

        [JsonInclude]
        public Guid? AcknowledgeId { get; private init; }

        [JsonInclude]
        public Guid SenderId { get; private init; }

        [JsonInclude]
        public DateTime CreationDate { get; private init; }
    }
}
