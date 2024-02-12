using MediatR;
using System;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Commands.ReportInstanceAggregate
{
    [DataContract]
    public class ChangeReportInstanceActivityCommand
        : IRequest<bool>
    {
        [DataMember]
        public Guid WorkFlowGuid { get; private set; }

        [DataMember]
        public int ReportInstanceId { get; private set; }

        [DataMember]
        public string Comments { get; set; }

        [DataMember]
        public string CurrentExecutionStatus { get; set; }

        [DataMember]
        public string NewExecutionStatus { get; set; }

        [DataMember]
        public string ContextCode { get; set; }

        [DataMember]
        public DateTime? ContextDate { get; set; }

        public ChangeReportInstanceActivityCommand()
        {
        }

        public ChangeReportInstanceActivityCommand(Guid workFlowGuid, int reportInstanceId, string comments, string currentExecutionStatus, string newExecutionStatus, string contextCode, DateTime? contextDate) : this()
        {
            WorkFlowGuid = workFlowGuid;
            ReportInstanceId = reportInstanceId;
            Comments = comments;
            CurrentExecutionStatus = currentExecutionStatus;
            NewExecutionStatus = newExecutionStatus;
            ContextCode = contextCode;
            ContextDate = contextDate;
        }
    }
}
