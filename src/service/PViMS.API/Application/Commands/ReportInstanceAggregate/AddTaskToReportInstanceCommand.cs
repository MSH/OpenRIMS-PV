using MediatR;
using PVIMS.API.Models;
using PVIMS.Core.Aggregates.ReportInstanceAggregate;
using System;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Commands.ReportInstanceAggregate
{
    [DataContract]
    public class AddTaskToReportInstanceCommand
        : IRequest<TaskDto>
    {
        [DataMember]
        public Guid WorkFlowGuid { get; private set; }

        [DataMember]
        public int ReportInstanceId { get; private set; }

        [DataMember]
        public string Source { get; private set; }

        [DataMember]
        public string Description { get; private set; }

        [DataMember]
        public TaskType TaskType { get; private set; }

        public AddTaskToReportInstanceCommand()
        {
        }

        public AddTaskToReportInstanceCommand(Guid workFlowGuid, int reportInstanceId, string source, string description, TaskType taskType) : this()
        {
            WorkFlowGuid = workFlowGuid;
            ReportInstanceId = reportInstanceId;
            Source = source;
            Description = description;
            TaskType = taskType;
        }
    }
}
