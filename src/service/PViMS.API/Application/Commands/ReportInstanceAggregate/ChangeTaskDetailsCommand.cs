using MediatR;
using System;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Commands.ReportInstanceAggregate
{
    [DataContract]
    public class ChangeTaskDetailsCommand
        : IRequest<bool>
    {
        [DataMember]
        public Guid WorkFlowGuid { get; private set; }

        [DataMember]
        public int ReportInstanceId { get; private set; }

        [DataMember]
        public int ReportInstanceTaskId { get; private set; }

        [DataMember]
        public string Source { get; private set; }

        [DataMember]
        public string Description { get; private set; }

        public ChangeTaskDetailsCommand()
        {
        }

        public ChangeTaskDetailsCommand(Guid workFlowGuid, int reportInstanceId, int reportInstanceTaskId, string source, string description) : this()
        {
            WorkFlowGuid = workFlowGuid;
            ReportInstanceId = reportInstanceId;
            ReportInstanceTaskId = reportInstanceTaskId;
            Source = source;
            Description = description;
        }
    }
}
