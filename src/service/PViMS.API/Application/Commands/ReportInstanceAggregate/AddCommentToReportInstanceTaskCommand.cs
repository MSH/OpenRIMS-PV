using MediatR;
using PVIMS.API.Models;
using System;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Commands.ReportInstanceAggregate
{
    [DataContract]
    public class AddCommentToReportInstanceTaskCommand
        : IRequest<TaskCommentDto>
    {
        [DataMember]
        public Guid WorkFlowGuid { get; private set; }

        [DataMember]
        public int ReportInstanceId { get; private set; }

        [DataMember]
        public int ReportInstanceTaskId { get; private set; }

        [DataMember]
        public string Comment { get; private set; }

        public AddCommentToReportInstanceTaskCommand()
        {
        }

        public AddCommentToReportInstanceTaskCommand(Guid workFlowGuid, int reportInstanceId, int reportInstanceTaskId, string comment) : this()
        {
            WorkFlowGuid = workFlowGuid;
            ReportInstanceId = reportInstanceId;
            ReportInstanceTaskId = reportInstanceTaskId;
            Comment = comment;
        }
    }
}
