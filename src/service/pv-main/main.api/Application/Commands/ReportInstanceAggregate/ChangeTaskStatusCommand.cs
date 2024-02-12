using MediatR;
using OpenRIMS.PV.Main.Core.Aggregates.ReportInstanceAggregate;
using System;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Commands.ReportInstanceAggregate
{
    [DataContract]
    public class ChangeTaskStatusCommand
        : IRequest<bool>
    {
        [DataMember]
        public Guid WorkFlowGuid { get; private set; }

        [DataMember]
        public int ReportInstanceId { get; private set; }

        [DataMember]
        public int ReportInstanceTaskId { get; private set; }

        [DataMember]
        public TaskStatus TaskStatus { get; private set; }

        public ChangeTaskStatusCommand()
        {
        }

        public ChangeTaskStatusCommand(Guid workFlowGuid, int reportInstanceId, int reportInstanceTaskId, TaskStatus taskStatus) : this()
        {
            WorkFlowGuid = workFlowGuid;
            ReportInstanceId = reportInstanceId;
            ReportInstanceTaskId = reportInstanceTaskId;
            TaskStatus = taskStatus;
        }
    }
}
