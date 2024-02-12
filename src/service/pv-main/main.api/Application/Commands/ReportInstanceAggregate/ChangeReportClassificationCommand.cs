using MediatR;
using OpenRIMS.PV.Main.Core.Aggregates.ReportInstanceAggregate;
using System;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Commands.ReportInstanceAggregate
{
    [DataContract]
    public class ChangeReportClassificationCommand
        : IRequest<bool>
    {
        [DataMember]
        public Guid WorkFlowGuid { get; private set; }

        [DataMember]
        public int ReportInstanceId { get; private set; }

        [DataMember]
        public ReportClassification ReportClassification { get; private set; }

        public ChangeReportClassificationCommand()
        {
        }

        public ChangeReportClassificationCommand(Guid workFlowGuid, int reportInstanceId, ReportClassification reportClassification) : this()
        {
            WorkFlowGuid = workFlowGuid;
            ReportInstanceId = reportInstanceId;
            ReportClassification = reportClassification;
        }
    }
}
