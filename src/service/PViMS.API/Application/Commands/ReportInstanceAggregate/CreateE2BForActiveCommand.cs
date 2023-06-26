using MediatR;
using System;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Commands.ReportInstanceAggregate
{
    [DataContract]
    public class CreateE2BForActiveCommand
        : IRequest<bool>
    {
        [DataMember]
        public Guid WorkFlowGuid { get; private set; }

        [DataMember]
        public int ReportInstanceId { get; private set; }

        public CreateE2BForActiveCommand()
        {
        }

        public CreateE2BForActiveCommand(Guid workFlowGuid, int reportInstanceId) : this()
        {
            WorkFlowGuid = workFlowGuid;
            ReportInstanceId = reportInstanceId;
        }
    }
}